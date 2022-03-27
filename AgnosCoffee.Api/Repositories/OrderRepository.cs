using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AgnosCoffee.Api.Interfaces.Repositories;
using AgnosCoffee.Data;
using AgnosCoffee.Data.Entities.Deal;
using AgnosCoffee.Data.Entities.Order;
using AgnosCoffee.Data.Models.Order;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AgnosCoffee.Api.Repositories;

public class OrderRepository : IOrderRepository
{
  private readonly DbContext context;

  public OrderRepository(DbContext context)
  {
    this.context = context;
  }

  public async Task<OrderPlacedDto> PlaceOrder(OrderRequestDto order)
  {
    Order newOrder = new Order();

    var generatedOrderAndDeal = await GenerateOrderFromRequest(order);
    newOrder = generatedOrderAndDeal.newOrder;
    await this.context.Orders.InsertOneAsync(newOrder);

    return await GetOrderTotals(newOrder, generatedOrderAndDeal.deal);
  }

  private async Task<(Order newOrder, Deal? deal)> GenerateOrderFromRequest(OrderRequestDto order, string? existingId = null)
  {
    Order newOrder = new Order();
    newOrder.Items = await VerifyOrderItems(order);
    Deal? orderDeal = await GetDealByCode(order.DealCode, shouldNullCheck: false);
    newOrder.DealCode = orderDeal?.DealCode;
    newOrder.Id = existingId ?? ObjectId.GenerateNewId().ToString();
    return (newOrder, orderDeal);
  }
  private async Task<OrderPlacedDto> GetOrderTotals(Order order, Deal? deal)
  {
    var orderTotals = await CalculateOrderTotal(order);
    return new OrderPlacedDto()
    {
      OrderId = order.Id!.ToString(),
      OrderTotal = orderTotals.Total.RoundToTwo(),
      OrderSubTotal = orderTotals.Subtotal.RoundToTwo(),
      GrandTotal = CalculateGrandTotalFromItemsAndDeal(order.Items!, deal).RoundToTwo(),
    };
  }

  private async Task<List<ItemQuantity>> VerifyOrderItems(OrderRequestDto order)
  {
    if (order is null || order.OrderItems is null)
    {
      throw new ArgumentNullException("Order or order items missing");
    }
    if (order.OrderItems.Count != order.OrderItems.DistinctBy(item => item.ItemId).Count())
    {
      throw new ArgumentException("Duplicate menu item id found");
    }
    //TODO cache this?
    var allMenuItemIds = await this.context.MenuItems
      .Find(_ => true)
      .Project(item => item.Id)
      .ToListAsync();

    var badItems = order.OrderItems.Where(item =>
      !allMenuItemIds.Contains(item.ItemId) || //? Handle invalid menu item
      item.Quantity < 0 //? Handle negative quantity.
    ).ToList();
    //? Any bad items? No-go!
    if (badItems.Any())
    {
      string itemDesc = "";
      badItems.ForEach(item => itemDesc += $"\nId: {item.ItemId}, Quantity: {item.Quantity}\n");
      throw new NotSupportedException($"Bad Menu Item(s) selected: {itemDesc}");
    }
    var itemsFromOrder = order.OrderItems.Select(item => item.ItemId).ToList();
    var dbItemsOrdered = await this.context.MenuItems
      .Find(item => itemsFromOrder.Contains(item.Id))
      .ToListAsync();
    List<ItemQuantity> newOrderItems = new List<ItemQuantity>();
    dbItemsOrdered.ForEach(
      dbItem =>
      {
        ItemQuantity orderItemQuantity = new ItemQuantity()
        {
          MenuItem = dbItem, //? Save item from DB in order, to prevent price change from impacting user :)
          Quantity = order.OrderItems
            .Where(item => item.ItemId == dbItem.Id)
            .FirstOrDefault()?.Quantity
            ?? throw new Exception($"Item {dbItem.Id} not found in order"),
        };
        orderItemQuantity.Quantity = orderItemQuantity.Quantity;
        newOrderItems.Add(orderItemQuantity);
      }
    );
    return newOrderItems;
  }
  public async Task<OrderPlacedDto> UpdateOrder(string? orderId, OrderRequestDto updated)
  {
    if (updated is null || orderId is null)
    {
      throw new ArgumentException("Update request or OrderId missing.");
    }

    Order existing = (await GetOrderById(orderId))!;
    var orderAndDeal = await GenerateOrderFromRequest(updated, orderId);

    //? I WAS going to do an update, but time is thin, so.... Replace the entire document!
    await this.context.Orders.ReplaceOneAsync(
      (order) => order.Id == existing.Id,
      orderAndDeal.newOrder
    );
    return await GetOrderTotals(orderAndDeal.newOrder, orderAndDeal.deal);
  }

