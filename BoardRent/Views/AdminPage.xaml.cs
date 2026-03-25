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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using System.ComponentModel;
using BoardRent.ViewModels;
using BoardRent.Utils;

namespace BoardRent.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminPage : Page, INotifyPropertyChanged
    {
        public AdminViewModel ViewModel { get; }

        public AdminPage()
        {
            InitializeComponent();
            // TODO: Resolve proper DI once unblocked
            ViewModel = new AdminViewModel(null);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public bool IsUnauthorized => !SessionContext.GetInstance().IsLoggedIn || SessionContext.GetInstance().Role != "Administrator";
        public Visibility IsAuthorizedVisibility => IsUnauthorized ? Visibility.Collapsed : Visibility.Visible;
        public bool IsErrorVisible => ViewModel != null && !string.IsNullOrEmpty(ViewModel.ErrorMessage);

        public event PropertyChangedEventHandler PropertyChanged;

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AdminViewModel.ErrorMessage))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsErrorVisible)));
            }
        }

        private void OnSignOutClicked(object sender, RoutedEventArgs e)
        {
            SessionContext.GetInstance().Clear();
            // App.NavigateTo(typeof(LoginPage));
        }
    }
}
