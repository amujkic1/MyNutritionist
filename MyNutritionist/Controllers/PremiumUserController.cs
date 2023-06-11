using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNutritionist.Data;
using MyNutritionist.Models;

namespace MyNutritionist.Controllers
{
    [Authorize(Roles = "PremiumUser")]
    public class PremiumUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<ApplicationUser> _userManager;

        public PremiumUserController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var premiumUser = await _context.PremiumUser.FirstOrDefaultAsync(p => p.Id == userId);

            if (premiumUser == null)
            {
                return NotFound();
            }

            var currentDate = DateTime.Now.Date;
            var currentDayOfWeek = (int)currentDate.DayOfWeek;

            var progressList = await _context.Progress
                .Where(p => p.PremiumUser.Id == userId && p.Date.Year == currentDate.Year && p.Date.Month == currentDate.Month && p.Date.Day >= currentDate.AddDays(-6).Day && p.Date.Day <= currentDate.Day)
                .OrderBy(p => p.Date)
                .ToListAsync();


            var averageConsumedCalories = 2000; // Prosječan broj unesenih kalorija
            var averageBurnedCalories = 300; // Prosječan broj potrošenih kalorija

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

            return View(premiumUser);
        }

        private double CalculateProgressPercentage(double value, double averageValue)
        {
            double progress = value / averageValue * 100;
            return progress > 100 ? 100 : (progress < 0 ? 0 : progress);
        }



        // GET: PremiumUser/Details/5
        public async Task<IActionResult> Details()
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

          
            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(userId));

            var nutritionist = _context.Nutritionist.FirstOrDefault(n => n.PremiumUsers.Any(p => p.Id.Equals(userId)));

            if (premiumUser == null)
            {
                return NotFound();
            }

            ViewBag.NUTRITIONIST = nutritionist;

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

        // POST: PremiumUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("City,Age,Weight,Height,Name,Email,NutriUsername,NutriPassword")] PremiumUser Premuser)
        {
            var usrId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            var user = await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(usrId));
            if (Premuser.NutriPassword != null)
            {
                await _userManager.ChangePasswordAsync(user, user.NutriPassword, Premuser.NutriPassword);
                user.NutriPassword = Premuser.NutriPassword;
            }
            if (Premuser.FullName != null) user.FullName = Premuser.FullName;
            if (Premuser.Email != null)
            {
                user.EmailAddress = Premuser.Email;
                user.Email = Premuser.Email;
            }
            if (Premuser.NutriUsername != null)
            {
                user.UserName = Premuser.NutriUsername;
                user.NutriUsername = Premuser.NutriUsername;
            }
            if (Premuser.Weight != 0) user.Weight = Premuser.Weight;
            if (Premuser.Height != 0) user.Height = Premuser.Height;
            if (Premuser.City != null) user.City = Premuser.City;
            if (Premuser.Age != 0) user.Age = Premuser.Age;


            var result = await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: PremiumUser/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete()
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

        // POST: PremiumUser/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            if (_context.PremiumUser == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PremiumUser'  is null.");
            }
            var premiumUser = await _context.PremiumUser.FindAsync(id);
            if (premiumUser != null)
            {
                _context.PremiumUser.Remove(premiumUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }

        private bool PremiumUserExists(string id)
        {
          return (_context.PremiumUser?.Any(e => e.Id.Equals(id))).GetValueOrDefault();
        }
        // GET: PremiumUser/DailyFoodAndActivity/5
        public async Task<IActionResult> PremiumDailyActivityAndFood(int? id)
        {
            /*
            if (id == null || _context.PremiumUser == null)
            {
                return NotFound();
            }

        */
            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            /*  if (premiumUser == null)
              {
                  return NotFound();
              }*/

            return View(premiumUser);
        }

        [ActionName("Leaderboard")]
        // GET: PremiumUser/Leaderboard
        public async Task<IActionResult> Leaderboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(u => u.Id == currentUser.Id);

            if (premiumUser == null)
            {
                // Trenutno ulogovani korisnik nije premium korisnik
                return NotFound();
            }
            
            Leaderboard leaderboard = MyNutritionist.Models.Leaderboard.getInstance();
            
            if (leaderboard.City == null || leaderboard.City != premiumUser.City) {
            var usersFromSameCity = await _context.PremiumUser
                .Where(u => u.City == premiumUser.City)
                .OrderByDescending(u => u.Points)
                .ToListAsync();

            leaderboard.Users = usersFromSameCity;
                leaderboard.City = premiumUser.City;
            }
            return View(leaderboard);
        }
        public IActionResult DailyActivityAndFood()
        {
            var model = new EnterActivityAndFoodViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult DailyActivityAndFood(EnterActivityAndFoodViewModel model)
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


                    var currentUser = await _context.PremiumUser.FirstOrDefaultAsync(u => u.Id == userId);


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
                        RegisteredUser = null,
                        PremiumUser = currentUser
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
