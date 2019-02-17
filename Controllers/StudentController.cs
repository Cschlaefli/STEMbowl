using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using stembowl.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Authentication;
using System.Security.Principal;
using Shared.Web.MvcExtensions;
using stembowl.Areas.Identity;


namespace stembowl.Controllers
{
    public class StudentController : Controller
    {

        QuestionDbContext _context;
        UserManager<ApplicationUser> _userContext;

        public StudentController(QuestionDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userContext = userManager;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userContext.GetUserAsync(User);
            var question = _context.TeamAnswers
                .Where(e => e.TeamID == user.TeamID)
                .Include(e => e.Question)
                .ThenInclude(q => q.Answers)
                .Where(e => String.IsNullOrEmpty(e.Answer))
                .FirstOrDefault();

            if (question == null)
            { 
                return RedirectToAction("EndOfRound"); 
            }
            return View(question);
        }

        [HttpPost]
        public IActionResult Index(TeamAnswers answers)
        {
            var correct = _context.Answer
                .Where(e => e.QuestionID == answers.QuestionID && e.Correct)
                .FirstOrDefault();

            if (answers.Answer.Equals(correct.Text))
                answers.Correct = true;
            else
                answers.Correct = false;

            _context.TeamAnswers.Update(answers);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult EndOfRound()
        {
            return View();  
        }

        public async Task<IActionResult> CheckScore()
        {
            double score = 0;
            
            var user = await _userContext.GetUserAsync(User);
            var questions = _context.TeamAnswers
                .Where(e => e.TeamID == user.TeamID);
                
            foreach(var question in questions)
            {
                if (question.Correct)
                score++;
            }

            double percentage = (score / questions.Count()) * 100 ;

            return View(percentage);  
        }

        


    }
}