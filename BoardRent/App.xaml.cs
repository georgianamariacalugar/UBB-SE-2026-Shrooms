using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BoardRent.Data;
using BoardRent.Views;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BoardRent
{
    public partial class App : Application
    {
        private static Window _window;
        private static Frame _rootFrame;

        public App()
        {
            InitializeComponent();
        }
        
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _rootFrame = new Frame();
            _window.Content = _rootFrame;
            _window.Activate();

            // Create DB tables on first launch
            var db = new AppDbContext();
            db.EnsureCreated();

            //Start at login page
            NavigateTo(typeof(LoginPage));
        }

        public static void NavigateTo(Type pageType)
        {
            _rootFrame?.Navigate(pageType);
        }

        public static void NavigateBack()
        {
            if (_rootFrame != null && _rootFrame.CanGoBack)
            {
                _rootFrame.GoBack();
            }
        }
    }
}
