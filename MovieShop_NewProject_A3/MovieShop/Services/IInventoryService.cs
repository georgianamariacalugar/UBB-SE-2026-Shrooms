using MovieShop.Models;
using System.Collections.Generic;

namespace MovieShop.Services
{
    public interface IInventoryService
    {
        IEnumerable<Movie> RemoveMovie(int userId, int movieId);

        IEnumerable<MovieEvent> RemoveTicket(int userId, int eventId);
    }
}