namespace FitStore.Tests.Services.Implementations
{
    using Data;
    using Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Implementations;
    using FitStore.Services.Models;
    using FitStore.Services.Models.Orders;
    using FitStore.Services.Models.Supplements;
    using FluentAssertions;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class OrderServiceTest : BaseServiceTest
    {
        private const string supplementName = "supplement";
        private const string manufacturerName = "manufacturer";
        private const string userId = "User Id";
        private const int supplementId = 1;
        private const int orderId = 1;
        private const int nonExistingOrderId = int.MaxValue;

        [Fact]
        public async Task GetDetailsByIdAsync_WithOrderId_ShouldReturnValidServiceModel()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IOrderService orderService = new OrderService(database);

            // Act
            OrderDetailsServiceModel result = await orderService.GetDetailsByIdAsync(orderId);

            // Assert
            result.Id.Should().Be(1);
            result.TotalPrice.Should().Be(1);
            result.Supplements.Count().Should().Be(1);
        }

        [Fact]
        public async Task AddSupplementToCartAsync_WithSupplementId_ShouldReturnFalseWithQuantityEqualsToZero()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            database.Supplements.Find(supplementId).Quantity = 0;
            database.SaveChanges();

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = await orderService.AddSupplementToCartAsync(supplementId, null);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task AddSupplementToCartAsync_WithSupplementId_ShouldIncrementQuantityInCartAndReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Supplement supplement = database.Supplements.Find(supplementId);
            SupplementInCartServiceModel supplementInCart = new SupplementInCartServiceModel
            {
                Id = supplement.Id,
                Name = supplement.Name,
                Quantity = supplement.Quantity,
                Price = supplement.Price,
                ManufacturerName = supplement.Manufacturer.Name
            };

            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.Supplements.Add(supplementInCart);

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = await orderService.AddSupplementToCartAsync(supplementId, shoppingCart);

