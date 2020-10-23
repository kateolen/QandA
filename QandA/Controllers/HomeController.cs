using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QandA.Data;
using QandA.Models;

namespace QandA.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataService _dataService;


        public HomeController(ILogger<HomeController> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        [HttpPost]
        public IActionResult Search([FromForm]string search)
        {
           return RedirectToAction(nameof(Index), new { search });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index([FromQuery]string search)
        {
            //Display questions
            List<Question> questions = new List<Question>();

            if(search != null && search.Length > 2)
            {
                questions = _dataService.GetFilteredQuestions(search);
            }
            else
            {
               questions = _dataService.GetQuestions();
            }
           

            List<QuestionAnswerViewModel> viewModel = questions.Select(q => new QuestionAnswerViewModel
            {
                Id = q.Id,
                DateCreated = q.DateCreated,
                Text = q.Text,
                Title = q.Title,
                UserName = q.User.UserName
            }).ToList();

            return View("AllQuestions", viewModel);
        }

        [HttpGet]
        public IActionResult Ask()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ask(AskQuestionViewModel askQuestionModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            User user = _dataService.GetUser(this.User.Identity.Name);

            var q = new Question
            {
                UserId = user.Id,
                Text = askQuestionModel.Text,
                Title = askQuestionModel.Title,
                DateCreated = System.DateTime.Now
            };

            _dataService.AddQuestion(q);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Answers(int id)
        {
            return DisplayQuestionAndAnswersView(id);
        }

        [HttpPost]
        public IActionResult Answers(QuestionAnswersViewModel qm)
        {
            if (!ModelState.IsValid)
            {
                return DisplayQuestionAndAnswersView(qm.Question.Id);
            }

            User user = _dataService.GetUser(this.User.Identity.Name);
            Answer answer = new Answer
            {
                QuestionId = qm.Question.Id,
                UserId = user.Id,
                DateCreated = DateTime.Now,
                Text = qm.Answer
            };

            _dataService.AddAnswer(answer);

            return RedirectToAction("Answers", new { id = qm.Question.Id });
        }

        private IActionResult DisplayQuestionAndAnswersView(int id)
        {
            Question q = _dataService.GetQuestionWithAnswers(id);
            QuestionAnswersViewModel qm = new QuestionAnswersViewModel
            {
                Question = new QuestionAnswerViewModel
                {
                    Id = q.Id,
                    DateCreated = q.DateCreated,
                    Text = q.Text,
                    Title = q.Title,
                    UserName = q.User.UserName
                },
                Answers = q.Answers.Select(a => new QuestionAnswerViewModel
                {
                    Text = a.Text,
                    UserName = a.User.UserName,
                    DateCreated = a.DateCreated
                }).ToList(),
            };
            return View(qm);
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
