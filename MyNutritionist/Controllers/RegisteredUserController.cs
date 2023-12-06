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
    // Represents the controller for registered user actions
    public class RegisteredUserController : Controller
    {
        // Database context
        private readonly ApplicationDbContext _context;
        // Provides access to the HttpContext
        private readonly IHttpContextAccessor _httpContextAccessor;
        // Manages user-related operations
        private readonly UserManager<ApplicationUser> _userManager;
        // Manages sign-in operations
        private readonly SignInManager<ApplicationUser> _signInManager;
        // List to store ingredients 
        private List<Ingredient> _ingredients = new List<Ingredient>();

        // Constructor to initialize dependencies
        public RegisteredUserController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Async method to set progress data for a registered user
        private async Task<List<List<object>>> SetProgressData(RegisteredUser registeredUser)
        {

            // Current date information
            var currentDate = DateTime.Now.Date;
            var currentDayOfWeek = (int)currentDate.DayOfWeek;

            // Retrieve progress data for the last 7 days
            var progressList = await _context.Progress
                .Where(p => p.RegisteredUser.Id == registeredUser.Id && p.Date.Date >= currentDate.AddDays(-6).Date && p.Date.Date <= currentDate.Date)
                .OrderBy(p => p.Date)
                .ToListAsync();

            // Average consumed and burned calories values
            var averageConsumedCalories = 2000;
            var averageBurnedCalories = 300;

            // Lists to store consumed and burned calories progress data
            var consumedCaloriesProgressData = new List<object>();
            var burnedCaloriesProgressData = new List<object>();

            // Loop through the last 7 days to calculate progress data
            for (var i = 6; i >= 0; i--)
            {
                var date = currentDate.AddDays(-i);
                var dayOfWeek = (int)date.DayOfWeek;
                var progress = progressList.FirstOrDefault(p => p.Date.Date == date.Date);

                // Consumed and burned calories values for the day
                var consumedCalories = progress?.ConsumedCalories ?? 0;
                var burnedCalories = progress?.BurnedCalories ?? 0;

                // Calculate progress percentages
                var consumedCaloriesProgressPercentage = (int)CalculateProgressPercentage(consumedCalories, averageConsumedCalories);
                var burnedCaloriesProgressPercentage = (int)CalculateProgressPercentage(burnedCalories, averageBurnedCalories);
                var isSelectedDay = dayOfWeek == currentDayOfWeek;

                // Add progress data for the day to the respective lists
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

            // Create a list to store both consumed and burned calories progress data
            var list = new List<List<Object>>();
            list.Add(consumedCaloriesProgressData);
            list.Add(burnedCaloriesProgressData);
            return list;
        }

        // GET: RegisteredUser
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> Index()
        {
            // Retrieve the user ID of the currently logged-in user
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            // Retrieve the registered user information based on the user ID
            var registeredUser = await _context.RegisteredUser.FirstOrDefaultAsync(p => p.Id == userId);

            if (registeredUser == null)
            {
                return NotFound();
            }

            // Call SetProgressData to retrieve progress data for the user
            var list = await SetProgressData(registeredUser);

            // Set ViewBag properties for the consumed and burned calories progress data
            ViewBag.ConsumedCaloriesProgressData = list[0];
            ViewBag.BurnedCaloriesProgressData = list[1];

            return View(registeredUser);
        }

        // Method to calculate progress percentage based on actual and average values
        private double CalculateProgressPercentage(double value, double averageValue)
        {
            double progress = value / averageValue * 100;
            return progress > 100 ? 100 : (progress < 0 ? 0 : progress);
        }
        // GET: RegisteredUser/Details/
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> Details()
        {

            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(userId));
            if (registeredUser == null)
            {
                return NotFound();
            }

            // Call SetProgressData to retrieve progress data for the user
            var list = await SetProgressData(registeredUser);

            // Set ViewBag properties for the consumed and burned calories progress data
            ViewBag.ConsumedCaloriesProgressData = list[0];
            ViewBag.BurnedCaloriesProgressData = list[1];

            return View(registeredUser);
        }

        // Display nutritional values for ingredients
        [Authorize(Roles = "RegisteredUser, PremiumUser")]
        public IActionResult NutritionalValues()
        {
            // Retrieve the list of ingredients from the database
            var ingredient = _context.Ingredient.ToList();
            return View(ingredient);
        }

        // GET: RegisteredUser/EditCard/5
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> EditCard()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            // Retrieve the registered user information based on the user ID
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (registeredUser == null)
            {
                return NotFound();
            }

            // Return the view for editing the card
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> EditCard([Bind("Owner,CardNumber,Balance")] Card card)
        {
            // Retrieve the user ID of the currently logged-in user
            var usrId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(usrId));

            // Check if the card balance is sufficient for the transaction
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

            // Create a new PremiumUser instance and populate its properties with the registered user's information
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
            premiumUser.Height = registeredUser.Height;
            premiumUser.Weight = registeredUser.Weight;
            premiumUser.City = registeredUser.City;

            // Create a new Card instance and populate its properties
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

            var progressRecords = await _context.Progress
                        .Where(p => p.RegisteredUser.Id == registeredUser.Id)
                        .ToListAsync();

            if (progressRecords.Any())
            {
               
                // Associate the Progress records with the new PremiumUser
                foreach (var progressRecord in progressRecords)
                {
                    progressRecord.RegisteredUser = null;
                    progressRecord.PremiumUser = premiumUser;
                    _context.Progress.Update(progressRecord);
                }
                await _context.SaveChangesAsync();
            }

            // Remove the existing registered user from the database
            _context.RegisteredUser.Remove(registeredUser);
            await _context.SaveChangesAsync();

            // Add the new PremiumUser and Card to the database
            _context.PremiumUser.Add(premiumUser);
            await _context.SaveChangesAsync();
            _context.Card.Add(newCard);
            await _context.SaveChangesAsync();

            // Remove the RegisteredUser role and add the PremiumUser role to the user
            await _userManager.RemoveFromRoleAsync(registeredUser, "RegisteredUser");
            await _context.SaveChangesAsync();
            await _userManager.AddToRoleAsync(premiumUser, "PremiumUser");
            await _context.SaveChangesAsync();

            // Retrieve the updated user information
            var user = await _userManager.FindByNameAsync(registeredUser.UserName);

            // If the user is found, perform sign-in operations and redirect
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

            // Redirect to the default "Index" page if any issues occur
            return Redirect("Index");
        }

        // Actions for editing and managing user information

        // POST: RegisteredUser/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> Edit([Bind("City,Age,Weight,Height,FullName,Email,NutriUsername,NutriPassword")] RegisteredUser Reguser)
        {
            var usrId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var user = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(usrId));

            // Update user information based on provided values
            if (Reguser.NutriPassword != null)
            {
                await _userManager.ChangePasswordAsync(user, user.NutriPassword, Reguser.NutriPassword);
                user.NutriPassword = Reguser.NutriPassword;
            }

            // Update other user properties if values are provided
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
            if (Reguser.Weight != 0) user.Weight = Reguser.Weight;
            if (Reguser.Height != 0) user.Height = Reguser.Height;
            if (Reguser.City != null) user.City = Reguser.City;
            if (Reguser.Age != 0) user.Age = Reguser.Age;
            user.Id = usrId;

            // Update the user in the user manager
            var result = await _userManager.UpdateAsync(user);

            // Save changes to the database
            await _context.SaveChangesAsync();


            // Redirect to the Index action
            return RedirectToAction("Index");
        }


        // GET: RegisteredUser/Edit
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> Edit()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));

            if (registeredUser == null)
            {
                return NotFound();
            }

            // Return the view for editing user information
            return View(registeredUser);
        }


        // GET: RegisteredUser/Delete
        [ActionName("Delete")]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> Delete()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var registeredUser = await _context.RegisteredUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            if (registeredUser == null)
            {
                return NotFound();
            }

            // Redirect to the Index action of the Home controller
            return RedirectToAction("Index", "Home");
        }

        // POST: RegisteredUser/Delete/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            if (_context.RegisteredUser == null)
            {
                return Problem("Entity set 'ApplicationDbContext.RegisteredUser'  is null.");
            }

            var registeredUser = await _context.RegisteredUser.FindAsync(id);

            // If the registered user is found, remove associated progress entries and the user
           
                if (registeredUser != null)
                {
                    var progressListForRegisteredUser = await _context.Progress
                        .Where(p => p.RegisteredUser.Id == id)
                        .ToListAsync();

                    // Remove the progress entries
                    _context.Progress.RemoveRange(progressListForRegisteredUser);

                    // Remove the registered user
                    _context.RegisteredUser.Remove(registeredUser);
                }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the Index action of the Home controller
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "RegisteredUser, PremiumUser")]
        public IActionResult DailyFoodAndActivity()
        {
            // Initialize the view model for entering activity and food data
            var model = new EnterActivityAndFoodViewModel();

            // Retrieve the list of ingredients from the database
            model.Ingredients = _context.Ingredient.ToList();

            // Return the view with the initialized model
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "RegisteredUser, PremiumUser")]
        public async Task<IActionResult> Save(EnterActivityAndFoodViewModel model)
        {

            // Retrieve ingredient information for breakfast, lunch, dinner, and snacks
            var breakfastIngredient = _context.Ingredient.FirstOrDefault(i => i.FoodName == model.Breakfast.FoodName);
            var lunchIngredient = _context.Ingredient.FirstOrDefault(i => i.FoodName == model.Lunch.FoodName);
            var dinnerIngredient = _context.Ingredient.FirstOrDefault(i => i.FoodName == model.Dinner.FoodName);
            var snacksIngredient = _context.Ingredient.FirstOrDefault(i => i.FoodName == model.Snacks.FoodName);

            // Initialize quantities for breakfast, lunch, dinner, and snacks
            var breakfastQuantity = 0;
            var lunchQuantity = 0;
            var dinnerQuantity = 0;
            var snacksQuantity = 0;

            // Parse quantities from the request form
            if (int.TryParse(Request.Form["breakfast-quantity"], out breakfastQuantity) &&
                    int.TryParse(Request.Form["lunch-quantity"], out lunchQuantity) &&
                    int.TryParse(Request.Form["dinner-quantity"], out dinnerQuantity) &&
                    int.TryParse(Request.Form["snacks-quantity"], out snacksQuantity))
            {
                // Calculate total consumed calories
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


                // Check user role and calculate points accordingly
                if (_httpContextAccessor.HttpContext.User.IsInRole("RegisteredUser"))
                {
                    var currentUser = await _context.RegisteredUser.FirstOrDefaultAsync(u => u.Id == userId);

                    if (currentUser == null)
                    {
                        return NotFound();
                    }

                    // Create a new progress entry for the user
                    var progress = new Progress
                    {
                        Date = DateTime.Now,
                        BurnedCalories = 0,
                        ConsumedCalories = consumedCalories,
                        RegisteredUser = currentUser,
                        PremiumUser = null
                    };

                    // Calculate burned calories and points
                    progress.BurnedCalories = progress.CalculateBurnedCalories(model.PhysicalActivity);
                    var points = progress.CalculatePoints(consumedCalories, progress.BurnedCalories);

                    // Update user points and save changes to the database
                    currentUser.Points += points;
                    _context.SaveChanges();

                    // Add the progress entry to the database
                    _context.Progress.Add(progress);
                    _context.SaveChanges();
                }
                else
                {

                    // Retrieve premium user based on the user ID
                    var currentUser = await _context.PremiumUser.FirstOrDefaultAsync(u => u.Id == userId);

                    if (currentUser == null)
                    {
                        return NotFound();
                    }

                    // Create a new progress entry for the user
                    var progress = new Progress
                    {
                        Date = DateTime.Now,
                        BurnedCalories = 0,
                        ConsumedCalories = consumedCalories,
                        RegisteredUser = null,
                        PremiumUser = currentUser
                    };

                    // Calculate burned calories and points
                    progress.BurnedCalories = progress.CalculateBurnedCalories(model.PhysicalActivity);

                    var points = progress.CalculatePoints(consumedCalories, progress.BurnedCalories);

                    // Update user points and save changes to the database
                    currentUser.Points += points;
                    _context.SaveChanges();

                    // Add the progress entry to the database
                    _context.Progress.Add(progress);
                    _context.SaveChanges();

                }
            }


            // Redirect to the Index action of the Home controller
            return RedirectToAction("Index", "Home");
        }

    }
}
