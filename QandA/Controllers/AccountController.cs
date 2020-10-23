using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QandA.Data;
using QandA.Models;

namespace QandA.Controllers
{
    public class AccountController : Controller
    {
        private readonly IDataService _dataService;
        public AccountController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // account/signin
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            { 
                return View();
            }
            if (!_dataService.UserExists(userViewModel.UserName))
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
                return View();
            }

            User dbUser = _dataService.GetUser(userViewModel.UserName);

            bool samePassword = BCrypt.Net.BCrypt.Verify(userViewModel.Password, dbUser.Password);

            if (!samePassword)
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
                return View();
            }

            var claims = new List<Claim>();
            var userNameClaim = new Claim(ClaimTypes.Name, userViewModel.UserName);
            claims.Add(userNameClaim);
            var userIdentity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterUserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (userViewModel.Password != userViewModel.ConfirmPassword)
            {
                ModelState.AddModelError(String.Empty, "Password Doesn't Match!");
                return View();
            }
            if (_dataService.UserExists(userViewModel.UserName))
            {
                ModelState.AddModelError(String.Empty, "UserName already exists");
                return View();
            }

            string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(userViewModel.Password);

            var user = new User
            {
                UserName = userViewModel.UserName,
                Password = encryptedPassword
            };
            _dataService.AddUser(user);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult SignOut()
        {
            return RedirectToAction("SignIn", "Account");
        }
    }
}
