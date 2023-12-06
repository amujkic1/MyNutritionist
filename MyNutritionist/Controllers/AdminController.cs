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

        //Constructor
        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            // Retrieve premium users from the database
            var premiumUsers = _context.PremiumUser.ToList();

            // Pass premium users to the view
            return View(premiumUsers);
        }

        // GET: AssignNutritionist action
        [HttpGet]
        public async Task<IActionResult> AssignNutritionist(string premiumUserName)
        {
            // Pass premium user name to the view
            ViewData["PremiumUserName"] = premiumUserName;

            // Retrieve nutritionists from the 'Nutritionist' role
            var nutritionists = await _userManager.GetUsersInRoleAsync("Nutritionist");
            var users = nutritionists.Select(u => (Nutritionist)u).ToList();

            // Pass nutritionists to the view
            return View(nutritionists);
        }

        // POST: UpgradeToPremium action
        [HttpPost]
        public async Task<IActionResult> UpgradeToPremium(string nutriUserName, string premiumUserName)
        {

            if (string.IsNullOrEmpty(premiumUserName) || string.IsNullOrEmpty(nutriUserName))
            {
                return BadRequest("Invalid name.");
            }

            // Retrieve the premium user from the database
            var premiumUser = await _context.PremiumUser.FirstOrDefaultAsync(u => u.UserName == premiumUserName);

            // Retrieve the nutritionist from the database
            var nutritionist = await _context.Nutritionist
               .FirstOrDefaultAsync(m => m.NutriUsername.Equals(nutriUserName));

            if (nutritionist == null)
            {
                return NotFound();
            }

            // Associate the premium user with the nutritionist
            nutritionist.PremiumUsers.Add(premiumUser);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the Admin Index action
            return RedirectToAction(nameof(Index));

        }
    }

}
