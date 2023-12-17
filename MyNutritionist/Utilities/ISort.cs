using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    /// Interface for sorting a list of PremiumUser objects.
    public class ISort
    { /// Sorts a list of PremiumUser objects based on a specific criteria.
        /// "users" The list of PremiumUser objects to be sorted.
        /// returns a sorted list of PremiumUser objects.
        public List<PremiumUser> Sort(List<PremiumUser> users, Func<PremiumUser, PremiumUser, int> comparison)
        {
                if (users == null)
                {
                    return new List<PremiumUser>();
                }

                if (users.Count <= 1)
                {
                    return users;
                }

                int pivotIndex = users.Count / 2;
                PremiumUser pivotUser = users[pivotIndex];

                List<PremiumUser> left = new List<PremiumUser>();
                List<PremiumUser> right = new List<PremiumUser>();

                for (int i = 0; i < users.Count; i++)
                {
                    if (i == pivotIndex)
                    {
                        continue;
                    }

                    int comparisonResult = comparison(users[i], pivotUser);

                    if (comparisonResult < 0)
                    {
                        left.Add(users[i]);
                    }
                    else
                    {
                        right.Add(users[i]);
                    }
                }

                List<PremiumUser> sortedUsers = new List<PremiumUser>();
                sortedUsers.AddRange(Sort(left, comparison));
                sortedUsers.Add(pivotUser);
                sortedUsers.AddRange(Sort(right, comparison));

                return sortedUsers;
        }
    }
}
