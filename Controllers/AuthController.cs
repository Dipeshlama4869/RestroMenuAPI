using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace RestroMenu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> _usersCollection;

        public AuthController(DbHelper dbHelper)
        {
            _usersCollection = dbHelper.GetCollection<User>();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginInputModel inputModel)
        {
            var usernameFilter = Builders<User>.Filter.Eq(x => x.Username, inputModel.Username);
            var isActiveFilter = Builders<User>.Filter.Eq(x => x.IsActive, true);

            var user = await _usersCollection
                .Find(usernameFilter & isActiveFilter)
                .Project(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Password = x.Password
                })
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return Unauthorized();
            }

            if (!BCrypt.Net.BCrypt.Verify(inputModel.Password, user.Password))
            {
                return Unauthorized();
            }

            string token = MD5Helper.CreateMD5("web token" + user.Id + DateTime.UtcNow);

            var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);

            var update = Builders<User>.Update
                .Set(x => x.WebToken, token)
                .Set(x => x.WebTokenIssuedAt, DateTime.UtcNow)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            await _usersCollection.UpdateOneAsync(filter, update);

            return Ok(new
            {
                Token = token,
                Name = user.Name,
            });
        }

        public class LoginInputModel
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }
    }
}
