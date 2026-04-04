using MovieShop.Models;
using MovieShop.Repositories;
using System;
using System.Collections.Generic;

namespace MovieShop.Services
{
	public class InventoryService : IInventoryService
	{
		private readonly IInventoryRepository _inventoryRepo;

		public InventoryService(IInventoryRepository inventoryRepo)
		{
			_inventoryRepo = inventoryRepo;
		}

		public IEnumerable<Movie> RemoveMovie(int userId, int movieId)
		{
			if (userId <= 0) throw new ArgumentException("Invalid user ID.", nameof(userId));
			if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", nameof(movieId));

			_inventoryRepo.RemoveOwnedMovie(userId, movieId);
			return _inventoryRepo.GetOwnedMovies(userId);
		}

		public IEnumerable<MovieEvent> RemoveTicket(int userId, int eventId)
		{
			if (userId <= 0) throw new ArgumentException("Invalid user ID.", nameof(userId));
			if (eventId <= 0) throw new ArgumentException("Invalid event ID.", nameof(eventId));

			_inventoryRepo.RemoveOwnedTicket(userId, eventId);
			return _inventoryRepo.GetOwnedTickets(userId);
		}
	}
}