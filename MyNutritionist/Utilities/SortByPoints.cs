using MyNutritionist.Models;

namespace MyNutritionist.Utilities
    {
        // Implementacija ISort interfejsa za sortiranje objekata PremiumUser po bodovima
        public class SortByPoints : ISort { 

            public List<PremiumUser> SortList(List<PremiumUser> users)
            {
                return Sort(users, (x, y) => y.Points - x.Points);
            }
        }
}
