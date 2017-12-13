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

            supplement.Quantity -= 1;

            await this.database.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveSupplementFromCartAsync(int supplementId, ShoppingCart shoppingCart)
        {
            if (shoppingCart.Supplements.Any(s => s.Id == supplementId))
            {
                Supplement supplement = await this.database
                    .Supplements
                    .Include(s => s.Manufacturer)
                    .Where(s => s.Id == supplementId)
                    .FirstOrDefaultAsync();

                SupplementInCartServiceModel supplemenInCart = shoppingCart
                    .Supplements
                    .Where(s => s.Id == supplementId)
                    .FirstOrDefault();

                supplement.Quantity += 1;

                supplemenInCart.Quantity -= 1;

                if (supplemenInCart.Quantity == 0)
                {
                    shoppingCart.Supplements.Remove(supplemenInCart);
                }

                await this.database.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> RemoveAllSupplementsFromCartAsync(int supplementId, ShoppingCart shoppingCart)
        {
            if (shoppingCart.Supplements.Any(s => s.Id == supplementId))
            {
                Supplement supplement = await this.database
                    .Supplements
                    .Include(s => s.Manufacturer)
                    .Where(s => s.Id == supplementId)
                    .FirstOrDefaultAsync();

                SupplementInCartServiceModel supplemenInCart = shoppingCart
                    .Supplements
                    .Where(s => s.Id == supplementId)
                    .FirstOrDefault();

                supplement.Quantity += supplemenInCart.Quantity;

                shoppingCart.Supplements.Remove(supplemenInCart);

                await this.database.SaveChangesAsync();

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

        public async Task CancelOrderAsync(ShoppingCart shoppingCart)
        {
            foreach (SupplementInCartServiceModel supplementInCart in shoppingCart.Supplements)
            {
                Supplement supplement = await this.database
                .Supplements
                .Where(s => s.Id == supplementInCart.Id)
                .FirstOrDefaultAsync();

                supplement.Quantity += supplementInCart.Quantity;
            }

            await this.database.SaveChangesAsync();
        }

        public async Task FinishOrderAsync(string userId, ShoppingCart shoppingCart)
        {
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
                    Quantity = supplementInCart.Quantity
                };

                order.Supplements.Add(supplement);
            }

            await this.database.SaveChangesAsync();
        }
    }
}