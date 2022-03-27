using System.Collections.Generic;
using System.Threading.Tasks;
using AgnosCoffee.Api.Interfaces.Repositories;
using AgnosCoffee.Data.Models.Deal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgnosCoffee.Api.Controllers;

[Route("api/[controller]")]
public class DealController : ControllerBase
{
  private readonly IDealRepository repository;
  private readonly ILogger<DealController> logger;

  public DealController(IDealRepository repository, ILogger<DealController> logger)
  {
    this.logger = logger;
    this.repository = repository;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    ICollection<DealDto> deals = await this.repository.GetAllDeals();
    return Ok(deals);
  }
}
