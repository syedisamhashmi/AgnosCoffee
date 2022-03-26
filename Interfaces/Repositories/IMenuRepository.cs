using System.Collections.Generic;
using System.Threading.Tasks;
using AgnosCoffee.Api.Models;

namespace AgnosCoffee.Api.Interfaces.Repositories;

public interface IMenuRepository
{
  Task<ICollection<MenuItem>> GetAllMenuItems();
}