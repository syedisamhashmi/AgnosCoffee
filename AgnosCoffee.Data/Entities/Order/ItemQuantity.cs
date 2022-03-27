using AgnosCoffee.Data.Entities.Menu;

namespace AgnosCoffee.Data.Entities.Order;

public class ItemQuantity
{
  public MenuItem MenuItem { get; set; } = null!;
  public decimal Quantity { get; set; } = 0;
}
