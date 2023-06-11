using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyNutritionist.Data;
using MyNutritionist.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace MyNutritionist.Controllers
{
    [Authorize(Roles = "PremiumUser")]
    public class PremiumUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<ApplicationUser> _userManager;

        public PremiumUserController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // GET: PremiumUser
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.PremiumUser.Include(p => p.Nutritionist);
            return View(await _context.PremiumUser.ToListAsync());
        }

        // GET: PremiumUser/Details/5
        public async Task<IActionResult> Details()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (premiumUser == null)
            {
                return NotFound();
            }

            return View(premiumUser);
        }


        // GET: PremiumUser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            /*if (id == null || _context.PremiumUser == null)
            {
                return NotFound();
            }*/

            var premiumUser = await _context.PremiumUser.FindAsync(id);
            /*if (premiumUser == null)
            {
                return NotFound();
            }
            ViewData["NutritionistId"] = new SelectList(_context.Nutritionist, "PID", "PID", premiumUser.NutritionistId);*/
            return View(premiumUser);
        }

        // POST: PremiumUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountNumber,LeaderboardId,NutritionistId,City,Age,Weight,Height,Points,PID,Name,Email,Username,Password")] PremiumUser premiumUser)
        {
            /*if (id != premiumUser.PID)
            {
                return NotFound();
            }*/

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(premiumUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PremiumUserExists(premiumUser.Id))
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

            return View(premiumUser);
        }

        // GET: PremiumUser/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
           if (id == null || _context.PremiumUser == null)
            {
                return NotFound();
            }

            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (premiumUser == null)
            {
                return NotFound();
            }

            return View(premiumUser);
        }

        // POST: PremiumUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PremiumUser == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PremiumUser'  is null.");
            }
            var premiumUser = await _context.PremiumUser.FindAsync(id);
            if (premiumUser != null)
            {
                _context.PremiumUser.Remove(premiumUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PremiumUserExists(string id)
        {
          return (_context.PremiumUser?.Any(e => e.Id.Equals(id))).GetValueOrDefault();
        }
        // GET: PremiumUser/DailyFoodAndActivity/5
        public async Task<IActionResult> PremiumDailyActivityAndFood(int? id)
        {
            /*
            if (id == null || _context.PremiumUser == null)
            {
                return NotFound();
            }

        */
            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            /*  if (premiumUser == null)
              {
                  return NotFound();
              }*/

            return View(premiumUser);
        }

        [ActionName("Leaderboard")]
        // GET: PremiumUser/Leaderboard
        public async Task<IActionResult> Leaderboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(u => u.Id == currentUser.Id);

            if (premiumUser == null)
            {
                // Trenutno ulogovani korisnik nije premium korisnik
                return NotFound();
            }

            var usersFromSameCity = await _context.PremiumUser
                .Where(u => u.City == premiumUser.City)
                .OrderByDescending(u => u.Points)
                .ToListAsync();

            var leaderboard = new Leaderboard
            {
                Users = usersFromSameCity
            };

            return View(leaderboard);
        }
        public IActionResult DailyActivityAndFood()
        {
            var model = new EnterActivityAndFoodViewModel();
            return View("~/Views/PremiumUser/DailyActivityAndFood.cshtml", model);
        }

        [HttpPost]
        public IActionResult DailyActivityAndFood(EnterActivityAndFoodViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Ovdje dodajte logiku za spremanje podataka u bazu ili izvršavanje drugih potrebnih radnji

                return RedirectToAction("Index", "Home"); // Preusmjerite na odgovarajuću stranicu nakon uspješnog spremanja
            }

            return View("~/Views/PremiumUser/DailyActivityAndFood.cshtml", model);
        }
    }

}
