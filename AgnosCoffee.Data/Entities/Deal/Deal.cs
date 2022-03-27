using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgnosCoffee.Data.Entities.Deal;

public class Deal
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }
  [BsonElement("constraints")]
  public DealConstraints? Constraints { get; set; }
  [BsonElement("effects")]
  public DealEffects? Effects { get; set; }
  [BsonElement("dealCode")]
  public string? DealCode { get; set; }

  public override string ToString()
  {
    return $"{this.Constraints?.ToString() + ", " ?? ""}{this.Effects?.ToString() ?? ""}";
  }
}
