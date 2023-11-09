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
    [Authorize(Roles = "RegisteredUser")]
    public class RegisteredUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private List<Ingredient> _ingredients = new List<Ingredient>();
        public RegisteredUserController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: RegisteredUser
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser.FirstOrDefaultAsync(p => p.Id == userId);

            if (registeredUser == null)
            {
                return NotFound();
            }

            var currentDate = DateTime.Now.Date;
            var currentDayOfWeek = (int)currentDate.DayOfWeek;

            var progressList = await _context.Progress
                .Where(p => p.RegisteredUser.Id == userId && p.Date.Year == currentDate.Year && p.Date.Month == currentDate.Month && p.Date.Day >= currentDate.AddDays(-6).Day && p.Date.Day <= currentDate.Day)
                .OrderBy(p => p.Date)
                .ToListAsync();


            var averageConsumedCalories = 2000;
            var averageBurnedCalories = 300;

            var consumedCaloriesProgressData = new List<object>();
            var burnedCaloriesProgressData = new List<object>();

            for (var i = 0; i < 7; i++)
            {
                var date = currentDate.AddDays(i);
                var dayOfWeek = (int)date.DayOfWeek;
                var progress = progressList.FirstOrDefault(p => p.Date.Year == date.Year && p.Date.Month == date.Month && p.Date.Day == date.Day);

                var consumedCalories = progress?.ConsumedCalories ?? 0;
                var burnedCalories = progress?.BurnedCalories ?? 0;

                var consumedCaloriesProgressPercentage = (int)CalculateProgressPercentage(consumedCalories, averageConsumedCalories);
                var burnedCaloriesProgressPercentage = (int)CalculateProgressPercentage(burnedCalories, averageBurnedCalories);
                var isSelectedDay = dayOfWeek == currentDayOfWeek;

                consumedCaloriesProgressData.Add(new
                {
                    DayOfWeek = date.ToString("ddd"),
                    HeightPercentage = consumedCaloriesProgressPercentage,
                    IsSelectedDay = isSelectedDay
                });

                burnedCaloriesProgressData.Add(new
                {
                    DayOfWeek = date.ToString("ddd"),
                    HeightPercentage = burnedCaloriesProgressPercentage,
                    IsSelectedDay = isSelectedDay
                });
            }

            ViewBag.ConsumedCaloriesProgressData = consumedCaloriesProgressData;
            ViewBag.BurnedCaloriesProgressData = burnedCaloriesProgressData;
            return View(registeredUser);
        }
        private double CalculateProgressPercentage(double value, double averageValue)
        {
            double progress = value / averageValue * 100;
            return progress > 100 ? 100 : (progress < 0 ? 0 : progress);
        }
        // GET: RegisteredUser/Details/
        public async Task<IActionResult> Details()
        {
       
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(userId));
              if (registeredUser == null)
              {
                  return NotFound();
              }
            var currentDate = DateTime.Now.Date;
            var currentDayOfWeek = (int)currentDate.DayOfWeek;

            var progressList = await _context.Progress
                .Where(p => p.RegisteredUser.Id == userId && p.Date.Year == currentDate.Year && p.Date.Month == currentDate.Month && p.Date.Day >= currentDate.AddDays(-6).Day && p.Date.Day <= currentDate.Day)
                .OrderBy(p => p.Date)
                .ToListAsync();


            var averageConsumedCalories = 2000;
            var averageBurnedCalories = 300;

            var consumedCaloriesProgressData = new List<object>();
            var burnedCaloriesProgressData = new List<object>();

            for (var i = 0; i < 7; i++)
            {
                var date = currentDate.AddDays(i);
                var dayOfWeek = (int)date.DayOfWeek;
                var progress = progressList.FirstOrDefault(p => p.Date.Year == date.Year && p.Date.Month == date.Month && p.Date.Day == date.Day);

                var consumedCalories = progress?.ConsumedCalories ?? 0;
                var burnedCalories = progress?.BurnedCalories ?? 0;

                var consumedCaloriesProgressPercentage = (int)CalculateProgressPercentage(consumedCalories, averageConsumedCalories);
                var burnedCaloriesProgressPercentage = (int)CalculateProgressPercentage(burnedCalories, averageBurnedCalories);
                var isSelectedDay = dayOfWeek == currentDayOfWeek;

                consumedCaloriesProgressData.Add(new
                {
                    DayOfWeek = date.ToString("ddd"),
                    HeightPercentage = consumedCaloriesProgressPercentage,
                    IsSelectedDay = isSelectedDay
                });

                burnedCaloriesProgressData.Add(new
                {
                    DayOfWeek = date.ToString("ddd"),
                    HeightPercentage = burnedCaloriesProgressPercentage,
                    IsSelectedDay = isSelectedDay
                });
            }

            ViewBag.ConsumedCaloriesProgressData = consumedCaloriesProgressData;
            ViewBag.BurnedCaloriesProgressData = burnedCaloriesProgressData;
            return View(registeredUser);
        }

        public IActionResult NutritionalValues()
        {
            Ingredient ingredient = new Ingredient(); // Create a new instance of the Ingredient model
            return View(ingredient);
        }



        [HttpPost]
        public IActionResult CheckNutritionalValues(Ingredient ingredient)
        {
            // Retrieve the ingredient from the database based on the entered name
            var queriedIngredient = _context.Ingredient
                .FirstOrDefault(i => i.FoodName == ingredient.FoodName);
            if(queriedIngredient==null)
            {
                return NotFound();
            }

            // Pass the queried ingredient to the view for display
            return View("NutritionalValues", queriedIngredient);
        }
        /*
        // GET: RegisteredUser/NutritionalValues/5
        public async Task<IActionResult> NutritionalValues(int? id)
        {
            
            if (id == null || _context.RegisteredUser == null)
            {
                return NotFound();
            }

        
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
              if (registeredUser == null)
              {
                  return NotFound();
              }

            return View(registeredUser);
        }
    */
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

            if (card.Balance < 50)
            {
                TempData["NotificationMessage"] = "Your card balance has to be 50 or above to finish this transaction.";
                return RedirectToAction("Index");
            }

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
            
            
            premiumUser.FullName = registeredUser.FullName;
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

            var user = await _userManager.FindByNameAsync(registeredUser.UserName);

            if (user != null)
            {
                // Sign out the user
                await _signInManager.SignOutAsync();

                // Generate a new security stamp (this invalidates existing tokens)
                await _userManager.UpdateSecurityStampAsync(user);

                // Sign in the user without requiring a password
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect to a page or perform any necessary actions after sign-in
                 return RedirectToAction("Index", "PremiumUser", new { area = "" });
            }
            return Redirect("Index");
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

            return RedirectToAction("Index", "Home");
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
            return RedirectToAction("Index", "Home");
        }

        // GET: RegisteredUser/DailyFoodAndActivity
        /*   public async Task<IActionResult> DailyFoodAndActivity()
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
           public IActionResult DailyActivityAndFood()
           {
               var model = new EnterActivityAndFoodViewModel();
               return View("~/Views/PremiumUser/DailyActivityAndFood.cshtml", model);
           }*/
        public IActionResult DailyFoodAndActivity()
        {
            var model = new EnterActivityAndFoodViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult DailyFoodAndActivity(EnterActivityAndFoodViewModel model)
        {
            if (ModelState.IsValid)
            {

                return RedirectToAction("Index", "Home"); 
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Save(EnterActivityAndFoodViewModel model)
        {
            var breakfastIngredient = _context.Ingredient.FirstOrDefault(i => i.FoodName == model.Breakfast.FoodName);
            var lunchIngredient = _context.Ingredient.FirstOrDefault(i => i.FoodName == model.Lunch.FoodName);
            var dinnerIngredient = _context.Ingredient.FirstOrDefault(i => i.FoodName == model.Dinner.FoodName);
            var snacksIngredient = _context.Ingredient.FirstOrDefault(i => i.FoodName == model.Snacks.FoodName);

            var breakfastQuantity = 0;
            var lunchQuantity = 0;
            var dinnerQuantity = 0;
            var snacksQuantity = 0;

            if (ModelState.IsValid)
            {
                if (int.TryParse(Request.Form["breakfast-quantity"], out breakfastQuantity) &&
                    int.TryParse(Request.Form["lunch-quantity"], out lunchQuantity) &&
                    int.TryParse(Request.Form["dinner-quantity"], out dinnerQuantity) &&
                    int.TryParse(Request.Form["snacks-quantity"], out snacksQuantity))
                {
                    var consumedCalories = 0;

                    if (breakfastIngredient != null)
                        consumedCalories += breakfastQuantity * breakfastIngredient.Calories;

                    if (lunchIngredient != null)
                        consumedCalories += lunchQuantity * lunchIngredient.Calories;

                    if (dinnerIngredient != null)
                        consumedCalories += dinnerQuantity * dinnerIngredient.Calories;

                    if (snacksIngredient != null)
                        consumedCalories += snacksQuantity * snacksIngredient.Calories;

                    var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);


                    var currentUser = await _context.RegisteredUser.FirstOrDefaultAsync(u => u.Id == userId);


                    if (currentUser == null)
                    {
                        return NotFound();
                    }

                    var burnedCalories = CalculateBurnedCalories(model.PhysicalActivity);
                    var points = CalculatePoints(consumedCalories, burnedCalories);

                  
                    currentUser.Points += points;
                    _context.SaveChanges();

                    var progress = new Progress
                    {
                        Date = DateTime.Now,
                        BurnedCalories = burnedCalories,
                        ConsumedCalories = consumedCalories,
                        RegisteredUser = currentUser,
                        PremiumUser = null
                    };

                    _context.Progress.Add(progress);
                    _context.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }
        private int CalculatePoints(int consumedCalories, int burnedCalories)
        {
            const int NormalDailyCalories = 2000;
            const int MaxPoints = 20;
            const int MinPoints = 0;

            var deviationPercentage = Math.Abs((consumedCalories - burnedCalories) / (double)NormalDailyCalories) * 100;
            var deviationPoints = MaxPoints - (int)Math.Round(deviationPercentage / 100 * MaxPoints);

            deviationPoints = Math.Max(MinPoints, Math.Min(MaxPoints, deviationPoints));

            return deviationPoints;
        }


        private int CalculateBurnedCalories(PhysicalActivity activity)
        {
            const int RunningCaloriesPerMinute = 10;
            const int WalkingCaloriesPerMinute = 5;
            const int CyclingCaloriesPerMinute = 8;

            var durationInMinutes = activity.Duration;

            switch (activity.ActivityType)
            {
                case ActivityType.RUNNING:
                    return durationInMinutes * RunningCaloriesPerMinute;

                case ActivityType.WALKING:
                    return durationInMinutes * WalkingCaloriesPerMinute;

                case ActivityType.CYCLING:
                    return durationInMinutes * CyclingCaloriesPerMinute;

                default:
                    return 0; 
            }
        }



    }
}
