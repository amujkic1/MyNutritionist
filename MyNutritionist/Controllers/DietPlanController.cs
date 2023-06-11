using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyNutritionist.Data;
using MyNutritionist.Models;

namespace MyNutritionist.Controllers
{
    //[Authorize]
    public class DietPlanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DietPlanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DietPlan
        [Authorize(Roles = "PremiumUser")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Handle the case when the user is not found
                return NotFound();
            }

            // Retrieve the diet plan for the signed-in user
            var dietPlan = _context.DietPlan
                                  .Include(d => d.Recipes)
                                  .FirstOrDefault(d => d.PremiumUser.Id == user.Id);
            if (dietPlan == null)
            {
                // Handle the case when the diet plan is not found
                return StatusCode(503, "Page is currently unavailable. Diet Plan will be availabe soon. Our nutritionist is working on that.");
            }

            return View(dietPlan);
        }

        // GET: DietPlan/Create
        [Authorize(Roles = "Nutritionist")]
        public IActionResult Create(string RegUser)
        {
            var dietPlan = new DietPlanViewModel();
            dietPlan.PremiumUserId = RegUser;

            return View(dietPlan);
        }

        // POST: DietPlan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Nutritionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string RegUser, [Bind("DietPlan,TotalCalories")] DietPlanViewModel dietPlanvm)
        {
            //if (ModelState.IsValid)
            {
                var dietPlan = dietPlanvm.DietPlan;


                for (var i = 0; i < 28; i++)
                {
                    if (dietPlan.Recipes[i].NameOfRecipe != null)
                    {
                        dietPlan.Recipes[i] = await _context.Recipe
                             .FirstOrDefaultAsync(r => r.NameOfRecipe == dietPlan.Recipes[i].NameOfRecipe);
                    }
                }

                var loggedInNutritionist = await _userManager.GetUserAsync(User);
                var user = await _context.Nutritionist.FindAsync(loggedInNutritionist.Id);
                dietPlan.PremiumUser = await _context.PremiumUser
                    .FirstOrDefaultAsync(m => m.Id.Equals(RegUser));

                var deletePlans = _context.DietPlan.Where(d => d.PremiumUser.Id == RegUser).ToList();
                if (deletePlans != null)
                {
                    _context.DietPlan.Remove(dietPlan);
                    _context.SaveChanges();
                }

                _context.Add(dietPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Nutritionist");
            }
            return View(dietPlanvm);
        }

    }
}
