using System.Collections.Generic;
using System.Threading.Tasks;
using AgnosCoffee.Api.Interfaces.Repositories;
using AgnosCoffee.Data.Entities.Menu;
using MongoDB.Driver;

namespace AgnosCoffee.Api.Repositories;

public class MenuRepository : IMenuRepository
{
  private readonly DbContext context;

  public MenuRepository(DbContext context)
  {
    this.context = context;
  }
  public async Task<ICollection<MenuItem>> GetAllMenuItems()
  {
    List<MenuItem> items = await this.context.MenuItems
      .Find(_ => true)
      .ToListAsync();
    return items;
  }
}