using MyNutritionist.Models;
using NuGet.Packaging.Signing;
using static System.Net.Mime.MediaTypeNames;

namespace MyNutritionist.Utilities
{
    /// Interface for an iterator over a collection of Recipe objects.
    public interface Iterator
    {
        ///Finds and returns the next Recipe in the iteration sequence.
        /// returns the next Recipe in the iteration sequence.
        public Recipe findNextRecipe();
        /// Gets the current Recipe in the iteration.
        /// returns the current Recipe in the iteration.
        public Recipe getRecipe();
    }
}
