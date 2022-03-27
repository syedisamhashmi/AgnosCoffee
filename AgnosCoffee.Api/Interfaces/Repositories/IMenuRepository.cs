using System.Collections.Generic;
using System.Threading.Tasks;
using AgnosCoffee.Data.Entities.Menu;

namespace AgnosCoffee.Api.Interfaces.Repositories;

public interface IMenuRepository
{
  Task<ICollection<MenuItem>> GetAllMenuItems();
}