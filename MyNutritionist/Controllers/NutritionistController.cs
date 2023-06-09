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

namespace MyNutritionist.Controllers
{
    [Authorize]
    public class NutritionistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NutritionistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Nutritionist
        public async Task<IActionResult> Index()
        {
            var premiumUsers = _userManager.GetUsersInRoleAsync("PremiumUser").Result;
            var idsOfPremiumUsers = premiumUsers.Select(u => u.Id);
            var users = premiumUsers.OfType<ApplicationUser>();


            return View(users);

            /*return _context.Nutritionist != null ? 
                        View(await _context.Nutritionist.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Nutritionist'  is null.");*/
        }

        // GET: Nutritionist/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Nutritionist == null)
            {
                return NotFound();
            }

            var nutritionist = await _context.Nutritionist
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (nutritionist == null)
            {
                return NotFound();
            }

            return View(nutritionist);
        }

        // GET: Nutritionist/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nutritionist/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Email,NutriUsername,NutriPassword")] ApplicationUser nutritionist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nutritionist);
                await _context.SaveChangesAsync();
                nutritionist.EmailConfirmed = true;
                await _userManager.AddToRoleAsync(nutritionist, "Nutritionist");
                
                return RedirectToAction(nameof(Index));
            }
            return View(nutritionist);
        }

        // GET: Nutritionist/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Nutritionist == null)
            {
                return NotFound();
            }

            var nutritionist = await _context.Nutritionist.FindAsync(id);
            if (nutritionist == null)
            {
                return NotFound();
            }
            return View(nutritionist);
        }

        // POST: Nutritionist/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PID,Name,Email,Username,Password")] Nutritionist nutritionist)
        {
            if (!id.Equals(nutritionist.Id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nutritionist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NutritionistExists(nutritionist.Id))
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
            return View(nutritionist);
        }

        // GET: Nutritionist/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Nutritionist == null)
            {
                return NotFound();
            }

            var nutritionist = await _context.Nutritionist
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (nutritionist == null)
            {
                return NotFound();
            }

            return View(nutritionist);
        }

        // POST: Nutritionist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Nutritionist == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Nutritionist'  is null.");
            }
            var nutritionist = await _context.Nutritionist.FindAsync(id);
            if (nutritionist != null)
            {
                _context.Nutritionist.Remove(nutritionist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NutritionistExists(string id)
        {
          return (_context.Nutritionist?.Any(e => e.Id.Equals(id))).GetValueOrDefault();
        }
        // GET: Nutritionist/AddRecipe/5
        public async Task<IActionResult> AddRecipe(int? id)
        {
            /*
            if (id == null || _context.Nutritionist == null)
            {
                return NotFound();
            }

        */
            var nutritionist = await _context.Nutritionist
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            /*  if (nutritionist == null)
              {
                  return NotFound();
              }*/

            return View(nutritionist);
        }
        // GET: RegisteredUser/EditCard/5
        
    }


}
