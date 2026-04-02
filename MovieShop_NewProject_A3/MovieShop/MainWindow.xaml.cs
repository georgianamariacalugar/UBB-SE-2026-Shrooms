using Microsoft.UI.Xaml;
using MovieShop.ViewModels;
using MovieShop.Views;

namespace MovieShop
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.Content = new MovieShop.Views.NavigationPage();

        }

        public void ShowMovieShopPage()
        {
            GlobalSaleBanner.Visibility=Visibility.Visible;
            MainContentArea.Content = new MovieShopView();
        }
       
    }
}