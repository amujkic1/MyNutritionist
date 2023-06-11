using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    public class SortByPoints : ISort
    {
        public List<PremiumUser> Sort(List<PremiumUser> users)
        {
            users.Sort((user1, user2) => user1.Points.CompareTo(user2.Points));
            return users;
        }
    }
}
