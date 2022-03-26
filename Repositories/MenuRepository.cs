using System.Collections.Generic;
using System.Threading.Tasks;
using AgnosCoffee.Api.Interfaces.Repositories;
using AgnosCoffee.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MenuRepository : IMenuRepository
{

  private readonly IMongoCollection<MenuItem> menuItems;

  public MenuRepository(IOptions<AgnosCoffeeDatabaseSettings> agnosCoffeeDatabaseSettings)
  {
    var mongoClient = new MongoClient(
        agnosCoffeeDatabaseSettings.Value.ConnectionString);

    var mongoDatabase = mongoClient.GetDatabase(
        agnosCoffeeDatabaseSettings.Value.DatabaseName);

    menuItems = mongoDatabase.GetCollection<MenuItem>(
        agnosCoffeeDatabaseSettings.Value.MenuCollectionName);
  }
  public async Task<ICollection<MenuItem>> GetAllMenuItems()
  {
    List<MenuItem> items = await this.menuItems.Find(_ => true).ToListAsync();
    return items;
  }
}