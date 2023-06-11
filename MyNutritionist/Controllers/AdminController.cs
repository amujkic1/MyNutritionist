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
        private string premiumUserName;
        
        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var premiumUsers = _context.PremiumUser.ToList();

            return View(premiumUsers);
        }

        [HttpGet]
        public async Task<IActionResult> AssignNutritionist(string premiumUserName)
        {
            ViewData["PremiumUserName"] = premiumUserName;
            var nutritionists = await _userManager.GetUsersInRoleAsync("Nutritionist");
            var users = nutritionists.Select(u => (Nutritionist)u).ToList();

            return View(nutritionists);
        }


        [HttpPost]
        public async Task<IActionResult> UpgradeToPremium(string nutriUserName, string premiumUserName)
        {
            
            if (string.IsNullOrEmpty(premiumUserName) || string.IsNullOrEmpty(nutriUserName))
            {
                return BadRequest("Invalid name.");
            }
            
            var premiumUser = await _context.PremiumUser.FirstOrDefaultAsync(u => u.UserName == premiumUserName);
            

            var nutritionist = await _context.Nutritionist
               .FirstOrDefaultAsync(m => m.NutriUsername.Equals(nutriUserName));
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
