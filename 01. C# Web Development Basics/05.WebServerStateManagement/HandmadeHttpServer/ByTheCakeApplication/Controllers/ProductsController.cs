namespace HandmadeHttpServer.ByTheCakeApplication.Controllers
{
    using HandmadeHttpServer.ByTheCakeApplication.Helpers;
    using HandmadeHttpServer.ByTheCakeApplication.Models;
    using HandmadeHttpServer.ByTheCakeApplication.Services;
    using HandmadeHttpServer.ByTheCakeApplication.Services.Contracts;
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Product;
    using HandmadeHttpServer.Server.Http.Contracts;
    using HandmadeHttpServer.Server.Http.Response;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProductsController : Controller
    {
        private const string AddView = @"Products\add";
        private const string SearchView = @"Products\search";
        private const string DetailsView = @"Products\details";

        private readonly IProductService productService;

        public ProductsController()
        {
            this.productService = new ProductService();
        }

        public IHttpResponse Add()
        {
            this.SetDefaultViewBag();

            return this.FileViewResponse(AddView);
        }

        public IHttpResponse Add(AddProductViewModel model)
        {
            this.SetDefaultViewBag();

            bool isModelValid = this.CheckIfModelIsValid(model);

            if (!isModelValid)
            {
                return this.FileViewResponse(AddView);
            }

            this.productService.CreateProduct(model.Name, model.Price, model.ImageUrl);

            this.ViewBag["displayResult"] = DisplayBlock;
            this.ViewBag["name"] = model.Name;
            this.ViewBag["price"] = model.Price.ToString();
            this.ViewBag["imageUrl"] = model.ImageUrl;

            return this.FileViewResponse(AddView);
        }

        public IHttpResponse Search(IHttpRequest httpRequest)
        {
            this.SetDefaultViewBag();

            string searchProductKey = "searchProduct";

            string searchProduct = httpRequest.UrlParameters.ContainsKey(searchProductKey)
                ? httpRequest.UrlParameters[searchProductKey]
                : null;

            this.ViewBag["searchProduct"] = searchProduct;

            IEnumerable<ProductListingViewModel> products = this.productService.GetAllProducts(searchProduct);

            this.ViewBag["results"] = "No cakes found!";

            if (products.Any())
            {
                IEnumerable<string> productsDivs = products.Select(c => $@"<div><a href=""/products/{c.Id}"">{c.Name}</a> - ${c.Price:F2} <a href=""/shopping/add/{c.Id}?searchProduct={searchProduct}"">Order</a></div>");

                string productsDivsAsString = string.Join(Environment.NewLine, productsDivs);

                this.ViewBag["results"] = productsDivsAsString;
            }

            ShoppingCart shoppingCart = httpRequest.Session.GetSession<ShoppingCart>(ShoppingCart.CurrentShoppingCartSessionKey);

            if (shoppingCart.ProductIdsWithQuantity.Any())
            {
                int totalProductsCount = shoppingCart.ProductIdsWithQuantity.Sum(pq => pq.Value);
                string totalProductsText = totalProductsCount != 1 ? "products" : "product";

                this.ViewBag["displayResult"] = DisplayBlock;
                this.ViewBag["products"] = $"{totalProductsCount} {totalProductsText}";
            }

            return this.FileViewResponse(SearchView);
        }

        public IHttpResponse Details(int productId)
        {
            bool isProductExisting = this.productService.Exists(productId);

            if (!isProductExisting)
            {
                return new NotFoundResponse();
            }

            ProductDetailsViewModel product = this.productService.FindProduct(productId);

            if (product == null)
            {
                return new NotFoundResponse();
            }

            this.ViewBag["name"] = product.Name;
            this.ViewBag["price"] = product.Price.ToString("F2");
            this.ViewBag["imageUrl"] = product.ImageUrl;

            return this.FileViewResponse(DetailsView);
        }

        private void SetDefaultViewBag()
        {
            this.ViewBag["displayResult"] = DisplayNone;
            this.ViewBag["displayError"] = DisplayNone;
        }

        private bool CheckIfModelIsValid(AddProductViewModel model)
        {
            if (model.Name.Length < 3 || model.Name.Length > 30)
            {
                this.AddError("Product name should be between 3 and 30 symbol!");
                return false;
            }

            if (model.ImageUrl.Length < 3 || model.ImageUrl.Length > 2000)
            {
                this.AddError("Image not supported!");
                return false;
            }

            return true;
        }
    }
}