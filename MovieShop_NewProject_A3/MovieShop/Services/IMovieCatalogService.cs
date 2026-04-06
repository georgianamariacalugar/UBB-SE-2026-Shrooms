using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieShop.Models;

namespace MovieShop.Services
{
    interface IMovieCatalogService
    {
        void ApplyDiscount(Movie movie);
        (List<Movie> Movies, Dictionary<int, int> ReviewCounts) GetUndiscountedMovies();
        (List<Movie> Movies, Dictionary<int, int> ReviewCounts) GetDiscountedMovies();
    }
}
