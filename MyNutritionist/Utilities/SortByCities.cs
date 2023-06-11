using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    public class SortByCities : ISort
    {
        public List<PremiumUser> Sort(List<PremiumUser> users)
        {
            users.Sort((user1, user2) => string.Compare(user1.City, user2.City, StringComparison.OrdinalIgnoreCase));
            return users;
        }
    }
}
