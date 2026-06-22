using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Services;
using BusinessObjects;

namespace HoLeQuocKhanhMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index(string searchString)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff")
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = _categoryService.GetCategories();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                categories = categories.Where(c => 
                    (c.CategoryName != null && c.CategoryName.ToLower().Contains(searchString)) ||
                    (c.CategoryDesciption != null && c.CategoryDesciption.ToLower().Contains(searchString))
                ).ToList();
            }

            ViewBag.CurrentSearch = searchString;
            return View(categories);
        }

        [HttpGet]
        public IActionResult GetCategoryModal(short? id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff")
            {
                return Challenge();
            }

            Category? category = null;
            if (id.HasValue)
            {
                category = _categoryService.GetCategoryById(id.Value);
            }

            if (category == null)
            {
                category = new Category
                {
                    IsActive = true
                };
            }

            // Provide active parent categories list for dropdown (exclude current category to prevent circular reference)
            var parentCategories = _categoryService.GetCategories()
                .Where(c => c.IsActive == true && (!id.HasValue || c.CategoryId != id.Value))
                .ToList();

            ViewBag.ParentCategories = parentCategories;
            return PartialView("_CategoryModal", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCategory(Category category)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff")
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            if (!ModelState.IsValid)
            {
                var parentCategories = _categoryService.GetCategories()
                    .Where(c => c.IsActive == true && (category.CategoryId == 0 || c.CategoryId != category.CategoryId))
                    .ToList();
                ViewBag.ParentCategories = parentCategories;
                return PartialView("_CategoryModal", category);
            }

            try
            {
                if (category.CategoryId == 0)
                {
                    // Create
                    _categoryService.SaveCategory(category);
                }
                else
                {
                    // Update
                    _categoryService.UpdateCategory(category);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving category: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(short id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff")
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            try
            {
                var category = _categoryService.GetCategoryById(id);
                if (category != null)
                {
                    _categoryService.DeleteCategory(category);
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Category not found." });
            }
            catch (InvalidOperationException ex)
            {
                // This is our custom exception for references check
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting category: " + ex.Message });
            }
        }
    }
}
