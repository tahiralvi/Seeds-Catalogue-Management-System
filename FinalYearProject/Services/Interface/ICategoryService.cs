using FinalYearProject.Models;

namespace FinalYearProject.Services.Interface
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<int> CreateCategoryAsync(Category category);
        Task<int> UpdateCategoryAsync(Category category);
        Task<int> DeleteCategoryAsync(int id);
        Task<Category> GetCategoryByIdAsync(int id);
    }
}
