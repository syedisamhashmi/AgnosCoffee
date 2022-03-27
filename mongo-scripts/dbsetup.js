//#region Definitions
// All tables to be cleared and populated.
var tableNames = ["Menu", "Deals", "Orders"];

// Item Types
const APPETIZER = 1;
const BEVERAGE = 2;
const ENTREE = 3;
const SIDE = 4;
const DESERT = 5;
// Item IDs - Hardcoded for relational queries, such as joining.
//TODO if time permits, figure out how to write a foreign key check in Mongo and implement that here.
const BANANA_ID = new ObjectId("000000000000000000000001");
const MUFFIN_ID = new ObjectId("000000000000000000000002");
const SMALL_BVG_ID = new ObjectId("000000000000000000000003");
const MED_BVG_ID = new ObjectId("000000000000000000000004");
const LARGE_BVG_ID = new ObjectId("000000000000000000000005");
const HAMBURGER_ID = new ObjectId("000000000000000000000006");
const SMALL_PIZZA_ID = new ObjectId("000000000000000000000007");
const MED_PIZZA_ID = new ObjectId("000000000000000000000008");
const LARGE_PIZZA_ID = new ObjectId("000000000000000000000009");
const XXL_PIZZA_ID = new ObjectId("00000000000000000000000A");
const SMALL_FRIES_ID = new ObjectId("00000000000000000000000B");
const LARGE_FRIES_ID = new ObjectId("00000000000000000000000C");
const ONION_RINGS_ID = new ObjectId("00000000000000000000000D");
const CHOCO_CAKE_ID = new ObjectId("00000000000000000000000E");
const CANNOLI_ID = new ObjectId("00000000000000000000000F");

// - DEAL Ids
const NONE = new ObjectId("000000000000000000000001");
const FREE_BVG_IF_ONE_BURGER = new ObjectId("000000000000000000000002");
const FREE_ENTREE_IF_TWO_ENTREE = new ObjectId("000000000000000000000003");
const HALF_OFF_IF_ONE_XXL_PIZZA = new ObjectId("000000000000000000000004");
const FREE_CANNOLI_IF_ONE_LG_PIZZA = new ObjectId("000000000000000000000005");
const FREE_CANNOLIS = new ObjectId("000000000000000000000006");
const FREE_CANNOLI_IF_ONE_CANNOLI = new ObjectId("000000000000000000000007");
const HALF_CANNOLI_IF_ONE_CANNOLI = new ObjectId("000000000000000000000008");
const TEN_PCT_OFF_CANNOLIS = new ObjectId("000000000000000000000009");

/**
 * Makes a new item with the specified information.
 * @param {ObjectId} id - The item's ID
 * @param {string} name - The Item's name
 * @param {number} type - The type of item
 * @param {number} price - The price of the item.
 * @param {number} taxRate - The tax rate applied to the item.
 * @returns The item to generate.
 */
function newItem(id, name, type, price, taxRate) {
  return {
    _id: id,
    itemName: name,
    itemType: type,
    price: price,
    taxRate: taxRate,
  };
}

/**
 * Create a table in the DB if the table doesn't exist, otherwise ignores the command.
 * @param {string} tableName - The table to create.
 */
function createTableIfNotExist(tableName) {
  console.log("Checking if", tableName, "exists");
  var exists = db.system.namespace.find({ name: "AgnosCoffee" + tableName });
  if (!exists) {
    console.log("Creating", tableName);
    db.createCollection(tableName);
  } else {
    console.log("Not creating", tableName);
  }
}
//#endregion

//#region DB Population
var connectionString = new Mongo(
  "mongodb://127.0.0.1:27017/?directConnection=true&serverSelectionTimeoutMS=2000&appName=mongosh+1.3.1"
);
db = connectionString.getDB("AgnosCoffee");

//TODO: using this as a migration removal tool... maybe remove in order to keep this a one time run.
tableNames.forEach((table) => {
  createTableIfNotExist(table);
  console.log("Clearing", table);
  db[table].drop();
});

