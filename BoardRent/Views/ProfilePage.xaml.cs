using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using BoardRent.ViewModels;
using BoardRent.Services;
using BoardRent.Repositories;
using BoardRent.Data;
using System.Diagnostics;
using BoardRent.Utils;
using WinRT.Interop;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BoardRent.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class ProfilePage : Page
    {
        public ProfileViewModel ViewModel { get; }

        public ProfilePage()
        {

            this.InitializeComponent();

            var dbContext = new AppDbContext();
            var userRepo = new UserRepository(dbContext);
            var failedLoginRepo = new FailedLoginRepository(dbContext);
            IUserService userService = new UserService(userRepo);
            IAuthService authService = new AuthService(userRepo, failedLoginRepo);
            ViewModel = new ProfileViewModel(userService, authService);

            this.DataContext = ViewModel;
            this.Loaded += async (s, e) =>
            {
                await ViewModel.LoadProfile();
                Debug.WriteLine($"Username after load: {ViewModel.Username}");
            };
        }


        private async void SignOut_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SignOut();

            App.NavigateTo(typeof(LoginPage));
        }

    }
}
