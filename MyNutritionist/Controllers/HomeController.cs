using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using MyNutritionist.Data;
using MyNutritionist.Migrations;
using MyNutritionist.Models;
using System.Diagnostics;

namespace MyNutritionist.Controllers
{
    /*
     * HomeController klasa predstavlja kontroler za osnovne rutine 
     * vezane za početnu stranicu i privatnost.
     */
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        /*
         * Konstruktor HomeController klase.
         *
         * @param logger: Logger za snimanje poruka o događajima.
         */
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /*
         * Akcija koja vraća rezultat za početnu stranicu.
         *
         * @return: Pogled (View) za početnu stranicu.
         */
        public IActionResult Index()
        {
            return View();
        }

        /*
         * Akcija koja vraća rezultat za stranicu privatnosti.
         *
         * @return: Pogled (View) za stranicu privatnosti.
         */
        public IActionResult Privacy()
        {
            return View();
        }

        /*
         * Akcija koja se poziva u slučaju greške.
         *
         * @return: Pogled (View) sa informacijama o greški.
         */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Training()
        {
            var trainings = new List<Training> {
                new Training { TID = 1, nameOfTraining="Training", image="image.png", link="", duration = 10},
                new Training { TID = 2, nameOfTraining="Training", image="image.png", link="", duration = 40},
                new Training { TID = 3, nameOfTraining="Training", image="image.png", link="", duration = 30},
                new Training { TID = 4, nameOfTraining="Training", image="image.png", link="", duration = 20},
                new Training { TID = 5, nameOfTraining="Training", image="image.png", link="", duration = 10},
                new Training { TID = 6, nameOfTraining="Training", image="image.png", link="", duration = 20}
            };
            return View(trainings);
        }
    }
}
