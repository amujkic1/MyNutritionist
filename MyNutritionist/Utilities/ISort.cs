using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    /// Interface for sorting a list of PremiumUser objects.
    public interface ISort
    { /// Sorts a list of PremiumUser objects based on a specific criteria.
        /// "users" The list of PremiumUser objects to be sorted.
        /// returns a sorted list of PremiumUser objects.
        List<PremiumUser> Sort(List<PremiumUser> users);
    }
}
