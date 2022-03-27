using AgnosCoffee.Data.Entities.Menu;

namespace AgnosCoffee.Data.Entities.Order;

public class ItemQuantity
{
  private decimal _quantity = 0;
  public MenuItem MenuItem { get; set; } = null!;
  public decimal Quantity { get { return _quantity.RoundToTwo(); } set { this._quantity = value.RoundToTwo(); } }
}
