using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNutritionist.Models
{
    public class Progress
    {
        [Key]
        public int PRId { get; set; }
        public DateTime Date { get; set; }
        public int BurnedCalories { get; set; }  
        public int ConsumedCalories { get; set; }
        public RegisteredUser RegisteredUser { get; set; }
        public PremiumUser PremiumUser { get; set; }
        public Progress() { }

        public int CalculatePoints(int consumedCalories, int burnedCalories)
        {
            const int NormalDailyCalories = 2000;
            const int MaxPoints = 20;
            const int MinPoints = 0;

            var deviationPercentage = Math.Abs((consumedCalories - burnedCalories) / (double)NormalDailyCalories) * 100;
            var deviationPoints = MaxPoints - (int)Math.Round(deviationPercentage / 100 * MaxPoints);

            deviationPoints = Math.Max(MinPoints, Math.Min(MaxPoints, deviationPoints));

            return deviationPoints;
        }

        public int CalculateBurnedCalories(PhysicalActivity activity)
        {
            const int RunningCaloriesPerMinute = 10;
            const int WalkingCaloriesPerMinute = 5;
            const int CyclingCaloriesPerMinute = 8;

            var durationInMinutes = activity.Duration;

            switch (activity.ActivityType)
            {
                case ActivityType.RUNNING:
                    return durationInMinutes * RunningCaloriesPerMinute;

                case ActivityType.WALKING:
                    return durationInMinutes * WalkingCaloriesPerMinute;

                case ActivityType.CYCLING:
                    return durationInMinutes * CyclingCaloriesPerMinute;

                default:
                    return 0;
            }
        }

    }
}
