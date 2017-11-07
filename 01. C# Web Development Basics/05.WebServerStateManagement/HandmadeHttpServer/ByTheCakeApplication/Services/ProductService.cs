namespace HandmadeHttpServer.ByTheCakeApplication.Services
{
    using HandmadeHttpServer.ByTheCakeApplication.Data;
    using HandmadeHttpServer.ByTheCakeApplication.Data.Models;
    using HandmadeHttpServer.ByTheCakeApplication.Services.Contracts;
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Product;
    using System.Collections.Generic;
    using System.Linq;

    public class ProductService : IProductService
    {
        public void CreateProduct(string name, decimal price, string imageUrl)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                Product product = new Product
                {
                    Name = name,
                    Price = price,
                    ImageUrl = imageUrl
                };

                database.Products.Add(product);
                database.SaveChanges();
            }
        }

        public IEnumerable<ProductListingViewModel> GetAllProducts(string searchProduct = null)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                IQueryable<Product> resultsQuery = database.Products.AsQueryable();

                if (!string.IsNullOrEmpty(searchProduct))
                {
                    resultsQuery = resultsQuery.Where(p => p.Name.ToLower().Contains(searchProduct.ToLower()));
                }

                return resultsQuery.Select(p => new ProductListingViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToList();
            }
        }

        public ProductDetailsViewModel FindProduct(int id)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                return database.Products
                    .Where(p => p.Id == id)
                    .Select(p => new ProductDetailsViewModel
                    {
                        Name = p.Name,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl
                    })
                    .FirstOrDefault();
            }
        }

        public bool Exists(int id)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                return database.Products.Any(p => p.Id == id);
            }
        }

        public IEnumerable<ProductInCartViewModel> FindProductsInCart(IDictionary<int, int> productIdsWithQuantity)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                return database.Products
                    .Where(p => productIdsWithQuantity.ContainsKey(p.Id))
                    .Select(p => new ProductInCartViewModel
                    {
                        Name = p.Name,
                        Price = p.Price,
                        Quantity = productIdsWithQuantity[p.Id]
                    })
                    .ToList();
            }
        }
    }
}