using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleBassedAuthentication.DTO;
using RoleBassedAuthentication.interfaces;

namespace RoleBassedAuthentication.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthencticationUser _auth;
        public AuthenticationController(IAuthencticationUser auth)
        {
            _auth = auth;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginUser model)
        {
            if (!ModelState.IsValid)
            {
            return View(model);

            }
            var result = await _auth.LoginAsync(model);
            if(result.StatusCode== 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["msg"] = result.Message;
                return RedirectToAction(nameof(Login));
            }
        }

        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUser model)
        {
            if (!ModelState.IsValid) { return View(model); }
            model.Role = "user";
            var result = await _auth.RegisterAsync(model);
            TempData["msg"] = result.Message;
            return RedirectToAction(nameof(RegisterUser));
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin()
        {
            var model = new RegisterUser
            {
                Username = "admin",
                FirstName = "Amit",
                LastName = "Mahi",
                Email = "eramit@gmail.com",
                Password = "Admin@123"

            };
            model.Role = "admin";

            var result = await _auth.RegisterAsync(model);
            return Ok(result);
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]

        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (!ModelState.IsValid) { return View(model); }

            var result = await _auth.ChangePasswordAsync(model, User.Identity.Name);
            TempData["msg"] = result.Message;
            return RedirectToAction("Index","Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _auth.LogoutAsync();
            return RedirectToAction("Login","Authentication");
        }
    }
}
