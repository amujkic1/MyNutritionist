using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    // Implementacija ISort interfejsa za sortiranje objekata PremiumUser po imenima
    public class SortByNames : ISort
    {

        public List<PremiumUser> SortList(List<PremiumUser> users)
        {
            // Proslijeđuje poziv metode QuickSort za sortiranje
            return Sort(users, (x, y) => string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
