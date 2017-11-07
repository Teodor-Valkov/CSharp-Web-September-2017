namespace _05.ShopHierarchy
{
    using Data;
    using Models;
    using System;

    public class StartUp
    {
        public static void Main()
        {
            using (MyDbContext context = new MyDbContext())
            {
                PrepareDatabase(context);

                AddSalesmenToDatabase(context);
                AddItemsToDatabase(context);

                ProcessCommands(context);

                // HelperMethods.PrintSalesmenWithCustomersCount(context);
                // HelperMethods.PrintCustomersWithOrdersCountAndReviewsCount(context);
                // HelperMethods.PrintCustomerWithOrdersCountItemsCountAndReviewsCount(context);
                // HelperMethods.PrintCustomerWithOrdersCountReviewsCountAndSalesman(context);
                // HelperMethods.PrintCustomerWithOrdersCountOfMoreThanOneItem(context);
            }
        }

        private static void PrepareDatabase(MyDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        private static void AddSalesmenToDatabase(MyDbContext context)
        {
            string[] salesmenArgs = Console.ReadLine().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string name in salesmenArgs)
            {
                Salesman salesman = new Salesman
                {
                    Name = name
                };

                context.Salesmen.Add(salesman);
            }

            context.SaveChanges();
        }

        private static void AddItemsToDatabase(MyDbContext context)
        {
            string input = string.Empty;
            while ((input = Console.ReadLine()) != "END")
            {
                string[] itemArgs = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string name = itemArgs[0];
                decimal price = decimal.Parse(itemArgs[1]);

                Item item = new Item
                {
                    Name = name,
                    Price = price
                };

                context.Items.Add(item);
            }

            context.SaveChanges();
        }

        private static void ProcessCommands(MyDbContext context)
        {
            string input = string.Empty;
            while ((input = Console.ReadLine()) != "END")
            {
                string[] tokens = input.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                string commandName = tokens[0];
                string commandArgs = tokens[1];

                switch (commandName)
                {
                    case "register":
                        AddCustomerToDatabase(context, commandArgs);
                        break;

                    case "order":
                        AddOrderToDatabase(context, commandArgs);
                        break;

                    case "review":
                        AddReviewToDatabase(context, commandArgs);
                        break;

                    default:
                        throw new ArgumentException("Invalid command!");
                }
            }
        }

        private static void AddCustomerToDatabase(MyDbContext context, string input)
        {
            string[] customerArgs = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string name = customerArgs[0];
            int salesmanId = int.Parse(customerArgs[1]);

            Customer customer = new Customer
            {
                Name = name,
                SalesmanId = salesmanId
            };

            context.Customers.Add(customer);
            context.SaveChanges();
        }

        private static void AddOrderToDatabase(MyDbContext context, string input)
        {
            string[] orderArgs = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int customerId = int.Parse(orderArgs[0]);

            Order order = new Order()
            {
                CustomerId = customerId
            };

            for (int i = 1; i < orderArgs.Length; i++)
            {
                int itemId = int.Parse(orderArgs[i]);

                order.Items.Add(new ItemOrder
                {
                    ItemId = itemId
                });
            }

            context.Orders.Add(order);
            context.SaveChanges();
        }

        private static void AddReviewToDatabase(MyDbContext context, string input)
        {
            string[] reviewArgs = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int customerId = int.Parse(reviewArgs[0]);
            int itemId = int.Parse(reviewArgs[1]);

            Review review = new Review
            {
                CustomerId = customerId,
                ItemId = itemId
            };

            context.Reviews.Add(review);
            context.SaveChanges();
        }
    }
}