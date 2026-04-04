using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieShop.Models;
using MovieShop.Repositories;
using MovieShop.Services;
using Microsoft.UI.Xaml;
using System;

namespace MovieShop.Views
{
    public sealed partial class BuyTicketPage : Page
    {
        private MovieEvent? _event;
        private readonly IEventRepository _eventRepo = App.Services.GetRequiredService<IEventRepository>();
        private readonly IUserRepository _userRepo = App.Services.GetRequiredService<IUserRepository>();
        private readonly IEventTicketService _ticketService = App.Services.GetRequiredService<IEventTicketService>();

        public BuyTicketPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int id)
                _event = _eventRepo.GetEventById(id);
            else if (e.Parameter is MovieEvent me)
                _event = me;

            if (_event == null) return;

            this.DataContext = _event;
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            if (!SessionManager.IsLoggedIn)
            {
                ConfirmButton.IsEnabled = false;
                InsufficientText.Text = "You must be signed in to purchase.";
                InsufficientText.Visibility = Visibility.Visible;
                return;
            }

            if (_event == null)
            {
                ConfirmButton.IsEnabled = false;
                return;
            }

            var canBuy = _ticketService.CanBuyTicket(SessionManager.CurrentUserID, _event);
            ConfirmButton.IsEnabled = canBuy;

            if (!canBuy)
            {
                InsufficientText.Text = $"Insufficient funds. Balance: {SessionManager.CurrentUserBalance:C} — Price: {_event.TicketPrice:C}";
                InsufficientText.Visibility = Visibility.Visible;
            }
            else
            {
                InsufficientText.Visibility = Visibility.Collapsed;
            }
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_event == null) return;

            if (!SessionManager.IsLoggedIn)
            {
                var dlg = new ContentDialog
                {
                    Title = "Sign in",
                    Content = "Please sign in to continue.",
                    PrimaryButtonText = "Sign in",
                    CloseButtonText = "Cancel",
                    XamlRoot = XamlRoot
                };

                if (await dlg.ShowAsync() != ContentDialogResult.Primary)
                    return;

                SessionManager.CurrentUserID = 1;
                SessionManager.CurrentUserBalance = _userRepo.GetBalance(1);
                UpdateButtonState();
                return;
            }

            try
            {
                await System.Threading.Tasks.Task.Run(() =>
                    _ticketService.PurchaseTicket(SessionManager.CurrentUserID, _event));

                UpdateButtonState();

                var dialog = new ContentDialog
                {
                    Title = "Purchase successful",
                    Content = $"Ticket for '{_event.Title}' purchased and added to your library.",
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await dialog.ShowAsync();

                if (this.XamlRoot?.Content is NavigationPage navPage)
                {
                    navPage.ViewModel.RefreshWallet();
                    navPage.ViewModel.CurrentViewModel = "Inventory";
                }
            }
            catch (InvalidOperationException ex)
            {
                var err = new ContentDialog
                {
                    Title = "Cannot complete purchase",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await err.ShowAsync();
            }
        }
    }
}