  public async Task<RecieptDto> PayOrder(string? orderId, PaymentDetailsDto paymentDetails)
  {
    if (orderId is null) //TODO I `was` going to use `orderId!!` in the param def, unsure who uses preview langVersion, though :(
    {
      throw new ArgumentNullException("OrderId not specified!");
    }
    Order order = (await GetOrderById(orderId))!;
    if (order.Items is null)
    {
      return new RecieptDto()
      {
        Remarks = "Payment request recieved, but no items in order to pay for."
      };
    }
    Deal? deal = await GetDealByCode(order.DealCode);
    var grandTotal = CalculateGrandTotalFromItemsAndDeal(order.Items, deal).RoundToTwo();
    var reciept = new RecieptDto();
    if (grandTotal >= 0)
    {
      if (paymentDetails.Amount is null)
      {
        reciept.Remarks = $"Please provide a payment amount!";
        return reciept;
      }
      if (paymentDetails.Amount.Value.RoundToTwo() < grandTotal.RoundToTwo())
      {
        reciept.Remarks = $"The order has a grand total of {grandTotal.RoundToTwo()}, you supplied {paymentDetails.Amount.Value.RoundToTwo()}. Provide more funds.";
        return reciept;
      }
    }
    //? Otherwise, amount is either equal or more.
    Thread.Sleep(5000); //Making order!!!

    reciept.OrderItems = "";
    order.Items.ForEach(
      (item) =>
      {

        reciept.OrderItems += $"\nEnjoy your {item.Quantity} {item.MenuItem.ItemName}(s)";
      }
    );
    if (paymentDetails.Amount?.RoundToTwo() > grandTotal.RoundToTwo())
    {
      reciept.Remarks =
      $"The grand total was {grandTotal.RoundToTwo()}, you provided {paymentDetails.Amount?.RoundToTwo()}. Your change is: {(paymentDetails.Amount?.RoundToTwo() - grandTotal.RoundToTwo())?.RoundToTwo()}";
    }

    //? REMOVE ORDER FROM DB. ITS BEEN PAID.
    await this.context.Orders.DeleteOneAsync(rec => rec.Id == orderId);
    return reciept;
  }

  #region Order Utils not exposed to users.
  private decimal CalculateSubTotalFromItems(List<ItemQuantity> items)
  {
    //? The quantity of the item 
    //? multiplied by its price,
    //? WITHOUT tax, is the subtotal.
    return items.Sum(item => (item.Quantity.RoundToTwo() * item.MenuItem.Price.RoundToTwo())).RoundToTwo();
  }
  private decimal CalculateTotalFromItems(List<ItemQuantity> items)
  {
    //? The quantity of the item 
    //? multiplied by its price,
    //? multiplied by the tax rate, is the total.
    return items.Sum(item => (item.Quantity.RoundToTwo() * (item.MenuItem.Price.RoundToTwo() * (item.MenuItem.TaxRate + 1)))).RoundToTwo();
  }
  private decimal CalculateGrandTotalFromItemsAndDeal(List<ItemQuantity> items, Deal? deal)
  {
    var linesToUse = items;
    //? If Deal exists and has constraints and effects
    if (deal != null && deal.Constraints != null && deal.Effects != null)
    {
      //? and items in order satisfy deal
      if (deal.Constraints.SatisfiedByOrder(linesToUse))
      {
        //? Apply deal to order.
        linesToUse = deal.Effects.ApplyToOrder(linesToUse);
      }
    }
    //? The quantity of the item (discounted!)
    //? multiplied by its price,
    //? multiplied by the tax rate, is the total.
    return CalculateTotalFromItems(linesToUse).RoundToTwo();
  }
  /// <summary>
  /// Calculates the subtotal and total on an order.
  /// </summary>
  /// <param name="Subtotal"></param>
  /// <param name="newOrder"></param>
  /// <returns></returns>
  private async Task<(decimal Subtotal, decimal Total)> CalculateOrderTotal(Order order)
  {
    Deal? deal = null;
    if (!string.IsNullOrWhiteSpace(order.DealCode))
    {
      deal = await this.context.Deals
        .Find(deal => deal.DealCode == order.DealCode)
        .FirstOrDefaultAsync();
      if (deal is null)
      {
        throw new NullReferenceException($"Deal {order.DealCode} not found!");
      }
    }
    if (order.Items is null || !order.Items.Any())
    {
      throw new NullReferenceException($"Order has no items!");
    }

    return
    (
      CalculateSubTotalFromItems(order.Items),
      CalculateTotalFromItems(order.Items)
    );
  }
  /// <summary>
  /// Get an order by ID
  /// </summary>
  /// <param name="orderId">The order's ID</param>
  /// <param name="shouldNullCheck">Whether or not to throw an exception if the order is not found.</param>
  /// <returns>The first order with the specified Id</returns>
  private async Task<Order?> GetOrderById(string? orderId, bool shouldNullCheck = true)
  {
    Order? existing = await this.context.Orders.Find(
      order => order.Id == orderId
    ).FirstOrDefaultAsync();
    if (shouldNullCheck && existing is null)
    {
      throw new Exception($"Order {orderId} missing!");
    }
    return existing;
  }
  /// <summary>
  /// Get a deal by its friendly code-name
  /// </summary>
  /// <param name="dealCode">The deal's code</param>
  /// <param name="shouldNullCheck">Whether or not to throw an exception if the deal is not found.</param>
  /// <returns>The first deal with the specified code</returns>
  private async Task<Deal?> GetDealByCode(string? dealCode, bool shouldNullCheck = true)
  {
    if (string.IsNullOrWhiteSpace(dealCode))
    {
      return null;
    }
    Deal? existing = await this.context.Deals.Find(
      order => order.DealCode == dealCode
    ).FirstOrDefaultAsync();
    if (shouldNullCheck && existing is null)
    {
      throw new Exception($"Deal {dealCode} missing!");
    }
    return existing;
  }
  #endregion
}