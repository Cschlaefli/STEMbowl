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
using Microsoft.EntityFrameworkCore;


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
                .Where(e => String.IsNullOrEmpty(e.Answer))
                .FirstOrDefault();

            if (question == null)
            { 
                return RedirectToAction("EndOfRound"); 
            }
            return View(question);
        }

        public IActionResult EndOfRound()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(TeamAnswers answers)
        {
            _context.TeamAnswers.Update(answers);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}