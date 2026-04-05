using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Services
{
    public interface IMovieReviewService
    {
        int GetReviewCount(int movieId);
        Dictionary<int, int> GetReviewCounts(IEnumerable<int> movieIds);
        string BuildStarDistributionTooltip(int movieId);
        void AddReview(int movieId, int userId, int rating, string? comment);
    }
}
