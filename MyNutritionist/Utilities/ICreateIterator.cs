using MyNutritionist.Controllers;
using MyNutritionist.Models;

namespace MyNutritionist.Utilities
{
    public interface ICreateIterator
    {
        Iterator CreateIterator(List<Recipe> recipes);
    }
}
