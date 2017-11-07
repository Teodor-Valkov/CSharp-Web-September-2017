namespace _05.ShopHierarchy
{
    using Data;
    using System;
    using System.Linq;

    public class HelperMethods
    {
        // Task 5
        //
        public static void PrintSalesmenWithCustomersCount(MyDbContext context)
        {
            var filteredSalesmen = context.Salesmen
                .Select(s => new
                {
                    Name = s.Name,
                    Customers = s.Customers.Count
                })
                .OrderByDescending(s => s.Customers)
                .ThenBy(s => s.Name)
                .ToList();

            foreach (var salesmen in filteredSalesmen)
            {
                Console.WriteLine($"{salesmen.Name} - {salesmen.Customers} customers");
            }
        }

        // Task 6
        //
        public static void PrintCustomersWithOrdersCountAndReviewsCount(MyDbContext context)
        {
            var filteredCustomers = context.Customers
                .Select(c => new
                {
                    Name = c.Name,
                    Orders = c.Orders.Count,
                    Reviews = c.Reviews.Count
                })
                .OrderByDescending(c => c.Orders)
                .ThenByDescending(c => c.Reviews)
                .ToList();

            foreach (var customer in filteredCustomers)
            {
                Console.WriteLine(customer.Name);
                Console.WriteLine($"Orders: {customer.Orders}");
                Console.WriteLine($"Reviews: {customer.Reviews}");
            }
        }

        // Task 7
        //
        public static void PrintCustomerWithOrdersCountItemsCountAndReviewsCount(MyDbContext context)
        {
            int customerId = int.Parse(Console.ReadLine());

            var filteredCustomer = context.Customers
                .Where(c => c.Id == customerId)
                .Select(c => new
                {
                    Orders = c.Orders
                        .Select(o => new
                        {
                            Id = o.Id,
                            Items = o.Items.Count
                        })
                        .OrderBy(o => o.Id),
                    Reviews = c.Reviews.Count
                })
                .FirstOrDefault();

            foreach (var order in filteredCustomer.Orders)
            {
                Console.WriteLine($"order {order.Id}: {order.Items} items");
            }

            Console.WriteLine($"reviews: {filteredCustomer.Reviews}");
        }

        // Task 8
        //
        public static void PrintCustomerWithOrdersCountReviewsCountAndSalesman(MyDbContext context)
        {
            int customerId = int.Parse(Console.ReadLine());

            var filteredCustomer = context.Customers
                .Where(c => c.Id == customerId)
                .Select(c => new
                {
                    Name = c.Name,
                    Orders = c.Orders.Count,
                    Reviews = c.Reviews.Count,
                    Salesman = c.Salesman.Name
                })
                .FirstOrDefault();

            Console.WriteLine($"Customer: {filteredCustomer.Name}");
            Console.WriteLine($"Orders count: {filteredCustomer.Orders}");
            Console.WriteLine($"Reviews count: {filteredCustomer.Reviews}");
            Console.WriteLine($"Salesman: {filteredCustomer.Salesman}");
        }

        // Task 9
        //
        public static void PrintCustomerWithOrdersCountOfMoreThanOneItem(MyDbContext context)
        {
            int customerId = int.Parse(Console.ReadLine());

            var filteredOrders = context.Orders
                .Where(o => o.CustomerId == customerId)
                .Where(o => o.Items.Count > 1)
                .Count();

            Console.WriteLine($"Orders: {filteredOrders}");
        }
    }
}