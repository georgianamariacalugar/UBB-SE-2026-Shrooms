using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Data.SqlClient;
using MovieShop.Models;
using MovieShop.Repositories;
using MovieShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using MovieShop.Services;

namespace MovieShop.Views;

public sealed partial class MovieDetailPage : Page
{
    private Movie? _movie;
    private MainViewModel? _mainVm;
   private readonly IMoviePurchaseService _purchaseService = App.Services.GetRequiredService<IMoviePurchaseService>();
    private readonly IMovieReviewService _reviewService = App.Services.GetRequiredService<IMovieReviewService>();
    private readonly IMovieCatalogService _movieCatalogService = App.Services.GetRequiredService<IMovieCatalogService>();

    public MovieDetailPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is not MovieDetailNavArgs args)
            return;

        _movie = args.Movie;
        _mainVm = args.MainViewModel;

        if (_movie == null)
            return;

        _movieCatalogService.ApplyDiscount(_movie);

        TitleBlock.Text = _movie.Title;
        DescriptionBlock.Text = string.IsNullOrEmpty(_movie.Description) ? "—" : _movie.Description;
        RatingBlock.Text = $"Rating: {_movie.Rating:0.0} / 10";
        UpdatePriceDisplay();

        TrySetPoster(_movie.ImageUrl);

        RefreshBuyButtonState();
        ToolTipService.SetToolTip(
            ReviewsButton,
            _reviewService.BuildStarDistributionTooltip(_movie.ID)
        );

    }

    private void UpdatePriceDisplay()
    {
        if (_movie == null)
            return;

        PriceBlock.Text = $"${_movie.DiscountedPriceText}";

        if (_movie.HasActiveSale)
        {
            OriginalPriceBlock.Visibility = Visibility.Visible;
            OriginalPriceBlock.Text = $"${_movie.OriginalPriceText}";
        }
        else
        {
            OriginalPriceBlock.Visibility = Visibility.Collapsed;
        }
    }

    private void TrySetPoster(string? url)
    {
        PosterImage.Source = null;
        if (string.IsNullOrWhiteSpace(url))
            return;
        try
        {
            PosterImage.Source = new BitmapImage(new Uri(url, UriKind.Absolute));
        }
        catch
        {
            /* ignore invalid image URL */
        }
    }

    private void RefreshBuyButtonState()
    {
        if (_movie == null)
            return;

        _mainVm?.RefreshBalanceFromDatabase();
        var props = _purchaseService.GetBuyButtonProps(
        _movie,
        SessionManager.CurrentUserID,
        SessionManager.IsLoggedIn,
        _mainVm?.Balance ?? SessionManager.CurrentUserBalance
    );

        BuyMovieButton.Content = props.Content;
        BuyMovieButton.IsEnabled = props.IsEnabled;
        BuyMovieButton.Opacity = props.Opacity;
        ToolTipService.SetToolTip(BuyMovieButton, props.ToolTip);
    }

    private async void BuyMovieButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_movie == null || _mainVm == null)
            return;

        if (!SessionManager.IsLoggedIn)
            return;

        RefreshBuyButtonState();
        if (!BuyMovieButton.IsEnabled)
            return;

        var confirm = new ContentDialog
        {
            Title = "Confirm purchase",
            Content = $"Buy \"{_movie.Title}\" for {_movie.GetEffectivePrice():C}? This will be charged to your balance.",
            PrimaryButtonText = "Buy",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = XamlRoot
        };

        if (await confirm.ShowAsync() != ContentDialogResult.Primary)
            return;

        try
        {
            _purchaseService.PurchaseMovie(SessionManager.CurrentUserID, _movie);

            _mainVm.RefreshWallet();
            SessionManager.CurrentUserBalance = _mainVm.Balance;

            RefreshBuyButtonState();

            var dialog = new ContentDialog
            {
                Title = "Purchase successful",
                Content = $"You now own \"{_movie.Title}\". It has been added to your inventory.",
                PrimaryButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot
            };
            _ = await dialog.ShowAsync();

            if (this.XamlRoot?.Content is NavigationPage navPage)
            {
                navPage.ViewModel.CurrentViewModel = "Inventory";
            }
        }
        catch (InvalidOperationException ex)
        {
            var err = new ContentDialog
            {
                Title = "Cannot complete purchase",
                Content = ex.Message,
                PrimaryButtonText = "OK",
                XamlRoot = XamlRoot
            };
            _ = await err.ShowAsync();
        }
    }

    private void ReviewsButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_movie == null || _mainVm == null)
            return;

        Frame?.Navigate(typeof(MovieReviewsPage), new MovieReviewsNavArgs
        {
            Movie = _movie,
            MainViewModel = _mainVm
        });
    }

    private void EventsButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_movie == null || _mainVm == null)
            return;

        Frame?.Navigate(typeof(MovieEventsPage), new MovieEventsNavArgs
        {
            Movie = _movie,
            MainViewModel = _mainVm
        });
    }

    private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (Frame?.CanGoBack == true)
        {
            Frame.GoBack();
            return;
        }

        if (this.XamlRoot?.Content is NavigationPage navPage)
        {
            navPage.ViewModel.CurrentViewModel = "Shop";
        }
    }


}