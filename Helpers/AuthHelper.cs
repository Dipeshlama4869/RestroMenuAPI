namespace RestroMenu.Helpers
{
    public class AuthHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private const string UserKey = "AuthUser";

        public AuthHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void SetUser(User user)
        {
            if (_contextAccessor.HttpContext is null) { return; }

            _contextAccessor.HttpContext.Items[UserKey] = user;
        }

        public User? GetUser()
        {
            return (User?)_contextAccessor.HttpContext?.Items[UserKey];
        }

        public class User
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
        }
    }
}
