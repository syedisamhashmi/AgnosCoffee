using System.Collections.Generic;
using System.Threading.Tasks;
using AgnosCoffee.Api.Interfaces.Repositories;
using AgnosCoffee.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgnosCoffee.Api.Controllers;

[Route("api/[controller]")]
public class MenuController : ControllerBase
{
  private readonly IMenuRepository repository;
  private readonly ILogger<MenuController> logger;

  public MenuController(IMenuRepository repository, ILogger<MenuController> logger)
  {
    this.logger = logger;
    this.repository = repository;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    ICollection<MenuItem> menuItems = await this.repository.GetAllMenuItems();
    return Ok(menuItems);
  }
}
