# AgnosCoffee

.Net/MongoDb

PROMPT

---

A day at a coffee shop!
A customer visits a coffee shop that sells a bunch of items (e.g. beverages, sandwiches etc.).
Items have varying tax rates and some are free or offered at a discount when ordered with another item.
The customer is made aware of the order total and once he pays for it, he can wait until notified of the order completion.
Describe a program that models the above problem at hand.
Design and implement the DB and backend.
Users should be able to

- see the list of available items,
- order them
- get a notification after a fixed amount of time that their order is ready.

Requirement - Various categories. -> beverages, sandwiches etc.
Requirement - Items have varying tax rates

Requirement - deals - some are free or offered at a discount when ordered with another item.

- Each category can have an impact on the other?
- One item has an impact on another in the same category?
  i.e
  Buy two sandwiches get one free?
  Buy a meal, get a drink free?
  Buy a drink get a side half off?

Requirement - The customer is made aware of the order total and once he pays for it, he can wait until notified of the order completion.

- Customer places order, with as many calls as necessary
  ✅ GET menu -> return all menu items
  ✅ POST order -> return orderId, order information, order total
  ✅ PATCH order/orderId to update order -> return orderId, order information, order total
  POST order/pay
  {
  paymentInformation,
  paymentAmount
  } -> update
  IFF order fully paid, process order, return order.

I have never used a NoSql DB! This sounds fun, Mongo FTW!

Here are the steps I followed... Play along!

1. brew install mongodb-community@5.0
1. brew services start mongodb-community@5.0
1. mongosh
1. load("{PATH_TO_REPOSITORY_HERE}/AgnosCoffee/mongo-scripts/dbsetup.js")
