namespace MyNutritionist.Models
{
    public class DietPlanViewModel
    {
        public DietPlan DietPlan { get; set; }
        public string PremiumUserId { get; set; }

        //public enum Days { Mon, Tue, Wed, Thu, Fri, Sat, Sun}

        public DietPlanViewModel() { }
    }
}
