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
                .Where(p => p.RegisteredUser.Id == userId && p.Date >= currentDate.AddDays(-6) && p.Date <= currentDate)
                .OrderBy(p => p.Date)
                .ToListAsync();

            var averageConsumedCalories = 2000;
            var averageBurnedCalories = 300;

            var consumedCaloriesProgressData = new List<object>();
            var burnedCaloriesProgressData = new List<object>();

            for (var i = 6; i >= 0; i--)
            {
                var date = currentDate.AddDays(-i);
                var dayOfWeek = (int)date.DayOfWeek;
                var progress = progressList.FirstOrDefault(p => p.Date.Date == date.Date);

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
            var currentDate = DateTime.Now.Date;
            var currentDayOfWeek = (int)currentDate.DayOfWeek;

            var progressList = await _context.Progress
                .Where(p => p.PremiumUser.Id == userId && p.Date.Year == currentDate.Year && p.Date.Month == currentDate.Month && p.Date.Day >= currentDate.AddDays(-6).Day && p.Date.Day <= currentDate.Day)
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
                _context.PremiumUser.Remove(premiumUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool PremiumUserExists(string id)
        {
          return (_context.PremiumUser?.Any(e => e.Id.Equals(id))).GetValueOrDefault();
        }
        // GET: PremiumUser/DailyFoodAndActivity/5
        public async Task<IActionResult> PremiumDailyActivityAndFood(int? id)
        {
            var premiumUser = await _context.PremiumUser
                .FirstOrDefaultAsync(m => m.Id.Equals(id));

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

    }

}
