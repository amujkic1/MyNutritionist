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

        public RecipesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Nutritionist")]
        // GET: Recipes/Create
        public IActionResult Create()
        {
            var recipeViewModel = new RecipeViewModel();
            recipeViewModel.input = new Recipe();
            recipeViewModel.recipesToDisplay = _context.Recipe.ToList();
            return View(recipeViewModel);
        }


        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Nutritionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("input")] RecipeViewModel recipeViewModel)
        {
                var loggedInNutritionist = await _userManager.GetUserAsync(User);
                var nutritionist = await _context.Nutritionist.FindAsync(loggedInNutritionist.Id);
                if (nutritionist == null)
                {
                    return NotFound("Nutritionist not found.");
                }

                var newRecipe = new Recipe
                {
                    NameOfRecipe = recipeViewModel.input.NameOfRecipe,
                    RecipeLink = recipeViewModel.input.RecipeLink,
                    TotalCalories = recipeViewModel.input.TotalCalories,
                    Nutritionist = nutritionist
                };

                _context.Add(newRecipe);
                await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Nutritionist");
        }
    }

}

