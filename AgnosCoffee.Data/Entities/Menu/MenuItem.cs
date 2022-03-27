using AgnosCoffee.Data.Enums;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgnosCoffee.Data.Entities.Menu;

public class MenuItem
{
  private decimal _price = 0;

  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }
  [BsonElement("itemName")]
  public string? ItemName { get; set; }
  [BsonElement("itemType")]
  public ItemType? ItemType { get; set; }
  [BsonElement("price")]
  public decimal Price { get { return this._price.RoundToTwo(); } set { this._price = value.RoundToTwo(); } }
  [BsonElement("taxRate")]
  public decimal TaxRate { get; set; }
}