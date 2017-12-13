﻿namespace FitStore.Services.Contracts
{
    using Models.Subcategories;
    using System.Threading.Tasks;

    public interface ISubcategoryService
    {
        Task<SubcategoryDetailsServiceModel> GetDetailsByIdAsync(int categoryId);

        Task<int> GetCategoryIdBySubcategoryId(int subcategoryId);

        Task<bool> IsSubcategoryExistingById(int subcategoryId, bool isDeleted);

        Task<bool> IsSubcategoryExistingById(int subcategoryId);

        Task<bool> IsSubcategoryExistingByName(string name);

        Task<bool> IsSubcategoryExistingByIdAndName(int subcategoryId, string name);
    }
}