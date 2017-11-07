namespace HandmadeHttpServer.ByTheCakeApplication.Services.Contracts
{
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Product;
    using System.Collections.Generic;

    public interface IProductService
    {
        void CreateProduct(string name, decimal price, string imageUrl);

        IEnumerable<ProductListingViewModel> GetAllProducts(string searchTerm = null);

        ProductDetailsViewModel FindProduct(int id);

        bool Exists(int id);

        IEnumerable<ProductInCartViewModel> FindProductsInCart(IDictionary<int, int> productIdsWithQuantiy);
    }
}