console.log("Inserting MenuItems");
var menuItemRes = db.Menu.insertMany([
  //     |ITEM_ID|       |ITEM_NAME|                   |ITEM_TYPE| |PRICE| |TAX RATE|
  newItem(BANANA_ID, "Banana", APPETIZER, 0.5, 0.0),
  newItem(MUFFIN_ID, "Muffin", APPETIZER, 1.0, 0.1),
  newItem(SMALL_BVG_ID, "Small Carbonated Beverage", BEVERAGE, 1.5, 0.0825),
  newItem(MED_BVG_ID, "Medium Carbonated Beverage", BEVERAGE, 2.5, 0.12),
  newItem(LARGE_BVG_ID, "Large Carbonated Beverage", BEVERAGE, 2.6, 0.11),
  newItem(HAMBURGER_ID, "Hamburger", ENTREE, 3.99, 0.0825),
  newItem(SMALL_PIZZA_ID, "Small Pizza", ENTREE, 6.99, 0.0825),
  newItem(MED_PIZZA_ID, "Medium Pizza", ENTREE, 8.99, 0.0825),
  newItem(LARGE_PIZZA_ID, "Large Pizza", ENTREE, 15.99, 0.1),
  newItem(XXL_PIZZA_ID, "X-X-Large Pizza", ENTREE, 49.99, 0.3),
  newItem(SMALL_FRIES_ID, "Small Fries", ENTREE, 2.99, 0.15),
  newItem(LARGE_FRIES_ID, "Large Fries", SIDE, 5.99, 0.2),
  newItem(ONION_RINGS_ID, "Onion Rings", SIDE, 8.99, 0.25),
  newItem(CHOCO_CAKE_ID, "Chocolate Cake", DESERT, 9.99, 0.025),
  newItem(CANNOLI_ID, "Cannoli", DESERT, 1.99, 0.1),
]);
if (menuItemRes.acknowledged) {
  console.log(
    "Inserted",
    Object.keys(menuItemRes.insertedIds).length,
    "MenuItems rows successfully"
  );
}
console.log("Inserting Deals");
var dealRes = db.Deals.insertMany([
  {
    _id: FREE_BVG_IF_ONE_BURGER,
    dealCode: "ThirstyFromThatBurger",
    constraints: {
      mustPurchaseQuantity: 1,
      mustPurchaseItem: HAMBURGER_ID,
      mustPurchaseItemName: "Hamburger",
    },
    effects: {
      discountPercentage: 1.0,
      appliesToItemType: BEVERAGE,
      quantity: 1,
    },
  },
  {
    _id: FREE_ENTREE_IF_TWO_ENTREE,
    dealCode: "TwoFer",
    constraints: {
      mustPurchaseQuantity: 2,
      mustPurchaseItemType: ENTREE,
    },
    effects: {
      discountPercentage: 1.0,
      appliesToItemType: ENTREE,
      quantity: 1,
    },
  },
  {
    _id: HALF_OFF_IF_ONE_XXL_PIZZA,
    dealCode: "HungryPersonSpecial",
    constraints: {
      mustPurchaseQuantity: 1,
      mustPurchaseItem: XXL_PIZZA_ID,
      mustPurchaseItemName: "X-X-Large Pizza",
    },
    effects: {
      discountPercentage: 0.5,
    },
  },
  {
    _id: FREE_CANNOLI_IF_ONE_LG_PIZZA,
    dealCode: "HaveSomeCanoliWithThat",
    constraints: {
      mustPurchaseQuantity: 1,
      mustPurchaseItem: LARGE_PIZZA_ID,
      mustPurchaseItemName: "Large Pizza",
    },
    effects: {
      discountPercentage: 1.0,
      appliesToItemId: CANNOLI_ID,
      appliesToItemName: "Cannoli",
      quantity: 1,
    },
  },
  {
    _id: FREE_CANNOLIS,
    dealCode: "SweetTimes",
    constraints: {},
    effects: {
      discountPercentage: 1.0,
      appliesToItemId: CANNOLI_ID,
      appliesToItemName: "Cannoli",
    },
  },
  {
    _id: TEN_PCT_OFF_CANNOLIS,
    dealCode: "10OffCannoli",
    constraints: {},
    effects: {
      discountPercentage: 0.1,
      appliesToItemId: CANNOLI_ID,
      appliesToItemName: "Cannoli",
    },
  },
  {
    _id: FREE_CANNOLI_IF_ONE_CANNOLI,
    dealCode: "Cannoli4Cannoli",
    constraints: {},
    effects: {
      discountPercentage: 1.0,
      appliesToItemId: CANNOLI_ID,
      appliesToItemName: "Cannoli",
      quantity: 1,
      requireMin: 2,
    },
  },
  {
    _id: HALF_CANNOLI_IF_ONE_CANNOLI,
    dealCode: "HalfACannoli",
    constraints: {},
    effects: {
      discountPercentage: 0.5,
      appliesToItemId: CANNOLI_ID,
      appliesToItemName: "Cannoli",
      quantity: 1,
      requireMin: 2,
    },
  },
]);
if (dealRes.acknowledged) {
  console.log(
    "Inserted",
    Object.keys(dealRes.insertedIds).length,
    "Deals rows successfully"
  );
}
//#endregion
