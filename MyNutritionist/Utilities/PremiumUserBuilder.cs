using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    public class PremiumUserBuilder : iUser
    {
        private string _city;
        private double _weight;
        private double _height;

        public void InitializeCity(string city)
        {
            _city = city;
        }

        public void InitializeWeight(double weight)
        {
            _weight = weight;
        }

        public void InitializeHeight(double height)
        {
            _height = height;
        }

        public PremiumUser Build()
        {
            return new PremiumUser(this);
        }
    }
}
