using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace RestroMenu.Authentication
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {

    }
    public class CustomAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly AuthHelper _authHelper;

        public CustomAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            DbHelper dbHelper,
            AuthHelper authHelper)
            : base(options, logger, encoder, clock)
        {
            _usersCollection = dbHelper.GetCollection<User>();
            _authHelper = authHelper;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("X-Auth"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string token = Request.Headers["X-Auth"];
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            try
            {
                return await validateToken(token);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private async Task<AuthenticateResult> validateToken(string token)
        {
            var webTokenFilter = Builders<User>.Filter.Eq(x => x.WebToken, token);
            var isActiveFilter = Builders<User>.Filter.Eq(x => x.IsActive, true);

            var user = await _usersCollection
                .Find(webTokenFilter & isActiveFilter)
                .Project(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Username = x.Username,
                    WebTokenIssuedAt = x.WebTokenIssuedAt
                })
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            DateTime? expiryDateTime = user.WebTokenIssuedAt?.AddHours(8);
            if (DateTime.UtcNow > expiryDateTime)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            AuthHelper.User authUser = new AuthHelper.User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username,
            };

            _authHelper.SetUser(authUser);

            return AuthenticateResult.Success(ticket);
        }
    }
}
