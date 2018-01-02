namespace FitStore.Services.Implementations
{
    using AutoMapper;
    using Data;
    using Data.Models;
    using Contracts;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Models.Supplements;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FitStore.Services.Models.Orders;
    using AutoMapper.QueryableExtensions;

    public class OrderService : IOrderService
    {
        private readonly FitStoreDbContext database;

        public OrderService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<OrderDetailsServiceModel> GetDetailsByIdAsync(int orderId)
        {
            return await this.database
                .Orders
                .Where(o => o.Id == orderId)
                .ProjectTo<OrderDetailsServiceModel>()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddSupplementToCartAsync(int supplementId, ShoppingCart shoppingCart)
        {
            Supplement supplement = await this.database
                .Supplements
                .Include(s => s.Manufacturer)
                .Where(s => s.Id == supplementId)
                .FirstOrDefaultAsync();

            if (supplement.Quantity == 0)
            {
                return false;
            }

            if (shoppingCart.Supplements.Any(s => s.Id == supplementId))
            {
                shoppingCart.Supplements.Where(s => s.Id == supplementId).FirstOrDefault().Quantity += 1;
            }
            else
            {
                SupplementInCartServiceModel supplementInCart = Mapper.Map<SupplementInCartServiceModel>(supplement);

                supplementInCart.Quantity = 1;

                shoppingCart.Supplements.Add(supplementInCart);
            }

            return true;
        }

        public bool RemoveSupplementFromCartAsync(int supplementId, ShoppingCart shoppingCart)
        {
            if (shoppingCart.Supplements.Any(s => s.Id == supplementId))
            {
                SupplementInCartServiceModel supplemenInCart = shoppingCart
                    .Supplements
                    .Where(s => s.Id == supplementId)
                    .FirstOrDefault();

                supplemenInCart.Quantity -= 1;

                if (supplemenInCart.Quantity == 0)
                {
                    shoppingCart.Supplements.Remove(supplemenInCart);
                }

                return true;
            }

            return false;
        }

        public bool RemoveAllSupplementsFromCartAsync(int supplementId, ShoppingCart shoppingCart)
        {
            if (shoppingCart.Supplements.Any(s => s.Id == supplementId))
            {
                SupplementInCartServiceModel supplemenInCart = shoppingCart
                    .Supplements
                    .Where(s => s.Id == supplementId)
                    .FirstOrDefault();

                shoppingCart.Supplements.Remove(supplemenInCart);

                return true;
            }

            return false;
        }

        public async Task<bool> IsOrderExistingById(int orderId)
        {
            return await this.database
                .Orders
                .AnyAsync(o => o.Id == orderId);
        }

        public async Task<bool> IsLastAvailableSupplementAlreadyAdded(int supplementId, ShoppingCart shoppingCart)
        {
            Supplement supplement = await this.database
                .Supplements
                .Where(s => s.Id == supplementId)
                .FirstOrDefaultAsync();

            SupplementInCartServiceModel supplementInCart = shoppingCart
                .Supplements
                .Where(s => s.Id == supplementId)
                .FirstOrDefault();

            if (supplementInCart == null)
            {
                return false;
            }

            return supplement.Quantity == supplementInCart.Quantity;
        }

        public async Task<bool> FinishOrderAsync(string userId, ShoppingCart shoppingCart)
        {
            foreach (SupplementInCartServiceModel supplementInCart in shoppingCart.Supplements)
            {
                Supplement supplement = await this.database
                    .Supplements
                    .Where(s => s.Id == supplementInCart.Id)
                    .FirstOrDefaultAsync();

                if (supplement.Quantity < supplementInCart.Quantity)
                {
                    return false;
                }

                supplement.Quantity -= supplementInCart.Quantity;
            }

            Order order = new Order
            {
                UserId = userId,
                PurchaseDate = DateTime.UtcNow,
                TotalPrice = shoppingCart.TotalPrice
            };

            await this.database.Orders.AddAsync(order);
            await this.database.SaveChangesAsync();

            foreach (SupplementInCartServiceModel supplementInCart in shoppingCart.Supplements)
            {
                OrderSupplements supplement = new OrderSupplements
                {
                    OrderId = order.Id,
                    SupplementId = supplementInCart.Id,
                    Quantity = supplementInCart.Quantity,
                    Price = supplementInCart.Price
                };

                order.Supplements.Add(supplement);
            }

            await this.database.SaveChangesAsync();

            return true;
        }
    }
}