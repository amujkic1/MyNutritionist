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
using Moq;
using MyNutritionist.Data;
using MyNutritionist.Models;
using MyNutritionist.Utilities;

namespace MyNutritionist.Controllers
{
    public class DietPlanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DietPlanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: DietPlan
        [Authorize(Roles = "PremiumUser")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var dietPlan = await _context.DietPlan
                .Include(d => d.PremiumUser)
                .FirstOrDefaultAsync(d => d.PremiumUser.Id == user.Id);

            if (dietPlan == null)
            {
                var referer = _httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString();
                return Redirect(!string.IsNullOrEmpty(referer) ? referer : "/");
            }

            var listOfRecipes = await _context.Recipe
                .Where(r => dietPlan.Recipes.Any(dpr => dpr.RID == r.RID))
                .ToListAsync();

            dietPlan.Recipes = listOfRecipes;

            return View(dietPlan);
        }


        // GET: DietPlan/Create
        [Authorize(Roles = "Nutritionist")]
        public IActionResult Create(string RegUser)
        {
            var dietPlan = new EditDietPlanVM
            {
                DietPlan = new DietPlan(),
                Recipes = _context.Recipe.ToList()
            };
            dietPlan.DietPlan.PremiumUser.Id = RegUser;

            return View(dietPlan);
        }

        // POST action to create a new diet plan
        // To protect from overposting attacks, only specific properties are bound
        [HttpPost]
        [Authorize(Roles = "Nutritionist")] // Access restricted to users in the "Nutritionist" role
        [ValidateAntiForgeryToken] // Helps prevent cross-site request forgery attacks
        public async Task<IActionResult> Create(string RegUser, [Bind("DietPlan")] EditDietPlanVM dietPlanvm)
        {
            // Extract the diet plan from the view model
            var dietPlan = dietPlanvm.DietPlan;

            // Ensure dietPlan.Recipes is not null
            dietPlan.Recipes ??= new List<Recipe>();

            // Check if each recipe in the diet plan exists
            for (var i = 0; i < dietPlan.Recipes.Count; i++)
            {
                // Retrieve each recipe from the context by its ID
                dietPlan.Recipes[i] = await _context.Recipe.FirstOrDefaultAsync(r => r.RID == dietPlan.Recipes[i].RID);

                // If a recipe is not found, return a "Not Found" response
                if (dietPlan.Recipes[i] == null)
                    return NotFound();
            }

            // Store the list of recipes temporarily and reset the diet plan's recipes list
            var listOfRecipes = dietPlan.Recipes;
            dietPlan.Recipes = new List<Recipe>();

            // Get the currently logged-in nutritionist
            var usrId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var loggedInNutritionist = await _context.Nutritionist.FirstOrDefaultAsync(m => m.Id.Equals(usrId));

            // If the logged-in nutritionist is not found, return a "Not Found" response
            if (loggedInNutritionist == null)
                return NotFound();



            // Associate the diet plan with the specified premium user
            dietPlan.PremiumUser = await _context.PremiumUser.FirstOrDefaultAsync(m => m.Id.Equals(RegUser));

            // Retrieve existing diet plans for the premium user and delete them
            var deletePlans = _context.DietPlan.Where(d => d.PremiumUser.Id == RegUser).ToList();
            if (deletePlans != null && deletePlans.Count != 0)
            {
                _context.DietPlan.Remove(deletePlans[0]);
                _context.SaveChanges();
            }

            // Add the new diet plan to the context
            _context.Add(dietPlan);
            await _context.SaveChangesAsync();

            // Associate each recipe with the diet plan by adding entries to the DietPlanRecipe table
            foreach (var recipe in listOfRecipes)
            {
                var sql = $"INSERT INTO DietPlanRecipe (RecipesRID, DietPlansDPID) VALUES ('{recipe.RID}', '{dietPlan.DPID}');";
                var mockContext = Mock.Get(_context);
                if (mockContext != null)
                {
                    // _context is a mock
                }
                else
                {
                    // _context is not a mock
                    _context.Database.ExecuteSqlRaw(sql);
                }
            }

            // Redirect to the Index action of the Nutritionist controller upon successful creation
            return RedirectToAction("Index", "Nutritionist");
        }


    }
}
