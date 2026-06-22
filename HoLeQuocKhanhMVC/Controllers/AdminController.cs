using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Services;
using BusinessObjects;

namespace HoLeQuocKhanhMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly INewsArticleService _newsService;

        public AdminController(IAccountService accountService, INewsArticleService newsService)
        {
            _accountService = accountService;
            _newsService = newsService;
        }

        // Account management list
        public IActionResult Accounts(string searchString)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var accounts = _accountService.GetAccounts();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                accounts = accounts.Where(a => 
                    (a.AccountName != null && a.AccountName.ToLower().Contains(searchString)) ||
                    (a.AccountEmail != null && a.AccountEmail.ToLower().Contains(searchString))
                ).ToList();
            }

            ViewBag.CurrentSearch = searchString;
            return View(accounts);
        }

        [HttpGet]
        public IActionResult GetAccountModal(short? id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Challenge();
            }

            SystemAccount? account = null;
            bool isEdit = false;
            if (id.HasValue)
            {
                account = _accountService.GetAccountById(id.Value);
                isEdit = true;
            }

            if (account == null)
            {
                account = new SystemAccount();
                // Suggest next account ID
                int maxId = 0;
                var accounts = _accountService.GetAccounts();
                if (accounts.Any())
                {
                    maxId = accounts.Max(a => a.AccountId);
                }
                account.AccountId = (short)(maxId + 1);
            }

            ViewBag.IsEdit = isEdit;
            return PartialView("_AccountModal", account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAccount(SystemAccount account)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_AccountModal", account);
            }

            try
            {
                var isEdit = Request.Form["isEdit"] == "true";
                var existing = _accountService.GetAccountById(account.AccountId);

                if (isEdit)
                {
                    _accountService.UpdateAccount(account);
                }
                else
                {
                    if (existing != null)
                    {
                        return Json(new { success = false, message = $"Account ID {account.AccountId} already exists. Please choose a different ID." });
                    }
                    _accountService.SaveAccount(account);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving account: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(short id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            try
            {
                var account = _accountService.GetAccountById(id);
                if (account != null)
                {
                    _accountService.DeleteAccount(account);
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Account not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting account: " + ex.Message });
            }
        }

        // Report Statistics
        public IActionResult Report(DateTime? startDate, DateTime? endDate)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var articles = _newsService.GetNewsArticles();

            if (startDate.HasValue)
            {
                articles = articles.Where(a => a.CreatedDate >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                var endOfDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                articles = articles.Where(a => a.CreatedDate <= endOfDate).ToList();
            }

            // Sort data in descending order of CreatedDate
            articles = articles.OrderByDescending(a => a.CreatedDate).ToList();

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(articles);
        }
    }
}
