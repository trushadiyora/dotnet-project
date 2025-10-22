using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ContactManagerApp.Models;
using ContactManagerApp.Services;
using System.Threading.Tasks;

namespace ContactManagerApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            // If already logged in, redirect to contacts
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Index", "Contacts");

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(model);

                if (result.Success)
                {
                    // Store user information in session
                    if (!string.IsNullOrEmpty(result.LocalId))
                        HttpContext.Session.SetString("UserId", result.LocalId);

                    if (!string.IsNullOrEmpty(result.Email))
                        HttpContext.Session.SetString("UserEmail", result.Email);

                    if (!string.IsNullOrEmpty(result.FirebaseToken))
                        HttpContext.Session.SetString("UserToken", result.FirebaseToken);

                    if (!string.IsNullOrEmpty(result.DisplayName))
                        HttpContext.Session.SetString("UserName", result.DisplayName);

                    TempData["SuccessMessage"] = "Login successful!";
                    return RedirectToAction("Index", "Contacts");
                }

                TempData["ErrorMessage"] = result.Message;
            }

            return View(model);
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            // If already logged in, redirect to contacts
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Index", "Contacts");

            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please login.";
                    return RedirectToAction(nameof(Login));
                }

                TempData["ErrorMessage"] = result.Message;
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Logged out successfully!";
            return RedirectToAction(nameof(Login));
        }

        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Please enter your email address.";
                return View();
            }

            var result = await _authService.ResetPasswordAsync(email);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Password reset email sent! Check your inbox.";
                return RedirectToAction(nameof(Login));
            }

            TempData["ErrorMessage"] = result.message;
            return View();
        }

        // GET: Account/Profile
        public IActionResult Profile()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction(nameof(Login));

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "User";
            
            return View();
        }
    }
}