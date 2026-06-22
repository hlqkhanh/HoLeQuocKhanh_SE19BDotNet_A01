using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        Category? GetCategoryById(short categoryId);
        void SaveCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
    }
}
