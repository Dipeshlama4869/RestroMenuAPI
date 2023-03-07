namespace RestroMenu.Models
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }
    }
}
