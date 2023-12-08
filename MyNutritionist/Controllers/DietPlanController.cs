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
using MyNutritionist.Utilities;

namespace MyNutritionist.Controllers
{
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
                                  .Include(d => d.PremiumUser)
                                  .FirstOrDefault(d => d.PremiumUser.Id == user.Id);

            if (dietPlan == null)
            {
                // Redirect back to the previous URL if dietPlan is null
                return Redirect(HttpContext.Request.Headers["Referer"].ToString());
            }

            // Initialize an empty list to store recipes
            var listOfRecipes = new List<Recipe>();

            var sql = $"SELECT * FROM Recipe r INNER JOIN DietPlanRecipe dpr ON dpr.RecipesRID = r.RID WHERE dpr.DietPlansDPID = '{dietPlan.DPID}';";
            // Execute the SQL query and populate the listOfRecipes with the retrieved recipes
            listOfRecipes = _context.Recipe.FromSqlRaw(sql).ToList();

            // Assign the retrieved list of recipes to the diet plan's Recipes property
            dietPlan.Recipes = listOfRecipes;

            // Return the diet plan along with the associated list of recipes to the view
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
            var loggedInNutritionist = await _userManager.GetUserAsync(User);

            // If the logged-in nutritionist is not found, return a "Not Found" response
            if (loggedInNutritionist == null)
                return NotFound();

            // Find the nutritionist in the context
            var user = await _context.Nutritionist.FindAsync(loggedInNutritionist.Id);

            // If the nutritionist is not found, return a "Not Found" response
            if (user == null)
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
                _context.Database.ExecuteSqlRaw(sql);
            }

            // Redirect to the Index action of the Nutritionist controller upon successful creation
            return RedirectToAction("Index", "Nutritionist");
        }


    }
}
