using MyNutritionist.Controllers;
using MyNutritionist.Models;
using System.Collections.Generic;

namespace MyNutritionist.Utilities
{
    /// Interface for creating iterators over a collection of recipes.
    public interface ICreateIterator
    {
        // Creates an iterator for the given list of recipes.
        /// "recipes" The list of recipes to iterate over.
        /// returns an iterator for the provided list of recipes.
        Iterator CreateIterator(List<Recipe> recipes);
    }
}
