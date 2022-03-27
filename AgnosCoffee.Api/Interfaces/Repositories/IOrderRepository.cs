using System.Threading.Tasks;
using AgnosCoffee.Data.Models.Order;

namespace AgnosCoffee.Api.Interfaces.Repositories;

public interface IOrderRepository
{
  Task<OrderPlacedDto> PlaceOrder(OrderRequestDto order);
  Task<OrderPlacedDto> UpdateOrder(string? orderId, OrderRequestDto updated);
  Task<RecieptDto> PayOrder(string? orderId, PaymentDetailsDto paymentDetails);
}