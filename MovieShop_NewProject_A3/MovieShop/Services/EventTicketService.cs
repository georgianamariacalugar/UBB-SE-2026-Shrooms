using MovieShop.Models;
using MovieShop.Repositories;
using System;

namespace MovieShop.Services
{
    public class EventTicketService : IEventTicketService
    {
        private readonly IEventRepository _eventRepo;
        private readonly IUserRepository _userRepo;

        public EventTicketService(IEventRepository eventRepo, IUserRepository userRepo)
        {
            _eventRepo = eventRepo;
            _userRepo = userRepo;
        }

        public bool CanBuyTicket(int userId, MovieEvent movieEvent)
        {
            if (userId <= 0 || movieEvent == null)
                return false;

            var balance = _userRepo.GetBalance(userId);
            SessionManager.CurrentUserBalance = balance;
            return balance >= movieEvent.TicketPrice;
        }

        // Moved from MovieEventsPage.xaml.cs:144-220 and BuyTicketPage.xaml.cs:84-143
        public void PurchaseTicket(int userId, MovieEvent movieEvent)
        {
            _eventRepo.PurchaseTicket(userId, movieEvent.ID);
            SessionManager.CurrentUserBalance = _userRepo.GetBalance(userId);
        }
    }
}