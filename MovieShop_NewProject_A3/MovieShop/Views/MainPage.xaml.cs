using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using MovieShop.Models;
using MovieShop.Repositories;
using MovieShop.Services;


namespace MovieShop.Views
{
    public sealed partial class MainPage : UserControl
    {
        public ViewModels.FlashSaleViewModel FlashSaleVM => MovieShop.Services.SaleService.CurrentSale;
        private List<Movie> _sourceMovies = new();
        private Dictionary<int, int> _reviewCountByMovieId = new();
        private readonly IMovieCatalogService _catalogService = App.Services.GetRequiredService<IMovieCatalogService>();

        public MainPage()
        {
            InitializeComponent();

            FlashSaleVM.PropertyChanged += FlashSaleVM_PropertyChanged!;
            UpdateBigBanner(FlashSaleVM.TimerText, FlashSaleVM.IsActive);

            SearchBox.TextChanged += (_, _) => ApplyFilterAndSort();
            SortAscPrice.IsChecked = true;
            RefreshUndiscountedMovies();
        }

        public void UpdateBigBanner(string time, bool isActive)
        {
            BigTimerText.Text = time;

            if (isActive)
            {
                BigSaleBanner.Visibility = Visibility.Visible;
                BigSaleBanner.Height = double.NaN;
                BigSaleBanner.Margin = new Thickness(0, 0, 0, 20);
            }
            else
            {
                BigSaleBanner.Visibility = Visibility.Collapsed;
                BigSaleBanner.Height = 0;
                BigSaleBanner.Margin = new Thickness(0);
            }
        }

        private void DiscoverButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.XamlRoot.Content is NavigationPage navPage)
            {
                navPage.ViewModel.CurrentViewModel = "SalesPage";
            }

        }

        private void FlashSaleVM_PropertyChanged(object senser, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateBigBanner(FlashSaleVM.TimerText, FlashSaleVM.IsActive);

                // Only refresh when the flash sale starts/ends (not every second).
                if (e.PropertyName == nameof(ViewModels.FlashSaleViewModel.IsActive))
                    RefreshUndiscountedMovies();
            });
        }

        private void RefreshUndiscountedMovies()
        {
            var (movies, reviewCounts) = _catalogService.GetUndiscountedMovies();

            _sourceMovies = movies;
            _reviewCountByMovieId = reviewCounts;

            ApplyFilterAndSort();
        }

        private void ApplyFilterAndSort()
        {
            var q = (SearchBox.Text ?? "").Trim().ToLower();
            var list = string.IsNullOrEmpty(q)
                ? _sourceMovies
                : _sourceMovies.Where(m => m.Title.ToLower().Contains(q));

            if (SortAscPrice.IsChecked == true) list = list.OrderBy(m => m.Price);
            else if (SortDescPrice.IsChecked == true) list = list.OrderByDescending(m => m.Price);
            else if (SortHighRating.IsChecked == true) list = list.OrderByDescending(m => m.Rating);
            else if (SortLowRating.IsChecked == true) list = list.OrderBy(m => m.Rating);
            else list = list.OrderBy(m => m.Title);

            var orderedMovies = list.ToList();
            UndiscountedMoviesGrid.ItemsSource = orderedMovies
                .Select(m => new MovieCatalogItem(m, _reviewCountByMovieId.TryGetValue(m.ID, out var c) ? c : 0))
                .ToList();
        }

        private void SortOption_Changed(object sender, RoutedEventArgs e) => ApplyFilterAndSort();

        private void MovieCard_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is not Border border) return;

            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 29, 185, 84));
            border.Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48));
            border.RenderTransform = new ScaleTransform { ScaleX = 1.03, ScaleY = 1.03 };
        }

        private void MovieCard_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is not Border border) return;

            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
            border.Background = new SolidColorBrush(Color.FromArgb(255, 42, 42, 42));
            border.RenderTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
        }

        private void UndiscountedMoviesGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not MovieCatalogItem item)
                return;

            NavigateToMovieDetail(item.Movie);
        }

        private void NavigateToMovieDetail(Movie movie)
        {
            var navPage = FindAncestorNavigationPage();
            navPage?.NavigateToMovieDetail(movie, showOnlySales: false);
        }

        private NavigationPage? FindAncestorNavigationPage()
        {
            UIElement? current = this;
            while (current != null)
            {
                if (current is NavigationPage navPage)
                    return navPage;

                current = VisualTreeHelper.GetParent(current) as UIElement;
            }

            return null;
        }

    }
}