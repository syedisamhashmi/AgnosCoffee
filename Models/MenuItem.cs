using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace AgnosCoffee.Api.Models;

public class MenuItem
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }

  [BsonElement("itemName")]
  public string? ItemName { get; set; }
}