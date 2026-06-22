using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Services;
using BusinessObjects;

namespace HoLeQuocKhanhMVC.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public NewsController(INewsArticleService newsService, ICategoryService categoryService, ITagService tagService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        public IActionResult Index(string searchString, short? categoryFilter)
        {
            var role = HttpContext.Session.GetString("Role");
            var articles = _newsService.GetNewsArticles();

            // 1. Authorization Filter (Guests and Lecturers can only see Active news)
            if (role != "Staff")
            {
                articles = articles.Where(a => a.NewsStatus == true).ToList();
            }

            // 2. Category Filter
            if (categoryFilter.HasValue)
            {
                articles = articles.Where(a => a.CategoryId == categoryFilter.Value).ToList();
            }

            // 3. Search Filter (Search by Title, Headline, Content, Source, or Tag Name)
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                articles = articles.Where(a => 
                    (a.NewsTitle != null && a.NewsTitle.ToLower().Contains(searchString)) ||
                    (a.Headline != null && a.Headline.ToLower().Contains(searchString)) ||
                    (a.NewsContent != null && a.NewsContent.ToLower().Contains(searchString)) ||
                    (a.NewsSource != null && a.NewsSource.ToLower().Contains(searchString)) ||
                    a.Tags.Any(t => t.TagName != null && t.TagName.ToLower().Contains(searchString))
                ).ToList();
            }

            ViewBag.Categories = _categoryService.GetCategories().Where(c => c.IsActive == true).ToList();
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryFilter;

            return View(articles);
        }

        public IActionResult History()
        {
            var role = HttpContext.Session.GetString("Role");
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (role != "Staff" || string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            if (short.TryParse(userIdStr, out short userId))
            {
                var historyArticles = _newsService.GetNewsHistoryByCreatorId(userId);
                return View(historyArticles);
            }

            return RedirectToAction("Index");
        }

        // GET: News/GetModal (AJAX endpoint returning modal HTML)
        [HttpGet]
        public IActionResult GetNewsModal(string id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff")
            {
                return Challenge();
            }

            NewsArticle? article = null;
            if (!string.IsNullOrEmpty(id))
            {
                article = _newsService.GetNewsArticleById(id);
            }

            if (article == null)
            {
                article = new NewsArticle
                {
                    NewsStatus = true,
                    CreatedDate = DateTime.Now
                };
            }

            ViewBag.Categories = _categoryService.GetCategories().Where(c => c.IsActive == true).ToList();
            ViewBag.Tags = _tagService.GetTags();
            return PartialView("_NewsModal", article);
        }

        // POST: News/Save (AJAX form submit endpoint)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveNews(NewsArticle article, List<int> selectedTags)
        {
            var role = HttpContext.Session.GetString("Role");
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (role != "Staff" || string.IsNullOrEmpty(userIdStr))
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            if (!short.TryParse(userIdStr, out short userId))
            {
                return Json(new { success = false, message = "Session invalid." });
            }

            if (string.IsNullOrEmpty(article.NewsArticleId))
            {
                ModelState.Remove("NewsArticleId");
            }

            if (!ModelState.IsValid)
            {
                // Re-prepare view data if returning partial view with errors
                ViewBag.Categories = _categoryService.GetCategories().Where(c => c.IsActive == true).ToList();
                ViewBag.Tags = _tagService.GetTags();
                return PartialView("_NewsModal", article);
            }

            try
            {
                if (string.IsNullOrEmpty(article.NewsArticleId))
                {
                    // CREATE Mode
                    // Generate new unique String ID based on max integer value in DB
                    int maxId = 0;
                    foreach (var art in _newsService.GetNewsArticles())
                    {
                        if (int.TryParse(art.NewsArticleId, out int parsedId))
                        {
                            if (parsedId > maxId) maxId = parsedId;
                        }
                    }
                    article.NewsArticleId = (maxId + 1).ToString();
                    article.CreatedDate = DateTime.Now;
                    article.CreatedById = userId;
                    article.UpdatedById = userId;
                    article.ModifiedDate = DateTime.Now;

                    _newsService.SaveNewsArticle(article, selectedTags);
                }
                else
                {
                    // UPDATE Mode
                    var existing = _newsService.GetNewsArticleById(article.NewsArticleId);
                    if (existing == null)
                    {
                        return Json(new { success = false, message = "Article not found." });
                    }

                    // Keep creation details
                    article.CreatedById = existing.CreatedById;
                    article.CreatedDate = existing.CreatedDate;
                    
                    // Update audit details
                    article.UpdatedById = userId;
                    article.ModifiedDate = DateTime.Now;

                    _newsService.UpdateNewsArticle(article, selectedTags);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving article: " + ex.Message });
            }
        }

        // POST: News/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff")
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid ID." });
            }

            try
            {
                var article = _newsService.GetNewsArticleById(id);
                if (article != null)
                {
                    _newsService.DeleteNewsArticle(article);
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Article not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting article: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var article = _newsService.GetNewsArticleById(id);
            if (article == null)
            {
                return NotFound();
            }

            var role = HttpContext.Session.GetString("Role");
            // Guests and Lecturers can only see Active news
            if (role != "Staff" && article.NewsStatus != true)
            {
                return RedirectToAction("Index");
            }

            return View(article);
        }
    }
}
