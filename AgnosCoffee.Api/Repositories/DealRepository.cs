using System.Collections.Generic;
using System.Threading.Tasks;

using AgnosCoffee.Api.Interfaces.Repositories;
using AgnosCoffee.Data.Models.Deal;

using MongoDB.Driver;

namespace AgnosCoffee.Api.Repositories;

public class DealRepository : IDealRepository
{
  private readonly DbContext context;

  public DealRepository(DbContext context)
  {
    this.context = context;
  }
  public async Task<ICollection<DealDto>> GetAllDeals()
  {
    List<DealDto> deals = await this.context.Deals
    .Find(_ => true)
    .Project(deal => new DealDto()
    {
      DealCode = deal.DealCode,
      DealDescription = deal.ToString()
    }).ToListAsync();
    return deals;
  }
}