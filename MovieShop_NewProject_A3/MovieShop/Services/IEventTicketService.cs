using MovieShop.Models;

namespace MovieShop.Services
{
    public interface IEventTicketService
    {     
        bool CanBuyTicket(int userId, MovieEvent movieEvent);

        void PurchaseTicket(int userId, MovieEvent movieEvent);
    }
}