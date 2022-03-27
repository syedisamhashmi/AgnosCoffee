using System;
using System.Collections.Generic;
using System.Linq;
using AgnosCoffee.Data.Entities.Order;
using AgnosCoffee.Data.Enums;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgnosCoffee.Data.Entities.Deal;
public sealed class DealEffects
{
  private decimal? _quantity = 0;
  private decimal _discountPercentage = 0;

  [BsonElement("discountPercentage")]
  public decimal DiscountPercentage
  {
    get { return this._discountPercentage.RoundToTwo(); }
    set { this._discountPercentage = value.RoundToTwo(); }
  }

  [BsonElement("appliesToItemType")]
  public ItemType? AppliesToItemType { get; set; } = null;

  [BsonElement("appliesToItemId")]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? AppliesToItemId { get; set; } = null;
  [BsonElement("appliesToItemName")]
  //? For printing purposes only
  public string? AppliesToItemName { get; set; } = null;

  [BsonElement("quantity")]
  public decimal? Quantity { get { return this._quantity?.RoundToTwo(); } set { this._quantity = value?.RoundToTwo(); } }
  [BsonElement("requireMin")]
  public decimal? RequireMin { get; set; } = 0;

  public override string ToString()
  {
    string desc = "";
    desc += "Get ";

    if (Quantity != null && Quantity.Value > 0)
    {
      desc += $"{Quantity.Value} ";
    }
    if (AppliesToItemType != null && AppliesToItemType != ItemType.None)
    {
      if (!string.IsNullOrWhiteSpace(AppliesToItemId))
      {
        // Shouldn't use both item type filtering and specific item filtering;
        throw new ApplicationException("You can not have both MustPurchaseItemType AND MustPurchaseItem on a deal.");
      }
      desc += $"{this.AppliesToItemType} ";
    }
    if (!string.IsNullOrWhiteSpace(AppliesToItemId))
    {
      if (AppliesToItemType != null && AppliesToItemType != ItemType.None)
      {
        // Shouldn't use both item type filtering and specific item filtering;
        throw new ApplicationException("You can not have both MustPurchaseItemType AND MustPurchaseItem on a deal.");
      }
      desc += $"{this.AppliesToItemName} ";
    }
    if (DiscountPercentage != 0)
    {
      desc += DiscountPercentage == 1 ? "free" : $"{DiscountPercentage * 100}% off";
    }
    return desc;
  }

  public List<ItemQuantity> ApplyToOrder(List<ItemQuantity> items)
  {
    ItemQuantity itemToApplyDiscount = null;
    var filteredByItem = AppliesToItemId != null;
    var filteredByGroup = AppliesToItemType != null && AppliesToItemType != ItemType.None;

    //? IFF effect applies to a specific item, that should take precedence.
    if (filteredByItem)
    {
      itemToApplyDiscount = items.Where(item => item.MenuItem.Id == AppliesToItemId).FirstOrDefault();
    }
    // apply effect to cheapest item in a specific item group 
    if (filteredByGroup)
    {
      // FREE_ENTREE_IF_TWO_ENTREE.
      // two of same type => one free
      // two diff ones => cheaper one is free.
      // likewise: FREE_BVG_IF_ONE_BURGER
      // same logic, cheapest drink.
      var itemGroups = items
        .Where(item => item.MenuItem.ItemType == AppliesToItemType)
        .OrderBy(item => item.MenuItem.Price) // Should sort by desc.
        .ToList();
      itemToApplyDiscount = itemGroups.FirstOrDefault();
    }

    if (filteredByItem || filteredByGroup)
    {
      //? Somehow, we are applying a deal to an order where the item does not exist. Therefore, do nothing.
      if (itemToApplyDiscount == null)
      {
        return items;
      }
      if (Quantity == null || Quantity.Value <= 0)
      {
        //? If no specified Quantity impacted, impact all the items.
        Quantity = itemToApplyDiscount.Quantity;
      }
      if (filteredByItem)
      {
        //? If buy-1-get-1, etc., the first one should not be the one being impacted, requiring a minimum of 2.
        if (RequireMin != null && itemToApplyDiscount.Quantity < RequireMin.Value.RoundToTwo())
        {
          return items;
        }
      }
      itemToApplyDiscount!.Quantity = CalculateDiscountedQuantity(itemToApplyDiscount);
      return items;
    }
    //? Doesn't apply to a specific item.
    //? Doesn't apply to a category.
    //? Impact the entire order then.
    if (DiscountPercentage != 0)
    {
      items.ForEach(item => { item.Quantity *= (1 - DiscountPercentage); });
    }
    return items;
  }
  private decimal CalculateDiscountedQuantity(ItemQuantity item)
  {
    //? Discounting the quantity is the same as discounting the amount!
    //? Makes calculations nice and simple
    //? 5items at %5 = $25
    //? 10% discount
    //? $4.50    * 5items = $22.5 = 4.5items * $5
    //?($5 * .9) * 5items = $22.5 = (5 * .9) * $5
    //? to handle free edge case -
    //? take the percentage times the quantity effected, and multiply
    //? by the overall quantity.
    //? total - (total * ((Discount% / total ) * discountQuantity) = newQuantity
    //? 5     - (5     * ((1         / 5     ) * 1               ) = 4 
    //? 5     - (5     * ((.1        / 5     ) * 1               ) = 4 
    return item.Quantity - (item.Quantity * ((DiscountPercentage / item.Quantity) * Quantity!.Value));
  }
}
