using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    public class SortByNames : ISort
    {
        public List<PremiumUser> Sort(List<PremiumUser> users)
        {
            users.Sort((user1, user2) => string.Compare(user1.FullName, user2.FullName, StringComparison.OrdinalIgnoreCase));
            return users;
        }
    }
}
