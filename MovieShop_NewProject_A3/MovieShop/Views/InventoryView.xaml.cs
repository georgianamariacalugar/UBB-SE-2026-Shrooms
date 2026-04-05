using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieShop.Repositories;
using MovieShop.Models;
using MovieShop.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieShop.Views
{
    public sealed partial class InventoryView : Page
    {
        private readonly IInventoryRepository _repo = App.Services.GetRequiredService<IInventoryRepository>();
        private readonly IUserRepository _userRepo = App.Services.GetRequiredService<IUserRepository>();
        private readonly IInventoryService _inventoryService = App.Services.GetRequiredService<IInventoryService>();

        public InventoryView()
        {
            InitializeComponent();
            Loaded += InventoryView_Loaded;
            MoviesGrid.ItemClick += MoviesGrid_ItemClick;
        }

        private void MoviesGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Movie m)
            {
                Frame?.Navigate(typeof(MovieDetailPage), new MovieDetailNavArgs { Movie = m, MainViewModel = null! });
            }
        }

        private void InventoryView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var userId = SessionManager.CurrentUserID;
            if (userId <= 0) return;

            var movies = _repo.GetOwnedMovies(userId);
            MoviesGrid.ItemsSource = movies;

            var tickets = _repo.GetOwnedTickets(userId);
            TicketsGrid.ItemsSource = tickets;

            var equipment = _repo.GetOwnedEquipment(userId);
            EquipmentGrid.ItemsSource = equipment;
        }

        private async void RemoveMovie_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => await RemoveMovieAsync(sender);

        private async Task RemoveMovieAsync(object sender)
        {
            if (sender is Microsoft.UI.Xaml.FrameworkElement fe && fe.DataContext is Movie m)
            {
                var dlg = new ContentDialog
                {
                    Title = "Remove movie",
                    Content = $"Are you sure you want to remove '{m.Title}' from your library? This will allow you to purchase it again.",
                    PrimaryButtonText = "Remove",
                    CloseButtonText = "Cancel",
                    XamlRoot = XamlRoot
                };

                MoviesGrid.ItemsSource = _inventoryService.RemoveMovie(SessionManager.CurrentUserID, m.ID);

                if (this.XamlRoot?.Content is NavigationPage navPage)
                    navPage.ViewModel.RefreshWallet();
            }
        }

        private async void RemoveTicket_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => await RemoveTicketAsync(sender);

        private async Task RemoveTicketAsync(object sender)
        {
            if (sender is Microsoft.UI.Xaml.FrameworkElement fe && fe.DataContext is MovieEvent ev)
            {
                var dlg = new ContentDialog
                {
                    Title = "Remove ticket",
                    Content = $"Remove your ticket for '{ev.Title}'? This will allow you to buy it again.",
                    PrimaryButtonText = "Remove",
                    CloseButtonText = "Cancel",
                    XamlRoot = XamlRoot
                };

                TicketsGrid.ItemsSource = _inventoryService.RemoveTicket(SessionManager.CurrentUserID, ev.ID);

                if (this.XamlRoot?.Content is NavigationPage navPage)
                    navPage.ViewModel.RefreshWallet();
            }
        }
    }
}