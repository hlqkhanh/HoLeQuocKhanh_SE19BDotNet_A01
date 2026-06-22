using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService()
        {
            _repository = new CategoryRepository();
        }

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public List<Category> GetCategories() => _repository.GetCategories();
        public Category? GetCategoryById(short categoryId) => _repository.GetCategoryById(categoryId);
        public void SaveCategory(Category category) => _repository.SaveCategory(category);
        public void UpdateCategory(Category category) => _repository.UpdateCategory(category);
        public void DeleteCategory(Category category) => _repository.DeleteCategory(category);
    }
}
