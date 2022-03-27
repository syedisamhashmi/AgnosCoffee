using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgnosCoffee.Data.Entities.Order;
public class Order
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }
  [BsonElement("items")]
  public List<ItemQuantity>? Items { get; set; }
  public string? DealCode { get; set; }
}
