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
    [Authorize(Roles = "RegisteredUser")]
    public class RegisteredUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private List<Ingredient> _ingredients = new List<Ingredient>();
        public RegisteredUserController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // GET: RegisteredUser
        public async Task<IActionResult> Index()
        {
            return _context.RegisteredUser != null ?
                        View(await _context.RegisteredUser.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.RegisteredUser'  is null.");
        }

        // GET: RegisteredUser/Details/
        public async Task<IActionResult> Details()
        {
       
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
              if (registeredUser == null)
              {
                  return NotFound();
              }

            return View(registeredUser);
        }
        // GET: RegisteredUser/NutritionalValues/5
        public async Task<IActionResult> NutritionalValues(int? id)
        {
            /*
            if (id == null || _context.RegisteredUser == null)
            {
                return NotFound();
            }

        */
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            /*  if (registeredUser == null)
              {
                  return NotFound();
              }*/

            return View(registeredUser);
        }

        // GET: RegisteredUser/EditCard/5
        public async Task<IActionResult> EditCard()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
              if (registeredUser == null)
              {
                  return NotFound();
              }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCard([Bind("Owner,CardNumber,Balance")] Card card)
        {
            //var user = Activator.CreateInstance<RegisteredUser>();
            var usrId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(usrId));
            

            if (registeredUser == null)
            {
                return NotFound("Registered user not found.");
            }


            var premiumUser = new PremiumUser();
            try
            {
                premiumUser = Activator.CreateInstance<PremiumUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
            
            //premiumUser.AccountNumber = ;
            premiumUser.City = registeredUser.City;
            premiumUser.Height = registeredUser.Height;
            premiumUser.Weight = registeredUser.Weight;
            premiumUser.Age = registeredUser.Age;
            premiumUser.Points = 0;
            premiumUser.AspUserId = "null";
            premiumUser.Age = registeredUser.Age;
            premiumUser.UserName = registeredUser.UserName;
            premiumUser.PasswordHash = registeredUser.PasswordHash;
            premiumUser.NutriPassword = registeredUser.NutriPassword;
            premiumUser.NutriUsername = registeredUser.NutriUsername;
            premiumUser.EmailAddress = registeredUser.Email;
            premiumUser.Email = registeredUser.Email;
            premiumUser.EmailConfirmed = true;

            var newCard = new Card();
            try
            {
                newCard = Activator.CreateInstance<Card>();
                newCard.PremiumUser = premiumUser;
                newCard.Balance = card.Balance;
                newCard.CardNumber = card.CardNumber;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }

            _context.RegisteredUser.Remove(registeredUser);
            await _context.SaveChangesAsync();
            _context.PremiumUser.Add(premiumUser);
            await _context.SaveChangesAsync();

            _context.Card.Add(newCard);
            await _context.SaveChangesAsync();

            await _userManager.RemoveFromRoleAsync(registeredUser, "RegisteredUser");
            await _context.SaveChangesAsync();
            await _userManager.AddToRoleAsync(premiumUser, "PremiumUser");
           
            

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("City,Age,Weight,Height,FullName,Email,NutriUsername,NutriPassword")] RegisteredUser Reguser)
        {
            //var user = Activator.CreateInstance<RegisteredUser>();
            var usrId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var user = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(usrId));
             if (Reguser.NutriPassword != null) {
                await _userManager.ChangePasswordAsync(user, user.NutriPassword, Reguser.NutriPassword);
                user.NutriPassword = Reguser.NutriPassword;
            }
             if(Reguser.FullName != null) user.FullName = Reguser.FullName;
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
             if (Reguser.Weight != 0) user.Weight = Reguser.Weight;
             if (Reguser.Height != 0) user.Height = Reguser.Height;
             if (Reguser.City != null) user.City = Reguser.City;
             if (Reguser.Age != 0) user.Age = Reguser.Age;
             user.Id = usrId;

            var result = await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // GET: RegisteredUser/Edit
        public async Task<IActionResult> Edit()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            
            if (registeredUser == null)
            {
                return NotFound();
            }
            
            return View(registeredUser);
        }


        // GET: RegisteredUser/Delete
        [ActionName("Delete")]
        public async Task<IActionResult> Delete()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (registeredUser == null)
            {
                return NotFound();
            }

            return View(registeredUser);
        }

        // POST: RegisteredUser/Delete/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            if (_context.RegisteredUser == null)
            {
                return Problem("Entity set 'ApplicationDbContext.RegisteredUser'  is null.");
            }
            var registeredUser = await _context.RegisteredUser.FindAsync(id);
            if (registeredUser != null)
            {
                _context.RegisteredUser.Remove(registeredUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: RegisteredUser/DailyFoodAndActivity
        public async Task<IActionResult> DailyFoodAndActivity()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
              if (registeredUser == null)
              {
                  return NotFound();
              }

            return View(registeredUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DailyFoodAndActivity([Bind("Breakfast,Lunch,Dinner,Snacks,PhysicalActivity")] EnterActivityAndFoodViewModel input)
        {
            var usrId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var user = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(usrId));

            

            return RedirectToAction("Index");
        }

    }
}
