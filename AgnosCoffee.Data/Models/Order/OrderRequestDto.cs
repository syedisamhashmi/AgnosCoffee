using System.Collections.Generic;

namespace AgnosCoffee.Data.Models.Order;

public class OrderRequestDto
{
  public List<ItemQuantityDto>? OrderItems { get; set; }
  public string? DealCode { get; set; }
}
