# AgnosCoffee

.Net/MongoDb

# **README**

If you plan to use postman, please check out the
PostmanCollection and PostmanEnvironment json files
at the root of the repository.

---

# Background

I have never used a NoSql DB! This sounds fun, Mongo FTW!
Here are the steps I followed... Play along!

1. brew install mongodb-community@5.0
1. brew services start mongodb-community@5.0
1. mongosh
1. load("{PATH_TO_REPOSITORY_HERE}/AgnosCoffee/mongo-scripts/dbsetup.js")

# **PROMPT**

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

---

# **Requirements and Design Decisions**

1.  Various categories.
    > - beverages,
    > - sandwiches
    > - etc.
1.  Items have varying tax rates
1.  deals - some items are free or offered at a discount when ordered with another item.

    > - Each category can have an impact on the other?
    > - One item has an impact on another in the same category?
    > - - Buy two sandwiches get one free?
    > - - Buy a meal, get a drink free?
    > - - Buy a drink get a side half off?

    > > The resulting decision became a `Condition`<->`Effect` relationship on a deal. If, for any order, conditions for a deal are met. The effects are applied, thusly.

1.  The customer is made aware of the order total and once he pays for it, he can wait until notified of the order completion.
    > - I take `notified` to mean "the HTTP request hangs until completion"

---

# Workflow

1. Customer calls /menu to view menu.
1. Customer calls /deals to view deals.
1. Customer places order, using item Ids and desired quantities from from `/menu`
   and deal codes from `/deals`
1. API returns the created orderId and totals.
1. User can use the createdOrderId to update their order
   as many times as desired.
1. User then pays for the order.
   The user can pay more than or equal to the total amount.
