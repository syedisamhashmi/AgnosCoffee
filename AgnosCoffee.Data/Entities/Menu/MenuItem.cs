using AgnosCoffee.Data.Enums;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgnosCoffee.Data.Entities.Menu;

public class MenuItem
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }
  [BsonElement("itemName")]
  public string? ItemName { get; set; }
  [BsonElement("itemType")]
  public ItemType? ItemType { get; set; }
  [BsonElement("price")]
  public decimal Price { get; set; }
  [BsonElement("taxRate")]
  public decimal TaxRate { get; set; }
}