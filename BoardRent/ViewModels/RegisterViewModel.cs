using BoardRent.DTOs;
using BoardRent.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace BoardRent.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        [ObservableProperty] private string displayName = string.Empty;
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string confirmPassword = string.Empty;
        [ObservableProperty] private string phoneNumber = string.Empty;
        [ObservableProperty] private string country = string.Empty;
        [ObservableProperty] private string city = string.Empty;
        [ObservableProperty] private string streetName = string.Empty;
        [ObservableProperty] private string streetNumber = string.Empty;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(DisplayName) || string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(Country) ||
                string.IsNullOrWhiteSpace(City) || string.IsNullOrWhiteSpace(StreetName) ||
                string.IsNullOrWhiteSpace(StreetNumber))
            {
                ErrorMessage = "Please fill in all the fields.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match!";
                return;
            }

            IsLoading = true;

            var registerDto = new RegisterDto
            {
                DisplayName = this.DisplayName,
                Username = this.Username,
                Email = this.Email,
                Password = this.Password,
                ConfirmPassword = this.ConfirmPassword,
                PhoneNumber = this.PhoneNumber,
                Country = this.Country,
                City = this.City,
                StreetName = this.StreetName,
                StreetNumber = this.StreetNumber
            };

            var result = await _authService.RegisterAsync(registerDto);

            if (result.Success)
            {
                App.NavigateBack();
            }
            else
            {
                ErrorMessage = result.Error ?? "Registration failed.";
            }

            IsLoading = false;
        }

        [RelayCommand]
        private void GoToLogin()
        {
            App.NavigateBack();
        }
    }
}