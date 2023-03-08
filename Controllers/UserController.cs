using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;
using System.Data;

namespace RestroMenu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly DbHelper _dbHelper;

        public UserController(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
            _usersCollection = _dbHelper.GetCollection<User>();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _dbHelper.FindByIdAsync(
                id,
                x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Username = x.Username
                },
                Builders<User>.Filter.Empty
            ) ;

            if (user is null)
            {
                return ErrorHelper.ErrorResult("Id", "Id is invalid.");
            }

            return Ok(new
            {
                User = user
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddInputModel input)
        {
            User user = new User
            {
                Name = input.Name,
                Username = input.Username,
                Email = StrHelper.NullIfEmpty(input.Email),
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
                IsActive = true
            };

            await _usersCollection.InsertOneAsync(user);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UpdateInputModel input)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);

            var update = Builders<User>.Update
                .Set(x => x.Name, input.Name)
                .Set(x => x.Email, StrHelper.NullIfEmpty(input.Email))
                .Set(x => x.Username, input.Username)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            if (!string.IsNullOrEmpty(input.Password))
            {
                update = update.Set(x => x.Password, BCrypt.Net.BCrypt.HashPassword(input.Password));
            }

            await _usersCollection.UpdateOneAsync(filter, update);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await _dbHelper.IdExistsAsync(id, Builders<User>.Filter.Empty))
            {
                return ErrorHelper.ErrorResult("Id", "Id is invalid.");
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, id);

            await _usersCollection.DeleteOneAsync(filter);

            return Ok();
        }


        public class BaseInputModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string PasswordConfirmation { get; set; }
        }

        public class AddInputModel : BaseInputModel { }

        public class UpdateInputModel : BaseInputModel { }
    }
}
