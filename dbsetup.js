var connectionString = new Mongo("mongodb://127.0.0.1:27017/?directConnection=true&serverSelectionTimeoutMS=2000&appName=mongosh+1.3.1");

db = connectionString.getDB("AgnosCoffee");


//TODO: using this as a migration removal tool... maybe remove in order to keep this a one time run.
var exists = db.system.namespace.find( { name: 'AgnosCoffee.Menu' } );
if (!exists)
{
  db.createCollection("Menu")
}
db.Menu.drop(); 

//- DB Migrations.
db.Menu.insertMany(
  [
  {
    "itemName":"Banana"
  },
  {
    "itemName":"Muffin"
  },
  {
    "itemName":"Nehari"
  },
  {
    "itemName":"Pizza"
  }]
);