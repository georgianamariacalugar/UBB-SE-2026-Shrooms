using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieShop.Models;
using MovieShop.Repositories;

namespace MovieShop.Services
{
    public class MovieCatalogService : IMovieCatalogService
    {
        private readonly IMovieRepository _movieRepo;
        private readonly IActiveSalesRepository _salesRepo;
        private readonly IMovieReviewService _reviewService;
        public MovieCatalogService(IMovieRepository movieRepo, IActiveSalesRepository activeSalesRepo, IMovieReviewService reviewService)
        {
            _movieRepo = movieRepo;
            _salesRepo = activeSalesRepo;
            _reviewService = reviewService;

        }
        public void ApplyDiscount(Movie movie)
        {
            var discountMap = _salesRepo.GetBestDiscountPercentByMovieId();
            ActiveSalesRepo.ApplyBestDiscountsToMovies(new List<Movie> { movie }, discountMap);
        }

        public (List<Movie> Movies, Dictionary<int, int> ReviewCounts) GetUndiscountedMovies()
        {
            var all = _movieRepo.GetAllMovies();

            var discountMap = _salesRepo.GetBestDiscountPercentByMovieId();
            ActiveSalesRepo.ApplyBestDiscountsToMovies(all, discountMap);

            var onSaleIds = _salesRepo.GetCurrentSales()
                                     .Select(s => s.Movie.ID)
                                     .Distinct()
                                     .ToHashSet();

            var undiscounted = all
                .Where(m => !onSaleIds.Contains(m.ID))
                .ToList();

            var reviewCounts = _reviewService
                .GetReviewCounts(undiscounted.Select(m => m.ID));

            return (undiscounted, reviewCounts);
        }

        public (List<Movie> Movies, Dictionary<int, int> ReviewCounts) GetDiscountedMovies()
        {
            var all = _movieRepo.GetAllMovies();

            var discountMap = _salesRepo.GetBestDiscountPercentByMovieId();
            ActiveSalesRepo.ApplyBestDiscountsToMovies(all, discountMap);

            var onSaleIds = _salesRepo.GetCurrentSales()
                                     .Select(s => s.Movie.ID)
                                     .Distinct()
                                     .ToHashSet();

            var discounted = all
                .Where(m => onSaleIds.Contains(m.ID))
                .ToList();

            var reviewCounts = _reviewService
                .GetReviewCounts(discounted.Select(m => m.ID));

            return (discounted, reviewCounts);
        }

    }
}
