using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgnosCoffee.Data;
using AgnosCoffee.Data.Entities.Menu;
using AgnosCoffee.Data.Entities.Order;
using AgnosCoffee.Data.Enums;
using AgnosCoffee.Data.Models.Order;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace AgnosCoffee.Api.Repositories.Tests
{
  [TestFixture]
  public class RepositoryTests
  {
    const string BANANA_ID = "000000000000000000000001";
    const string MUFFIN_ID = "000000000000000000000002";
    const string SMALL_BVG_ID = "000000000000000000000003";
    const string MED_BVG_ID = "000000000000000000000004";
    const string LARGE_BVG_ID = "000000000000000000000005";
    const string HAMBURGER_ID = "000000000000000000000006";
    const string SMALL_PIZZA_ID = "000000000000000000000007";
    const string MED_PIZZA_ID = "000000000000000000000008";
    const string LARGE_PIZZA_ID = "000000000000000000000009";
    const string XXL_PIZZA_ID = "00000000000000000000000a";
    const string SMALL_FRIES_ID = "00000000000000000000000b";
    const string LARGE_FRIES_ID = "00000000000000000000000c";
    const string ONION_RINGS_ID = "00000000000000000000000d";
    const string CHOCO_CAKE_ID = "00000000000000000000000e";
    const string CANNOLI_ID = "00000000000000000000000f";

    private MenuRepository menuRepository;
    private DealRepository dealRepository;
    private OrderRepository orderRepository;
    static Dictionary<string, string> IdToName = new Dictionary<string, string>();
    static Dictionary<string, ItemType> IdToType = new Dictionary<string, ItemType>();
    static Dictionary<string, decimal> IdToPrice = new Dictionary<string, decimal>();
    static Dictionary<string, decimal> IdToTaxrate = new Dictionary<string, decimal>();

    public void AddToDicts(string Id, string name, ItemType type, decimal price, decimal tax)
    {
      IdToName.TryAdd(Id, name);
      IdToType.TryAdd(Id, type);
      IdToPrice.TryAdd(Id, price);
      IdToTaxrate.TryAdd(Id, tax);
    }

    [SetUp]
    public void SetUp()
    {
      AddToDicts(BANANA_ID, "Banana", ItemType.Appetizer, 0.5m, 0.0m);
      AddToDicts(MUFFIN_ID, "Muffin", ItemType.Appetizer, 1.0m, 0.1m);
      AddToDicts(SMALL_BVG_ID, "Small Carbonated Beverage", ItemType.Beverage, 1.5m, 0.0825m);
      AddToDicts(MED_BVG_ID, "Medium Carbonated Beverage", ItemType.Beverage, 2.5m, 0.12m);
      AddToDicts(LARGE_BVG_ID, "Large Carbonated Beverage", ItemType.Beverage, 2.6m, 0.11m);
      AddToDicts(HAMBURGER_ID, "Hamburger", ItemType.Entree, 3.99m, 0.0825m);
      AddToDicts(SMALL_PIZZA_ID, "Small Pizza", ItemType.Entree, 6.99m, 0.0825m);
      AddToDicts(MED_PIZZA_ID, "Medium Pizza", ItemType.Entree, 8.99m, 0.0825m);
      AddToDicts(LARGE_PIZZA_ID, "Large Pizza", ItemType.Entree, 15.99m, 0.1m);
      AddToDicts(XXL_PIZZA_ID, "X-X-Large Pizza", ItemType.Entree, 49.99m, 0.3m);
      AddToDicts(SMALL_FRIES_ID, "Small Fries", ItemType.Entree, 2.99m, 0.15m);
      AddToDicts(LARGE_FRIES_ID, "Large Fries", ItemType.Side, 5.99m, 0.2m);
      AddToDicts(ONION_RINGS_ID, "Onion Rings", ItemType.Side, 8.99m, 0.25m);
      AddToDicts(CHOCO_CAKE_ID, "Chocolate Cake", ItemType.Desert, 9.99m, 0.025m);
      AddToDicts(CANNOLI_ID, "Cannoli", ItemType.Desert, 1.99m, 0.1m);

      var services = new ServiceCollection();
      services.AddSingleton<DbContext>();

      IOptions<AgnosCoffeeDatabaseSettings> dbOpts = Options.Create<AgnosCoffeeDatabaseSettings>(
        new AgnosCoffeeDatabaseSettings()
        {
          ConnectionString = "mongodb://localhost/?directConnection=true&serverSelectionTimeoutMS=2000",
          DatabaseName = "AgnosCoffee",
          MenuCollectionName = "Menu",
          DealsCollectionName = "Deals",
          OrdersCollectionName = "Orders"
        }
      );

      services.AddSingleton<IOptions<AgnosCoffeeDatabaseSettings>>(
        dbOpts
      );
      services.AddSingleton<DbContext>();
      services.AddScoped<MenuRepository, MenuRepository>();
      services.AddScoped<DealRepository, DealRepository>();
      services.AddScoped<OrderRepository, OrderRepository>();
      var provider = services.BuildServiceProvider();
      this.menuRepository = provider.GetRequiredService<MenuRepository>();
      this.dealRepository = provider.GetRequiredService<DealRepository>();
      this.orderRepository = provider.GetRequiredService<OrderRepository>();

    }

    [Test]
    public async Task GetInvalidDealTest()
    {
      try
      {
        await this.orderRepository.GetDealByCode("1234567890", true);
      }
      catch (Exception)
      {
        Assert.Pass("PASS - Expected deal missing exception!");
        return;
      }
      Assert.Fail("Expected deal missing exception!");
    }

    public static ItemQuantity MenuItem(string id, decimal quantity)
    {
      return new ItemQuantity()
      {
        MenuItem = new MenuItem()
        {
          Id = id,
          ItemName = IdToName.GetValueOrDefault(id),
          ItemType = IdToType.GetValueOrDefault(id),
          Price = IdToPrice.GetValueOrDefault(id),
          TaxRate = IdToTaxrate.GetValueOrDefault(id)
        },
        Quantity = quantity
      };
    }
    public static ItemQuantityDto MenuItemDto(string id, decimal quantity)
    {
      return new ItemQuantityDto()
      {
        ItemId = id,
        Quantity = (int)quantity
      };
    }

    [Test]
    [TestCase(new string[] { CANNOLI_ID }, new int[] { 2 }, "Cannoli4Cannoli", ExpectedResult = 2.19)]
    [TestCase(new string[] { CANNOLI_ID }, new int[] { 2 }, "HalfACannoli", ExpectedResult = 3.28)]
    [TestCase(new string[] { CANNOLI_ID }, new int[] { 100 }, "SweetTimes", ExpectedResult = 0)]
    [TestCase(new string[] { CANNOLI_ID }, new int[] { 2 }, "10OffCannoli", ExpectedResult = 3.94)]
    [TestCase(new string[] { CANNOLI_ID, LARGE_PIZZA_ID }, new int[] { 2, 2 }, "HaveSomeCanoliWithThat", ExpectedResult = 37.37)]
    [TestCase(new string[] { XXL_PIZZA_ID, ONION_RINGS_ID, CHOCO_CAKE_ID }, new int[] { 2, 10, 6 }, "HungryPersonSpecial", ExpectedResult = 151.89)]
    [TestCase(new string[] { HAMBURGER_ID, LARGE_BVG_ID, SMALL_BVG_ID }, new int[] { 2, 1, 1 }, "ThirstyFromThatBurger", ExpectedResult = 11.52)]
    [TestCase(new string[] { HAMBURGER_ID, SMALL_PIZZA_ID }, new int[] { 4, 3 }, "TwoFer", ExpectedResult = 35.66)]
    public async Task<decimal> TestTotalCalculation(string[] ids, int[] quantities, string dealCode)
    {
      List<ItemQuantity> items = new List<ItemQuantity>();
      for (int i = 0; i < ids.Length; i++)
      {
        var id = ids[i];
        var amount = quantities[i];
        items.Add(MenuItem(id, new decimal(amount)));
      }
      try
      {
        var deal = await this.orderRepository.GetDealByCode(dealCode);
        return this.orderRepository.CalculateGrandTotalFromItemsAndDeal(items, deal);
      }
      catch (Exception)
      {
        Assert.Fail("Expected order total wrong!");
        return -1;
      }
    }


    [Test]
    [TestCase(new string[] { BANANA_ID, BANANA_ID, MUFFIN_ID, MED_BVG_ID }, new int[] { 1, 2, 1, 1 }, null, ExpectedResult = 1)]
    [TestCase(new string[] { BANANA_ID, MUFFIN_ID, MED_BVG_ID }, new int[] { 1, 0, -5 }, null, ExpectedResult = 1)]
    [TestCase(new string[] { BANANA_ID, "abcdef000000000000000001", MED_BVG_ID }, new int[] { 1, 1, -5 }, null, ExpectedResult = 1)]
    [TestCase(new string[] { BANANA_ID, "abcdef000000000000000001", MED_BVG_ID }, new int[] { 1, 1, -5 }, "1234567890", ExpectedResult = 1)]
    public async Task<decimal> PostOrderFailTests(string[] ids, int[] quantities, string dealCode)
    {
      List<ItemQuantityDto> items = new List<ItemQuantityDto>();
      for (int i = 0; i < ids.Length; i++)
      {
        var id = ids[i];
        var amount = quantities[i];
        items.Add(MenuItemDto(id, new decimal(amount)));
      }
      try
      {
        OrderRequestDto req = new OrderRequestDto()
        {
          OrderItems = items,
          DealCode = dealCode
        };
        var placed = await this.orderRepository.PlaceOrder(req);
      }
      catch (Exception)
      {
        Assert.Pass("PASS - Expected order total wrong!");
      }
      Assert.Fail("Expected order total wrong!");
      return -1;
    }

  }
}