using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace QandA.Controllers
{
    public class AccountController : Controller
    {
        // account/signin
        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult Register() 
        {
            return View();
        }
    }
}
