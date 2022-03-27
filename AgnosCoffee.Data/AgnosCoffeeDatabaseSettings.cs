namespace AgnosCoffee.Data;

public class AgnosCoffeeDatabaseSettings
{
  public string ConnectionString { get; set; } = null!;
  public string DatabaseName { get; set; } = null!;
  public string MenuCollectionName { get; set; } = null!;
  public string DealsCollectionName { get; set; } = null!;
  public string OrdersCollectionName { get; set; } = null!;
}