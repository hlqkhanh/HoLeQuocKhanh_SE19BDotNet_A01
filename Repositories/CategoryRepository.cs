using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetCategories() => CategoryDAO.Instance.GetCategories();
        public Category? GetCategoryById(short categoryId) => CategoryDAO.Instance.GetCategoryById(categoryId);
        public void SaveCategory(Category category) => CategoryDAO.Instance.SaveCategory(category);
        public void UpdateCategory(Category category) => CategoryDAO.Instance.UpdateCategory(category);
        public void DeleteCategory(Category category) => CategoryDAO.Instance.DeleteCategory(category);
    }
}
