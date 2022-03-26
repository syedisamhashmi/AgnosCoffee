namespace AgnosCoffee.Api.Models;

public class AgnosCoffeeDatabaseSettings
{
  public string ConnectionString { get; set; } = null!;

  public string DatabaseName { get; set; } = null!;

  public string MenuCollectionName { get; set; } = null!;
}