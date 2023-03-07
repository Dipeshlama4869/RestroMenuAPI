namespace RestroMenu.Models
{
    public class TrnUser
    {
        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; }
    }
}
