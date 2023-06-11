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
using Microsoft.CodeAnalysis.Rename;
using Microsoft.EntityFrameworkCore;
using MyNutritionist.Data;
using MyNutritionist.Models;

namespace MyNutritionist.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var premiumUsers = await _userManager.GetUsersInRoleAsync("PremiumUser");
            var users = premiumUsers.Select(u => (PremiumUser)u).ToList();

            return View(premiumUsers);
        }

        public async Task<IActionResult> UpgradeToPremium(string userName)
        {
            
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("Invalid name.");
            }
            var premiumUser = await _context.PremiumUser.FirstOrDefaultAsync(u => u.UserName == userName);

            var nutritionist = await _context.Nutritionist
               .FirstOrDefaultAsync(m => m.NutriUsername.Equals("nutri123"));
                if (nutritionist == null)
                {
                    return NotFound();
                }
            
                nutritionist.PremiumUsers.Add(premiumUser);
            
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

        }
    }

}
