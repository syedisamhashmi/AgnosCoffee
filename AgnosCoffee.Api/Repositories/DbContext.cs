using AgnosCoffee.Data;
using AgnosCoffee.Data.Entities.Deal;
using AgnosCoffee.Data.Entities.Menu;
using AgnosCoffee.Data.Entities.Order;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace AgnosCoffee.Api.Repositories;

public class DbContext
{
  public readonly IMongoCollection<MenuItem> MenuItems;
  public readonly IMongoCollection<Deal> Deals;
  public readonly IMongoCollection<Order> Orders;
  private readonly MongoClient mongoClient;
  private readonly IMongoDatabase mongoDatabase;

  public DbContext(IOptions<AgnosCoffeeDatabaseSettings> agnosCoffeeDatabaseSettings)
  {
    this.mongoClient = new MongoClient(
        agnosCoffeeDatabaseSettings.Value.ConnectionString);
    this.mongoDatabase = mongoClient.GetDatabase(
        agnosCoffeeDatabaseSettings.Value.DatabaseName);
    this.MenuItems = mongoDatabase.GetCollection<MenuItem>(
        agnosCoffeeDatabaseSettings.Value.MenuCollectionName);
    this.Deals = mongoDatabase.GetCollection<Deal>(
        agnosCoffeeDatabaseSettings.Value.DealsCollectionName);
    this.Orders = mongoDatabase.GetCollection<Order>(
        agnosCoffeeDatabaseSettings.Value.OrdersCollectionName);
  }
}