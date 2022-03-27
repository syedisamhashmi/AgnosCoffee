using System.Collections.Generic;
using System.Threading.Tasks;
using AgnosCoffee.Api.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AgnosCoffee.Data.Entities.Menu;

namespace AgnosCoffee.Api.Controllers;

[Route("api/[controller]")]
public class MenuController : ControllerBase
{
  private readonly IMenuRepository repository;
  public MenuController(IMenuRepository repository)
  {
    this.repository = repository;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    ICollection<MenuItem> menuItems = await this.repository.GetAllMenuItems();
    return Ok(menuItems);
  }
}
