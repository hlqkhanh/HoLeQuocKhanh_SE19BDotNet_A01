using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Services;
using BusinessObjects;
using Microsoft.Extensions.Configuration;

namespace HoLeQuocKhanhMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;

        public AccountController(IAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to respective homepage
            var role = HttpContext.Session.GetString("Role");
            if (!string.IsNullOrEmpty(role))
            {
                return RedirectToDashboard(role);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            // Check Admin Credentials from appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) && password == adminPassword)
            {
                HttpContext.Session.SetString("UserId", "0");
                HttpContext.Session.SetString("Username", "System Admin");
                HttpContext.Session.SetString("Email", adminEmail ?? "");
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("Accounts", "Admin");
            }

            // Check SystemAccount Table
            var account = _accountService.GetAccountByEmail(email);
            if (account != null && account.AccountPassword == password)
            {
                string roleName = account.AccountRole switch
                {
                    1 => "Staff",
                    2 => "Lecturer",
                    _ => "Guest"
                };

                HttpContext.Session.SetString("UserId", account.AccountId.ToString());
                HttpContext.Session.SetString("Username", account.AccountName ?? "User");
                HttpContext.Session.SetString("Email", account.AccountEmail ?? "");
                HttpContext.Session.SetString("Role", roleName);

                return RedirectToDashboard(roleName);
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var role = HttpContext.Session.GetString("Role");
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (role != "Staff" || string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login");
            }

            if (short.TryParse(userIdStr, out short userId))
            {
                var account = _accountService.GetAccountById(userId);
                if (account != null)
                {
                    return View(account);
                }
            }

            return NotFound("Account not found.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(SystemAccount updatedAccount)
        {
            var role = HttpContext.Session.GetString("Role");
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (role != "Staff" || string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login");
            }

            if (short.TryParse(userIdStr, out short userId) && updatedAccount.AccountId == userId)
            {
                try
                {
                    var existingAccount = _accountService.GetAccountById(userId);
                    if (existingAccount == null)
                    {
                        return NotFound("Account not found.");
                    }

                    existingAccount.AccountName = updatedAccount.AccountName;
                    existingAccount.AccountEmail = updatedAccount.AccountEmail;

                    // Only update password if a new one is provided
                    if (!string.IsNullOrEmpty(updatedAccount.AccountPassword))
                    {
                        existingAccount.AccountPassword = updatedAccount.AccountPassword;
                    }

                    _accountService.UpdateAccount(existingAccount);
                    
                    // Update session username just in case it changed
                    HttpContext.Session.SetString("Username", existingAccount.AccountName ?? "User");
                    HttpContext.Session.SetString("Email", existingAccount.AccountEmail ?? "");

                    ViewBag.Message = "Profile updated successfully!";
                    return View(existingAccount);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating profile: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid request parameters.");
            }

            return View(updatedAccount);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            var role = HttpContext.Session.GetString("Role");
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(userIdStr))
            {
                return Json(new { success = false, message = "Unauthorized access." });
            }

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                return Json(new { success = false, message = "Both old and new passwords are required." });
            }

            if (short.TryParse(userIdStr, out short userId))
            {
                var account = _accountService.GetAccountById(userId);
                if (account == null)
                {
                    return Json(new { success = false, message = "Account not found." });
                }

                if (account.AccountPassword != oldPassword)
                {
                    return Json(new { success = false, message = "Incorrect old password." });
                }

                try
                {
                    account.AccountPassword = newPassword;
                    _accountService.UpdateAccount(account);
                    return Json(new { success = true, message = "Password changed successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error updating password: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Invalid request." });
        }

        private IActionResult RedirectToDashboard(string role)
        {
            return role switch
            {
                "Admin" => RedirectToAction("Accounts", "Admin"),
                "Staff" => RedirectToAction("Index", "News"),
                "Lecturer" => RedirectToAction("Index", "News"),
                _ => RedirectToAction("Index", "News")
            };
        }
    }
}
