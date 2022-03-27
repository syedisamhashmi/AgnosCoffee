using System;
using System.Threading.Tasks;
using AgnosCoffee.Api.Interfaces.Repositories;
using AgnosCoffee.Data.Models.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgnosCoffee.Api.Controllers;

[Route("api/[controller]")]
public class OrderController : ControllerBase
{
  private readonly IOrderRepository repository;
  private readonly ILogger<OrderController> logger;

  public OrderController(IOrderRepository repository, ILogger<OrderController> logger)
  {
    this.logger = logger;
    this.repository = repository;
  }

  [HttpPost]
  public async Task<IActionResult> PlaceOrder([FromBody] OrderRequestDto order)
  {
    try
    {
      var newOrder = await this.repository.PlaceOrder(order);
      return Ok(newOrder);
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  //! TODO(security): IF security was a part of this: users should have a 
  //! JWT bearer token with their userId and that should be checked
  //! to ensure a user only updates their order...
  [HttpPatch("{orderId}")]
  public async Task<IActionResult> UpdateOrder([FromRoute] string? orderId, [FromBody] OrderRequestDto order)
  {
    try
    {
      var updateRes = await this.repository.UpdateOrder(orderId, order);
      return Ok(updateRes);
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  [HttpPost("{orderId}/pay")]
  public async Task<IActionResult> PayOrder([FromRoute] string? orderId, [FromBody] PaymentDetailsDto paymentDetails)
  {
    var paymentRes = await this.repository.PayOrder(orderId, paymentDetails);
    return Ok(paymentRes);
  }
}
