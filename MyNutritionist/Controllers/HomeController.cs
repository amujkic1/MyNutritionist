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

        /*
         * Konstruktor HomeController klase.
         *
         * @param logger: Logger za snimanje poruka o događajima.
         */
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            return View("Index");
        }
    }
}
