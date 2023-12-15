using MyNutritionist.Models;

namespace MyNutritionist.Utilities
    {
        // Implementacija ISort interfejsa za sortiranje objekata PremiumUser po bodovima
        public class SortByPoints : ISort
        {
            // Vrši sortiranje liste objekata PremiumUser korištenjem QuickSort algoritma
            public List<PremiumUser> QuickSort(List<PremiumUser> users)
            {
                // Provjera za null ulaz, vraća praznu listu ako je ulaz null
                if (users == null)
                {
                    return new List<PremiumUser> { };
                }

                // Osnovni slučaj: Ako lista ima 1 element ili je prazna, već je sortirana
                if (users.Count <= 1)
                {
                    return users;
                }

                // Odabir pivota (srednji element u ovom slučaju)
                int pivotIndex = users.Count / 2;
                PremiumUser pivotUser = users[pivotIndex];

                // Kreiranje dvije liste za čuvanje elemenata manjih i većih od pivota
                List<PremiumUser> left = new List<PremiumUser>();
                List<PremiumUser> right = new List<PremiumUser>();

                // Partitioniranje liste bazirano na pivotu
                for (int i = 0; i < users.Count; i++)
                {
                    // Preskoči pivotni element
                    if (i == pivotIndex)
                    {
                        continue;
                    }

                    // Uporedi elemente sa pivotom bazirano na Points svojstvu
                    int comparisonResult = users[i].Points.CompareTo(pivotUser.Points);

                    // Smještaj elemenata u odgovarajuću listu
                    if (comparisonResult < 0)
                    {
                        left.Add(users[i]);
                    }
                    else
                    {
                        right.Add(users[i]);
                    }
                }

                // Rekurzivno sortiraj lijeve i desne podliste
                List<PremiumUser> sortedUsers = new List<PremiumUser>();
                sortedUsers.AddRange(QuickSort(left));
                sortedUsers.Add(pivotUser);
                sortedUsers.AddRange(QuickSort(right));

                // Vrati sortiranu listu
                return sortedUsers;
            }

            public List<PremiumUser> Sort(List<PremiumUser> users)
            {
                // Proslijeđuje poziv metode QuickSort za sortiranje
                return QuickSort(users);
            }
        }
}
