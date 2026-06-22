using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class CategoryDAO
    {
        private static CategoryDAO? _instance = null;
        private static readonly object _instanceLock = new object();

        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new CategoryDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<Category> GetCategories()
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.Categories.Include(c => c.ParentCategory).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCategories: " + ex.Message);
            }
        }

        public Category? GetCategoryById(short categoryId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.Categories.Include(c => c.ParentCategory).FirstOrDefault(c => c.CategoryId == categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCategoryById: " + ex.Message);
            }
        }

        public void SaveCategory(Category category)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Categories.Add(category);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SaveCategory: " + ex.Message);
            }
        }

        public void UpdateCategory(Category category)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry(category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateCategory: " + ex.Message);
            }
        }

        public void DeleteCategory(Category category)
        {
            try
            {
                using var context = new FunewsManagementContext();
                
                // Check if any news articles are referencing this category
                bool hasArticles = context.NewsArticles.Any(a => a.CategoryId == category.CategoryId);
                if (hasArticles)
                {
                    throw new InvalidOperationException("This category is associated with one or more news articles and cannot be deleted.");
                }

                // Check if it is a parent category of other categories
                bool hasSubCategories = context.Categories.Any(c => c.ParentCategoryId == category.CategoryId && c.CategoryId != category.CategoryId);
                if (hasSubCategories)
                {
                    throw new InvalidOperationException("This category has sub-categories and cannot be deleted.");
                }

                var cat = context.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                if (cat != null)
                {
                    context.Categories.Remove(cat);
                    context.SaveChanges();
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteCategory: " + ex.Message);
            }
        }
    }
}
