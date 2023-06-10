using System;
using System.Collections.Generic;
using System.Drawing.Text;
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
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var registeredUsers = _userManager.GetUsersInRoleAsync("RegisteredUser").Result;
            var idsOfRegisteredUsers = registeredUsers.Select(u => u.Id);
            var users = registeredUsers.OfType<ApplicationUser>();


            return View(users);
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PID,Name,Email,Username,Password")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PID,Name,Email,Username,Password")] Admin admin)
        {
            if (id != admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id))
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
            return View(admin);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Admin == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Admin'  is null.");
            }
            var admin = await _context.Admin.FindAsync(id);
            if (admin != null)
            {
                _context.Admin.Remove(admin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpgradeToPremium(string regName)
        {
            if (string.IsNullOrEmpty(regName))
            {
                return BadRequest("Invalid name.");
            }

            var registeredUser = await _context.RegisteredUser.FirstOrDefaultAsync(u => u.FullName == regName);
            if (registeredUser == null)
            {
                return NotFound("Registered user not found.");
            }

            try {
                var premiumUser = Activator.CreateInstance<PremiumUser>();

                premiumUser.AccountNumber = "";
                premiumUser.City = registeredUser.City;
                premiumUser.Height = registeredUser.Height;
                premiumUser.Weight = registeredUser.Weight;
                premiumUser.Age = registeredUser.Age;
                premiumUser.Points = 0;
                premiumUser.AspUserId = 0;

                _context.PremiumUser.Add(premiumUser);

                await _userManager.RemoveFromRoleAsync(registeredUser, "RegisteredUser");
                await _userManager.AddToRoleAsync(registeredUser, "PremiumUser");

                await _context.SaveChangesAsync();

                var nutritionist = await _context.Nutritionist
               .FirstOrDefaultAsync(m => m.NutriUsername.Equals("nutri123"));
                if (nutritionist == null)
                {
                    return NotFound();
                }

                nutritionist.PremiumUsers.Add(await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(registeredUser.Id)));

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }

        }

        public async Task<IActionResult> AssignNutritionist(int regId)
        {
            var nutritionist = await _context.Nutritionist
               .FirstOrDefaultAsync(m => m.NutriUsername.Equals("nutri123"));
            if (nutritionist == null)
            {
                return NotFound();
            }

            nutritionist.PremiumUsers.Add(await _context.PremiumUser
            .FirstOrDefaultAsync(m => m.Id.Equals(regId)));

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(string id)
        {
          return (_context.Admin?.Any(e => e.Id.Equals(id))).GetValueOrDefault();
        }
    }
    

}









