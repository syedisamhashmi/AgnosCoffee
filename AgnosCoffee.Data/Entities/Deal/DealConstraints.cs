using System;
using System.Collections.Generic;
using System.Linq;
using AgnosCoffee.Data.Entities.Order;
using AgnosCoffee.Data.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgnosCoffee.Data.Entities.Deal;

public sealed class DealConstraints
{
  [BsonElement("mustPurchaseQuantity")]
  public int? MustPurchaseQuantity { get; set; } = -1;
  /// A specific item type that must be purchased in order to fill the constraints.
  [BsonElement("mustPurchaseItemType")]
  public ItemType? MustPurchaseItemType { get; set; } = null;
  /// A specific item that must be purchased in order to fill the constraints.
  [BsonElement("mustPurchaseItem")]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? MustPurchaseItem { get; set; } = null;
  [BsonElement("mustPurchaseItemName")]
  //! For printing purpose ONLY.
  public string? MustPurchaseItemName { get; set; } = null;

  public override string ToString()
  {
    string desc = "";
    if (MustPurchaseQuantity != null && MustPurchaseQuantity > 0)
    {
      desc += $"Buy {MustPurchaseQuantity} ";
    }
    if (MustPurchaseItemType != null && MustPurchaseItemType != ItemType.None)
    {
      if (!string.IsNullOrWhiteSpace(MustPurchaseItem))
      {
        // Shouldn't use both item type filtering and specific item filtering;
        throw new ApplicationException("You can not have both MustPurchaseItemType AND MustPurchaseItem on a deal.");
      }
      desc += $"{this.MustPurchaseItemType}";
    }
    if (!string.IsNullOrWhiteSpace(MustPurchaseItem))
    {
      if (MustPurchaseItemType != null && MustPurchaseItemType != ItemType.None)
      {
        // Shouldn't use both item type filtering and specific item filtering;
        throw new ApplicationException("You can not have both MustPurchaseItemType AND MustPurchaseItem on a deal.");
      }
      desc += $"{this.MustPurchaseItemName}";
    }
    return desc;
  }
  public bool SatisfiedByOrder(List<ItemQuantity> items)
  {
    //? IFF deal requires a specific item, that will take precedense.
    if (MustPurchaseItem != null)
    {
      return CheckItemCriteriaMet(items, (item) => item.MenuItem.Id == MustPurchaseItem);
    }
    //? IFF deal requires a specific type of item
    if (MustPurchaseItemType != null && MustPurchaseItemType != ItemType.None)
    {
      return CheckItemCriteriaMet(items, (item) => item.MenuItem.ItemType == MustPurchaseItemType);
    }
    //? No type required, no name required...
    //? Satisfies the conditions.
    //? Also, failing open is a good practice.
    return true;
  }
  private bool CheckItemCriteriaMet(ICollection<ItemQuantity> items, Func<ItemQuantity, bool> filterFunc)
  {
    var specificItem = items.Where(filterFunc).FirstOrDefault();
    if (specificItem is null)
    {
      //? Item not found in cart. Deal constraints not met.
      return false;
    }
    return CheckItemQuantity(specificItem);
  }
  private bool CheckItemQuantity(ItemQuantity validItem)
  {
    //? IFF quantity is not specified or less than 0 (by default)
    //? then, require at least one
    if (MustPurchaseQuantity != null &&
        MustPurchaseQuantity < 0 &&
        validItem.Quantity >= 1)
    {
      //? User purchased enough of the specific item.
      //? Deal constraints are satisfied.
      return true;
    }
    //? Quantity specified, they have to buy at least this.MustPurchaseQuantity amount
    if (validItem.Quantity >= this.MustPurchaseQuantity)
    {
      //? User purchased enough of the specific item.
      //? Deal constraints are satisfied.
      return true;
    }
    //? Item quantity not met in either situation.
    return false;
  }
}

