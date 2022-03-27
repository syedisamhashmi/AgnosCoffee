using System.Collections.Generic;
using System.Threading.Tasks;
using AgnosCoffee.Data.Models.Deal;

namespace AgnosCoffee.Api.Interfaces.Repositories;

public interface IDealRepository
{
  Task<ICollection<DealDto>> GetAllDeals();
}