using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyNutritionist.Data;
using MyNutritionist.Models;

namespace MyNutritionist.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Nutritionist")]
        // GET: Recipes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TotalCalories,RecipeLink")] Recipe recipe)
        {
            var nutritionist = await _context.Nutritionist.FindAsync(11111);
            if (nutritionist == null)
            {
                return NotFound("Nutritionist not found.");
            }

            // Povežite nutricionista s receptom
            
            recipe = Activator.CreateInstance<Recipe>();
            //recipe.RID = 1;
            recipe.RecipeLink = "link";
            recipe.TotalCalories = 34;
            recipe.Nutritionist = nutritionist;
            var DietPlan = await _context.DietPlan
                .FirstOrDefaultAsync(m => m.DPID == 1);
            if (DietPlan == null)
            {
                return NotFound();
            }
            DietPlan.Recipes.Add(await _context.Recipe.FirstOrDefaultAsync(m => m.RID == 1));

            await _context.SaveChangesAsync();
            _context.Add(recipe);
            await _context.SaveChangesAsync();

            return View(recipe);
        }

        [Authorize(Roles = "Nutritionist")]
        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Recipe == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RID,RecipeLink,TotalCalories,DietPlanID")] Recipe recipe)
        {
            if (id != recipe.RID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.RID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        private bool RecipeExists(int id)
        {
          return (_context.Recipe?.Any(e => e.RID == id)).GetValueOrDefault();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecipe([Bind("TotalCalories,RecipeLink")] Recipe recipe)
        {
            var nutritionist = await _context.Nutritionist.FindAsync(11111);
            if (nutritionist == null)
            {
                return NotFound("Nutritionist not found.");
            }

            recipe.Nutritionist = nutritionist;
            var DietPlan = await _context.DietPlan.FirstOrDefaultAsync(m => m.DPID == 1);
            if (DietPlan == null)
            {
                return NotFound();
            }
            DietPlan.Recipes.Add(recipe);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }

}

