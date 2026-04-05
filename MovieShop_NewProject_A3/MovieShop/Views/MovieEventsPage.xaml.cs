using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieShop.Models;
using MovieShop.Repositories;
using MovieShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;

namespace MovieShop.Views;

public sealed partial class MovieEventsPage : Page
{
    private Movie? _movie;
    private List<MovieEvent>? _allEvents;
    private readonly IEventRepository _eventRepo = App.Services.GetRequiredService<IEventRepository>();
    private readonly IUserRepository _userRepo = App.Services.GetRequiredService<IUserRepository>();
    private readonly IEventTicketService _ticketService = App.Services.GetRequiredService<IEventTicketService>();

    private static object? FindDescendantByName(DependencyObject parent, string name)
    {
        if (parent == null) return null;
        var count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is FrameworkElement fe && fe.Name == name)
                return fe;
            var found = FindDescendantByName(child, name);
            if (found != null) return found;
        }
        return null;
    }

    public MovieEventsPage()
    {
        InitializeComponent();
        this.Loaded += MovieEventsPage_Loaded;
    }

    private void MovieEventsPage_Loaded(object? sender, RoutedEventArgs e)
    {
        if (EventsList.ItemsSource == null)
        {
            TitleBlock.Text = "Events";
            _allEvents = _eventRepo.GetAllEvents();
            EventsList.ItemsSource = _allEvents;
            UpdateBuyButtons();
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();

    private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

    private void ApplyFilters()
    {
        if (_allEvents == null) return;

        var filtered = _allEvents.AsEnumerable();

        var search = SearchBox?.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(search))
            filtered = filtered.Where(ev =>
                (ev.Title ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                (ev.Description ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                (ev.Location ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);

        var sel = (FilterCombo?.SelectedItem as ComboBoxItem)?.Content as string ?? "All";
        if (sel == "Upcoming")
            filtered = filtered.Where(ev => ev.Date >= DateTime.Now);
        else if (sel == "Past")
            filtered = filtered.Where(ev => ev.Date < DateTime.Now);

        EventsList.ItemsSource = filtered.ToList();
        UpdateBuyButtons();
    }

    private void UpdateBuyButtons()
    {
        var userId = SessionManager.CurrentUserID;

        foreach (var item in EventsList.Items)
        {
            var lvi = EventsList.ContainerFromItem(item) as ListViewItem;
            if (lvi == null) continue;
            var btn = FindDescendantByName(lvi, "BuyTicketButton") as Button;
            if (btn == null) continue;

            var ev = item as MovieEvent;
            var canBuy = ev != null && _ticketService.CanBuyTicket(userId, ev);
            btn.IsEnabled = canBuy;
            btn.Opacity = canBuy ? 1.0 : 0.55;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is not MovieEventsNavArgs args) return;

        _movie = args.Movie;
        if (_movie == null)
        {
            TitleBlock.Text = "Events";
            _allEvents = _eventRepo.GetAllEvents();
            EventsList.ItemsSource = _allEvents;
            return;
        }

        TitleBlock.Text = $"Events - {_movie.Title}";
        _allEvents = _eventRepo.GetEventsForMovie(_movie.ID);
        EventsList.ItemsSource = _allEvents;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Frame?.CanGoBack == true)
            Frame.GoBack();
    }

    private async void BuyTicket_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement fe || fe.DataContext is not MovieEvent me)
            return;

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
            try
            {
                SessionManager.CurrentUserBalance = _userRepo.GetBalance(1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MovieEventsPage] Failed to fetch balance after login: {ex.Message}");
                SessionManager.CurrentUserBalance = 0m;
            }

            UpdateBuyButtons();
            return; // let the user click Buy again after signing in
        }

        try
        {
            await System.Threading.Tasks.Task.Run(() =>
                _ticketService.PurchaseTicket(SessionManager.CurrentUserID, me));

            UpdateBuyButtons();

            var dialog = new ContentDialog
            {
                Title = "Purchase successful",
                Content = $"Ticket for '{me.Title}' purchased and added to your library.",
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MovieEventsPage] Unexpected purchase error: {ex}");
            var err = new ContentDialog
            {
                Title = "Error",
                Content = "An unexpected error occurred: " + ex.Message,
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            };
            await err.ShowAsync();
        }
    }
}