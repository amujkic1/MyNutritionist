﻿using System;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NutritionistController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // GET: Nutritionist
        public async Task<IActionResult> Index()
        {
            var loggedInNutritionistId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var user = await _context.Nutritionist
                            .Include(n => n.PremiumUsers) // Include the PremiumUsers list
                            .FirstOrDefaultAsync(n => n.Id == loggedInNutritionistId);
            
            if (user == null)
            {
                // Slučaj kada niko nije ulogovan
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("FullName,Email,NutriUsername,Image")] Nutritionist Reguser)
        {
            if (Reguser == null)
            {
                return NotFound();
            }

            var usrId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var user = await _context.Nutritionist.FirstOrDefaultAsync(m => m.Id.Equals(usrId));

            if (user == null)
            {
                // Slučaj kada nutricionista nije pronađen
                return NotFound();
            }

            if (Reguser.FullName != null) user.FullName = Reguser.FullName;
            if (Reguser.Email != null)
            {
                user.EmailAddress = Reguser.Email;
                user.Email = Reguser.Email;
            }
            if (Reguser.NutriUsername != null)
            {
                user.UserName = Reguser.NutriUsername;
                user.NutriUsername = Reguser.NutriUsername;
            }
            if(Reguser.Image != null)
            {
                user.Image = Reguser.Image;
            }
            user.Id = usrId;

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Nutritionist/Edit
        public async Task<IActionResult> Edit()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var nutritionist = await _context.Nutritionist
                .FirstOrDefaultAsync(m => m.Id.Equals(id));

            if (nutritionist == null)
            {
                return NotFound();
            }

            return View(nutritionist);
        }

        public async Task<IActionResult> SortByNames()
        {
            var loggedInNutritionistId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var user = await _context.Nutritionist
                .Include(n => n.PremiumUsers) // Include the PremiumUsers list
                .FirstOrDefaultAsync(n => n.Id == loggedInNutritionistId);

            if (user == null)
            {
                return NotFound();
            }
            user.SortUsers(new SortByNames(), (x, y) => string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase));
            return View("Index", user);
        }

        // Funkcija za sortiranje premium korisnika koji su povezani sa ulogovanim nutricionistom po kriteriju osvojenih bodova
        public async Task<IActionResult> SortByPoints()
        {
            var loggedInNutritionistId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var user = await _context.Nutritionist
                .Include(n => n.PremiumUsers) // Include the PremiumUsers list
                .FirstOrDefaultAsync(n => n.Id == loggedInNutritionistId);

            if(user == null)
            {
                return NotFound();
            }
            user.SortUsers(new SortByPoints(), (x, y) => y.Points - x.Points);
            return View("Index", user);
        }
    }

}