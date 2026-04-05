using MovieShop.Models;
using MovieShop.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MovieShop.ViewModels
{
    public class SellEquipmentViewModel : INotifyPropertyChanged
    {
        private readonly IEquipmentRepository _repo = App.Services.GetRequiredService<IEquipmentRepository>();

        private string _newItemTitle = string.Empty;
        private string _newItemDesc = string.Empty;
        private string _priceInput = string.Empty;
        private decimal _validatedPrice;
        private string _priceErrorMessage = string.Empty;
        private bool _canPost;

        public string NewItemTitle
        {
            get => _newItemTitle;
            set { _newItemTitle = value; OnPropertyChanged(); ValidateForm(); }
        }

        public string NewItemDesc
        {
            get => _newItemDesc;
            set { _newItemDesc = value; OnPropertyChanged(); ValidateForm(); }
        }

        public string PriceInput
        {
            get => _priceInput;
            set { _priceInput = value; OnPropertyChanged(); ValidateForm(); }
        }

        public string PriceErrorMessage
        {
            get => _priceErrorMessage;
            set { _priceErrorMessage = value; OnPropertyChanged(); }
        }

        public bool CanPost
        {
            get => _canPost;
            set { _canPost = value; OnPropertyChanged(); }
        }

        public decimal ValidatedPrice => _validatedPrice;

        public void SubmitListing(string? category, string? condition, string imageUrl)
        {
            var newItem = new Equipment
            {
                SellerID = SessionManager.CurrentUserID,
                Title = NewItemTitle,
                Description = NewItemDesc,
                Price = ValidatedPrice,
                Category = category,
                Condition = condition,
                ImageUrl = imageUrl,
                Status = EquipmentStatus.Available
            };

            _repo.ListItem(newItem);
        }

        private void ValidateForm()
        {
            bool isPriceValid = decimal.TryParse(_priceInput, out decimal result);
            bool isTitleValid = !string.IsNullOrWhiteSpace(_newItemTitle);

            if (!isPriceValid && !string.IsNullOrEmpty(_priceInput))
            {
                PriceErrorMessage = "Please enter a valid numeric price!";
                CanPost = false;
                return;
            }

            if (isPriceValid && result <= 0)
            {
                PriceErrorMessage = "Price must be greater than 0!";
                CanPost = false;
                return;
            }

            if (isPriceValid && isTitleValid)
            {
                _validatedPrice = result;
                PriceErrorMessage = string.Empty;
                CanPost = true;
            }
            else
            {
                CanPost = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}