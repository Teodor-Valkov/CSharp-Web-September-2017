namespace HandmadeHttpServer.ByTheCakeApplication.Services
{
    using HandmadeHttpServer.ByTheCakeApplication.Data;
    using HandmadeHttpServer.ByTheCakeApplication.Data.Models;
    using HandmadeHttpServer.ByTheCakeApplication.Services.Contracts;
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Orders;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ShoppingService : IShoppingService
    {
        public void CreateOrder(int userId, IDictionary<int, int> productIdWithQuantity)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                Order order = new Order
                {
                    UserId = userId,
                    CreationDate = DateTime.UtcNow,
                    Products = productIdWithQuantity
                        .Select(pq => new OrderProduct
                        {
                            ProductId = pq.Key,
                            Quantity = pq.Value
                        })
                    .ToList()
                };

                database.Orders.Add(order);
                database.SaveChanges();
            }
        }

        public bool Exists(int orderId)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                return database.Orders.Any(o => o.Id == orderId);
            }
        }

        public IEnumerable<OrderListingViewModel> GetUserOrders(int userId)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                return database.Orders
                    .Where(o => o.UserId == userId)
                    .Select(o => new OrderListingViewModel
                    {
                        Id = o.Id,
                        CreationDate = o.CreationDate,
                        TotalSum = o.Products.Sum(op => op.Product.Price * op.Quantity)
                    })
                    .ToList();
            }
        }

        public OrderDetailsViewModel GetOrder(int orderId)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                OrderDetailsViewModel order = database.Orders
                    .Where(o => o.Id == orderId)
                    .Select(o => new OrderDetailsViewModel
                    {
                        Id = o.Id,
                        ProductIds = o.Products.Select(op => op.ProductId).ToList(),
                        ProductNames = o.Products.Select(op => op.Product.Name).ToList(),
                        ProductPrices = o.Products.Select(op => op.Product.Price).ToList(),
                        ProductQuantities = o.Products.Select(op => op.Quantity).ToList(),
                        CreationDate = o.CreationDate
                    })
                    .FirstOrDefault();

                return order;
            }
        }
    }
}