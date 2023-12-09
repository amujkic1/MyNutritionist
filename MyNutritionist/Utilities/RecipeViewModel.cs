using MyNutritionist.Models;
using System.Collections.Generic;

namespace MyNutritionist.Utilities
{
    /*
     * RecipeViewModel klasa predstavlja ViewModel (model za prikaz) za rad sa receptima.
     * Koristi se za prikazivanje podataka u vezi sa receptima u korisničkom interfejsu.
     */
    public class RecipeViewModel
    {
        /*
         * Svojstvo koje predstavlja ulazni recept, podrazumijevano inicijalizovan na novi recept.
         */
        public Recipe input { get; set; } = new Recipe();

        /*
         * Svojstvo koje predstavlja listu recepata za prikazivanje.
         * Podrazumijevano inicijalizovano na praznu listu.
         */
        public List<Recipe> recipesToDisplay { get; set; } = new List<Recipe>();

        /*
         * Konstruktor RecipeViewModel klase.
         * Inicijalizuje objekat klase.
         */
        public RecipeViewModel() { }
    }
}
