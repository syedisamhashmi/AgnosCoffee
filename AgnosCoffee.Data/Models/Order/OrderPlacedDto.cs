namespace AgnosCoffee.Data.Models.Order;

public class OrderPlacedDto
{
  public string? OrderId { get; set; }
  public decimal OrderTotal { get; set; }
  public decimal OrderSubTotal { get; set; }
  public decimal GrandTotal { get; set; }
}