            // Assert
            result.Should().Be(true);
            supplementInCart.Quantity.Should().Be(supplement.Quantity + 1);
        }

        [Fact]
        public async Task AddSupplementToCartAsync_WithSupplementId_ShouldAddSupplementInCartAndReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ShoppingCart shoppingCart = new ShoppingCart();

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = await orderService.AddSupplementToCartAsync(supplementId, shoppingCart);

            // Assert
            result.Should().Be(true);
            shoppingCart.Supplements.Count().Should().Be(1);
        }

        [Fact]
        public void RemoveSupplementFromCartAsync_WithSupplementId_ShouldReturnFalseWithSupplementNotInShoppingCart()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ShoppingCart shoppingCart = new ShoppingCart();

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = orderService.RemoveSupplementFromCartAsync(supplementId, shoppingCart);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void RemoveSupplementFromCartAsync_WithSupplementId_ShouldDecreaseQuantityInCartAndReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Supplement supplement = database.Supplements.Find(supplementId);
            SupplementInCartServiceModel supplementInCart = new SupplementInCartServiceModel
            {
                Id = supplement.Id,
                Name = supplement.Name,
                Quantity = 10,
                Price = supplement.Price,
                ManufacturerName = supplement.Manufacturer.Name
            };

            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.Supplements.Add(supplementInCart);

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = orderService.RemoveSupplementFromCartAsync(supplementId, shoppingCart);

            // Assert
            result.Should().Be(true);
            supplementInCart.Quantity.Should().Be(9);
        }

        [Fact]
        public void RemoveSupplementFromCartAsync_WithSupplementId_ShouldRemoveSupplementFromCartAndReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Supplement supplement = database.Supplements.Find(supplementId);
            SupplementInCartServiceModel supplementInCart = new SupplementInCartServiceModel
            {
                Id = supplement.Id,
                Name = supplement.Name,
                Quantity = 1,
                Price = supplement.Price,
                ManufacturerName = supplement.Manufacturer.Name
            };

            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.Supplements.Add(supplementInCart);

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = orderService.RemoveSupplementFromCartAsync(supplementId, shoppingCart);

            // Assert
            result.Should().Be(true);
            shoppingCart.Supplements.Count().Should().Be(0);
        }

        [Fact]
        public void RemoveAllSupplementsFromCartAsync_WithSupplementId_ShouldReturnFalseWithSupplementNotInShoppingCart()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ShoppingCart shoppingCart = new ShoppingCart();

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = orderService.RemoveAllSupplementsFromCartAsync(supplementId, shoppingCart);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void RemoveAllSupplementsFromCartAsync_WithSupplementId_ShouldRemoveSupplementFromCartAndReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Supplement supplement = database.Supplements.Find(supplementId);
            SupplementInCartServiceModel supplementInCart = new SupplementInCartServiceModel
            {
                Id = supplement.Id,
                Name = supplement.Name,
                Quantity = 10,
                Price = supplement.Price,
                ManufacturerName = supplement.Manufacturer.Name
            };

            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.Supplements.Add(supplementInCart);

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = orderService.RemoveAllSupplementsFromCartAsync(supplementId, shoppingCart);

            // Assert
            result.Should().Be(true);
            shoppingCart.Supplements.Count().Should().Be(0);
        }

        [Fact]
        public async Task IsOrderExistingById_WithOrderId_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = await orderService.IsOrderExistingById(orderId);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task IsOrderExistingById_WithOrderId_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = await orderService.IsOrderExistingById(nonExistingOrderId);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsLastAvailableSupplementAlreadyAdded_WithSupplementIdAndShoppingCartWithoutSupplement_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            ShoppingCart shoppingCart = new ShoppingCart();

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = await orderService.IsLastAvailableSupplementAlreadyAdded(supplementId, shoppingCart);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsLastAvailableSupplementAlreadyAdded_WithSupplementIdAndShoppingCartWithSupplement_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Supplement supplement = database.Supplements.Find(supplementId);
            SupplementInCartServiceModel supplementInCart = new SupplementInCartServiceModel
            {
                Id = supplement.Id,
                Name = supplement.Name,
                Quantity = 0,
                Price = supplement.Price,
                ManufacturerName = supplement.Manufacturer.Name
            };

            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.Supplements.Add(supplementInCart);

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = await orderService.IsLastAvailableSupplementAlreadyAdded(supplementId, shoppingCart);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task IsLastAvailableSupplementAlreadyAdded_WithSupplementIdAndShoppingCartWithSupplement_ShouldReturnTrue()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Supplement supplement = database.Supplements.Find(supplementId);
            SupplementInCartServiceModel supplementInCart = new SupplementInCartServiceModel
            {
                Id = supplement.Id,
                Name = supplement.Name,
                Quantity = 1,
                Price = supplement.Price,
                ManufacturerName = supplement.Manufacturer.Name
            };

            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.Supplements.Add(supplementInCart);

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = await orderService.IsLastAvailableSupplementAlreadyAdded(supplementId, shoppingCart);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task FinishOrderAsync_WithQuantityLessThanAvailable_ShouldReturnFalse()
        {
            // Arrange
            FitStoreDbContext database = this.Database;
            DatabaseHelper.SeedData(database);

            Supplement supplement = database.Supplements.Find(supplementId);
            SupplementInCartServiceModel supplementInCart = new SupplementInCartServiceModel
            {
                Id = supplement.Id,
                Name = supplement.Name,
                Quantity = 10,
                Price = supplement.Price,
                ManufacturerName = supplement.Manufacturer.Name
            };

            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.Supplements.Add(supplementInCart);

            IOrderService orderService = new OrderService(database);

            // Act
            bool result = await orderService.FinishOrderAsync(userId, shoppingCart);

            // Assert
            result.Should().Be(false);
        }

        // To find out how to get over the exception for "Order cannot be tracked because another instance with the same key value for {Id} is already being tracked.";
        //
        //[Fact]
        //public async Task FinishOrderAsync_WithCorrectData_ShouldCreateOrderAndReturnTrue()
        //{
        //    // Arrange
        //    FitStoreDbContext database = this.Database;
        //    DatabaseHelper.SeedData(database);

        //    database.Supplements.Find(5).Quantity = 20;
        //    database.SaveChanges();

        //    SupplementInCartServiceModel supplementInCart = new SupplementInCartServiceModel
        //    {
        //        Id = 5,
        //        Name = "supplement",
        //        Quantity = 10,
        //        Price = 10,
        //        ManufacturerName = "manufacturer"
        //    };

        //    ShoppingCart shoppingCart = new ShoppingCart();
        //    shoppingCart.Supplements.Add(supplementInCart);

        //    IOrderService orderService = new OrderService(database);

        //    // Act
        //    bool result = await orderService.FinishOrderAsync(userId, shoppingCart);

        //    // Assert
        //    result.Should().Be(true);

        //    Order order = database.Orders.Find(orderId);

        //    order.UserId.Should().Be(userId);
        //    order.TotalPrice.Should().Be(100);
        //    order.Supplements.Count().Should().Be(1);
        //    order.Supplements.First().Quantity.Should().Be(10);
        //    order.Supplements.First().Supplement.Name.Should().Be(supplementName);
        //    order.Supplements.First().Supplement.Manufacturer.Name.Should().Be(manufacturerName);
        //    order.PurchaseDate.Date.Should().Be(DateTime.UtcNow.Date);
        //}
    }
}