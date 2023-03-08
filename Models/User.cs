namespace RestroMenu.Models
{
    public class User: BaseModel
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("is_active")]
        public bool IsActive { get; set; }

        [BsonElement("token")]
        public string Token { get; set; }

        [BsonElement("web_token")]
        public string WebToken { get; set; }

        [BsonElement("web_token_issued_at")]
        public DateTime? WebTokenIssuedAt { get; set; }

    }
}
