using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            //_httpContextAccessor = httpContextAccessor;
            //_roleManager = roleManager;, RoleManager<IdentityRole> roleManager
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            //var registeredUsers = _userManager.GetUsersInRoleAsync("RegisteredUser").Result;
            //var idsOfRegisteredUsers = registeredUsers.Select(u => u.Id);
            //var users = registeredUsers.OfType<ApplicationUser>();
            return _context.Admin != null ?
                        View(await _context.Admin.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.RegisteredUser'  is null.");

            //return View(user);
        }
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

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }

        }

        private bool AdminExists(string id)
        {
          return (_context.Admin?.Any(e => e.Id.Equals(id))).GetValueOrDefault();
        }
    }
    

}









