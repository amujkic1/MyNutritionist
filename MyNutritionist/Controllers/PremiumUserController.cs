using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System.Globalization;

namespace MyNutritionist.Controllers
{
    [Authorize(Roles = "PremiumUser")]
    public class PremiumUserController : Controller
    {
        // Declaring private fields to hold references to necessary dependencies
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // Constructor to initialize the PremiumUserController with required dependencies
        public PremiumUserController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            // Assigning the provided instances of dependencies to corresponding private fields
            _context = context; 
            _httpContextAccessor = httpContextAccessor;  
            _userManager = userManager;
            _signInManager = signInManager;
        }


        async Task<List<List<object>>> SetProgressData(PremiumUser premiumUser)
        {

            // Current date information
            var currentDate = DateTime.Now.Date;
            var currentDayOfWeek = (int)currentDate.DayOfWeek;

            // Retrieve progress data for the last 7 days
            var progressList = await _context.Progress
                .Where(p => p.PremiumUser.Id == premiumUser.Id && p.Date.Date >= currentDate.AddDays(-6).Date && p.Date.Date <= currentDate.Date)
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
                    DayOfWeek = date.ToString("ddd", new CultureInfo("en-US")),
                    HeightPercentage = consumedCaloriesProgressPercentage,
                    IsSelectedDay = isSelectedDay
                });

                burnedCaloriesProgressData.Add(new
                {
                    DayOfWeek = date.ToString("ddd", new CultureInfo("en-US")),
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


        // Action method for the PremiumUser's index page
        public async Task<IActionResult> Index()
        {
            // Retrieves the current user's ID from the HttpContext User using UserManager
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            // Retrieves the PremiumUser associated with the user ID from the database
            var premiumUser = await _context.PremiumUser.FirstOrDefaultAsync(p => p.Id == userId);

            // Checks if the PremiumUser is not found
            if (premiumUser == null)
            {
                return NotFound();
            }

            // Call SetProgressData method to retrieve progress data for the user
            var list = await SetProgressData(premiumUser);


            if (list.Count != 0)
            {
                // Set ViewBag properties to pass consumed and burned calories progress data to the view
                ViewBag.ConsumedCaloriesProgressData = list[0];
                ViewBag.BurnedCaloriesProgressData = list[1];
            }

            // Returns the view associated with the PremiumUser passing the PremiumUser object
            return View(premiumUser);
        }


        // Method to calculate progress percentage based on a value and an average value
        private double CalculateProgressPercentage(double value, double averageValue)
        {
            // Calculate the progress percentage using the provided values
            double progress = value / averageValue * 100;

            // Ensure the progress percentage is within the valid range (0 to 100)
            return progress > 100 ? 100 : (progress < 0 ? 0 : progress);
        }


        // Action method for displaying details of a PremiumUser
        public async Task<IActionResult> Details()
        {
            // Retrieve the current user's ID using UserManager
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            // Fetch the PremiumUser associated with the user ID from the database
            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(userId));

            // Find a nutritionist associated with the current user
            var nutritionist = _context.Nutritionist.FirstOrDefault(n => n.PremiumUsers.Any(p => p.Id.Equals(userId)));

            // Check if the PremiumUser is not found
            if (premiumUser == null)
            {
                return NotFound();
            }

            // Call SetProgressData method to retrieve progress data for the user
            var list = await SetProgressData(premiumUser);

            // Set ViewBag properties to pass consumed and burned calories progress data to the view
            ViewBag.ConsumedCaloriesProgressData = list[0];
            ViewBag.BurnedCaloriesProgressData = list[1];

            // Return the view associated with the PremiumUser passing the PremiumUser object
            return View(premiumUser);
        }



        // GET: PremiumUser/Edit/5
        public async Task<IActionResult> Edit()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));

            if (premiumUser == null)
            {
                return NotFound();
            }

            return View(premiumUser);
        }

        // Action method for handling editing of PremiumUser details upon form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("City,Age,Weight,Height,Name,Email,NutriUsername,NutriPassword")] PremiumUser Premuser)
        {
            // Retrieve the current user's ID using UserManager
            var usrId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            // Find the user's PremiumUser data from the database
            var user = await _context.PremiumUser.FirstOrDefaultAsync(m => m.Id.Equals(usrId));

            // Check if the user is not found
            if (user == null)
            {
                return NotFound();
            }

            // Update user's password if the NutriPassword field is not null
            if (Premuser.NutriPassword != null)
            {
                await _userManager.ChangePasswordAsync(user, user.NutriPassword, Premuser.NutriPassword);
                user.NutriPassword = Premuser.NutriPassword;
            }

            // Update user's full name if provided
            if (Premuser.FullName != null)
            {
                user.FullName = Premuser.FullName;
            }

            // Update user's email if provided
            if (Premuser.Email != null)
            {
                user.EmailAddress = Premuser.Email;
                user.Email = Premuser.Email;
            }

            // Update user's username if provided
            if (Premuser.NutriUsername != null)
            {
                user.UserName = Premuser.NutriUsername;
                user.NutriUsername = Premuser.NutriUsername;
            }

            // Update user's weight if provided
            if (Premuser.Weight != 0)
            {
                user.Weight = Premuser.Weight;
            }

            // Update user's height if provided
            if (Premuser.Height != 0)
            {
                user.Height = Premuser.Height;
            }

            // Update user's city if provided
            if (Premuser.City != null)
            {
                user.City = Premuser.City;
            }

            // Update user's age if provided
            if (Premuser.Age != 0)
            {
                user.Age = Premuser.Age;
            }

            // Update the user in the UserManager and save changes to the database
            var result = await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            // Redirect to the Index action method after successful update
            return RedirectToAction("Index");
        }


        // Action method for displaying the delete view
        public async Task<IActionResult> Delete()
        {
            // Retrieve the current user's ID using UserManager
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            // Fetch the PremiumUser associated with the user ID from the database
            var premiumUser = await _context.PremiumUser.FirstOrDefaultAsync(m => m.Id.Equals(id));

            // Check if the PremiumUser is not found
            if (premiumUser == null)
            {
                // Return a "Not Found" response if the user is not found
                return NotFound();
            }
            // Return the view associated with the delete confirmation passing the PremiumUser object
            return View(premiumUser);
        }


        // POST action method to handle confirmed deletion of a PremiumUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string username, string password)
        {
            // Retrieve the current user's ID using UserManager
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

            // Check if PremiumUser entity set is null in the context
            if (_context.PremiumUser == null)
            {
                // Return a Problem response if the PremiumUser entity set is null
                return Problem("Entity set 'ApplicationDbContext.PremiumUser' is null.");
            }

            // Find the PremiumUser to delete from the database using the ID
            var premiumUser = await _context.PremiumUser.FirstOrDefaultAsync(m => m.Id.Equals(id));

            // Check if PremiumUser is not found
            if (premiumUser == null)
            {
                // Return a "Not Found" response if the PremiumUser is not found
                return NotFound();
            }

            // Validate provided username and password for deletion confirmation
            if (premiumUser.UserName != username || premiumUser.NutriPassword != password)
            {
                // Add model error for invalid username or password
                ModelState.AddModelError("", "Invalid username or password");

                // Return the Delete view to re-enter credentials for deletion
                return View("Delete");
            }

            // Fetch related data (progress, diet plans, user cards) associated with the PremiumUser
            var progressListForPremiumUser = await _context.Progress
                .Where(p => p.PremiumUser.Id == id)
                .ToListAsync();
            _context.Progress.RemoveRange(progressListForPremiumUser);

            var dietPlanListForPremiumUser = await _context.DietPlan
                .Where(p => p.PremiumUser.Id == id)
                .ToListAsync();
            _context.DietPlan.RemoveRange(dietPlanListForPremiumUser);

            var userCards = await _context.Card
                .Where(card => card.PremiumUserId == id)
                .ToListAsync();
            _context.Card.RemoveRange(userCards);

            // Remove the PremiumUser entity from the database context
            _context.PremiumUser.Remove(premiumUser);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Sign out the user
            await _signInManager.SignOutAsync();

            // Redirect to the Home/Index page after successful deletion
            return RedirectToAction("Index", "Home");
        }


        // Action method for displaying the leaderboard
        [ActionName("Leaderboard")]
        public async Task<IActionResult> Leaderboard()
        {
            // Retrieve the current user using UserManager
            //var currentUser = await _userManager.GetUserAsync(User);
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var premiumUser = await _context.PremiumUser.FirstOrDefaultAsync(u => u.Id.Equals(id));

            // Check if the PremiumUser is not found
            if (premiumUser == null)
            {
                // Return a "Not Found" response if the user is not found
                return NotFound();
            }


            // Retrieve the singleton instance of the Leaderboard class
            Leaderboard leaderboard = MyNutritionist.Models.Leaderboard.getInstance();

            // Check if the leaderboard city is null or different from the premiumUser's city
            if (leaderboard.City == null || leaderboard.City != premiumUser.City)
            {
                // Fetch users from the same city as the current premiumUser and order them by points
                var usersFromSameCity = await _context.PremiumUser
                    .Where(u => u.City == premiumUser.City)
                    .OrderByDescending(u => u.Points)
                    .ToListAsync();

                // Update the leaderboard data with users from the same city and the city name
                leaderboard.Users = usersFromSameCity;
                leaderboard.City = premiumUser.City;
            }

            // Return the view with the updated leaderboard data
            return View(leaderboard);
        }

        public async Task<IActionResult> GetQuote()
        {
			var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
			var premiumUser = await _context.PremiumUser.FirstOrDefaultAsync(p => p.Id == userId);
            return View(premiumUser);
		}

    }

}
