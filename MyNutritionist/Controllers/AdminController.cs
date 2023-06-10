using System;
using System.Collections.Generic;
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

}









