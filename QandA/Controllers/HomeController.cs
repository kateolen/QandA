using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QandA.Data;
using QandA.Models;

namespace QandA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QAndAContext _dbContext;

        public HomeController(ILogger<HomeController> logger, QAndAContext qAndAContext)
        {
            _logger = logger;
            _dbContext = qAndAContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
           
            //Display questions
            List<Question> questions = _dbContext.Questions.Include(p => p.User).ToList();
            return View(questions);
           
        }

        [HttpGet]
        public IActionResult Ask()
        {
            return View();
        }
       
        [HttpPost]
        public IActionResult Ask(AskQuestionModel askQuestionModel)
        {
            //TODO: Save question to the database
            _dbContext.Questions.Add(new Question { UserId = 1, Text = askQuestionModel.Text, Title = askQuestionModel.Title, DateCreated = System.DateTime.Now});
            _dbContext.SaveChanges();
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
