using MyNutritionist.Models;
using System.Collections.Generic;

namespace MyNutritionist.Utilities
{
    /*
     * SortByPoints klasa implementira ISort interfejs i služi za 
     * sortiranje liste PremiumUser objekata prema bodovima.
     */
    public class SortByPoints : ISort
    {
        /*
         * Metoda za sortiranje liste PremiumUser objekata prema broju bodova.
         *
         * @param users: Lista PremiumUser objekata koja se sortira.
         *
         * @return: Sortirana lista PremiumUser objekata prema broju bodova.
         */
        public List<PremiumUser> Sort(List<PremiumUser> users)
        {
            users.Sort((user1, user2) => user1.Points.CompareTo(user2.Points));
            return users;
        }
    }
}
