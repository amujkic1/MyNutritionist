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
    [Authorize(Roles = "Nutritionist")]
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
            var loggedInNutritionist = await _userManager.GetUserAsync(User);
            var user = await _context.Nutritionist
                .Include(n => n.PremiumUsers) // Include the PremiumUsers list
                .FirstOrDefaultAsync(n => n.Id == loggedInNutritionist.Id);
            return View(user);
        }

        public async Task<IActionResult> SortByNames()
        {
            var loggedInNutritionist = await _userManager.GetUserAsync(User);
            var user = await _context.Nutritionist
                .Include(n => n.PremiumUsers) // Include the PremiumUsers list
                .FirstOrDefaultAsync(n => n.Id == loggedInNutritionist.Id);
            user.SortUsers(new SortByNames());
            return View("Index", user);
        }

        public async Task<IActionResult> SortByPoints()
        {
            var loggedInNutritionist = await _userManager.GetUserAsync(User);
            var user = await _context.Nutritionist
                .Include(n => n.PremiumUsers) // Include the PremiumUsers list
                .FirstOrDefaultAsync(n => n.Id == loggedInNutritionist.Id);
            user.SortUsers(new SortByPoints());
            return View("Index", user);
        }
    }


}
