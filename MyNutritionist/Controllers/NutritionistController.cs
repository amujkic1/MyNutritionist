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

        // GET: Nutritionist/Create
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

    }


}
