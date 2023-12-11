using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor for the RecipesController class that takes ApplicationDbContext and UserManager as dependencies
        public RecipesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Recipes/Create
        [Authorize(Roles = "Nutritionist")]
        // Action to display the page for creating a recipe
        public IActionResult Create()
        {
            // Creating a ViewModel object used to transfer data between the controller and the view
            var recipeViewModel = new RecipeViewModel();
            recipeViewModel.input = new Recipe();

            // Check if _context and _context.Recipe are not null before calling ToList
            if (_context != null && _context.Recipe != null)
            {
                recipeViewModel.recipesToDisplay = _context.Recipe.ToList();
            }
            else
            {
                // Handle the case where _context or _context.Recipe is null
                // You can log a message or take appropriate action
                // For now, you can set it to an empty list or handle it based on your requirements
                recipeViewModel.recipesToDisplay = new List<Recipe>();
            }

            return View(recipeViewModel);
        }


        // POST: Recipes/Create
        // Method that handles the HTTP POST request for creating a new recipe
        // Uses the [Bind] attribute to prevent over-posting by binding only desired data
        [HttpPost]
        [Authorize(Roles = "Nutritionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("input")] RecipeViewModel recipeViewModel)
        {
            // Getting the currently logged-in nutritionist
            var loggedInNutritionist = await _userManager.GetUserAsync(User);

            // Searching for the nutritionist in the database
            var nutritionist = await _context.Nutritionist.FindAsync(loggedInNutritionist.Id);

            // Checking if the nutritionist is found
            if (nutritionist == null)
            {
                return NotFound("Nutritionist not found.");
            }

            // Creating a new recipe based on the data entered through the ViewModel
            var newRecipe = new Recipe
            {
                NameOfRecipe = recipeViewModel.input.NameOfRecipe,
                RecipeLink = recipeViewModel.input.RecipeLink,
                TotalCalories = recipeViewModel.input.TotalCalories,
                Nutritionist = nutritionist
            };

            // Adding the new recipe to the database
            _context.Add(newRecipe);
            await _context.SaveChangesAsync();

            // Redirecting to the Index action of the Nutritionist controller after successfully adding the recipe
            return RedirectToAction("Index", "Nutritionist");
        }
    }
}
