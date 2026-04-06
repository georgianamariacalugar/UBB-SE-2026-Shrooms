using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using MovieShop.Models;
using MovieShop.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MovieShop.ViewModels
{
    public class ConfirmReceiptViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly IUserRepository _userRepo = App.Services.GetRequiredService<IUserRepository>();
        private int _currentUserID;

        private Transaction _transaction;
        public Transaction Transaction
        {
            get => _transaction;
            set { _transaction = value; OnPropertyChanged(nameof(Transaction)); }
        }

        private decimal _sellerBalance;
        public decimal SellerBalance
        {
            get => _sellerBalance;
            set { _sellerBalance = value; OnPropertyChanged(nameof(SellerBalance)); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        private string _successMessage = string.Empty;
        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(nameof(SuccessMessage)); }
        }

        private bool _isConfirmButtonVisible;
        public bool IsConfirmButtonVisible
        {
            get => _isConfirmButtonVisible;
            set { _isConfirmButtonVisible = value; OnPropertyChanged(nameof(IsConfirmButtonVisible)); }
        }

        private bool _isSuccessVisible;
        public bool IsSuccessVisible
        {
            get => _isSuccessVisible;
            set { _isSuccessVisible = value; OnPropertyChanged(nameof(IsSuccessVisible)); }
        }

        public IRelayCommand ConfirmReceiptCommand { get; }
        public IRelayCommand DismissSuccessCommand { get; }

        public ConfirmReceiptViewModel(int userID, Transaction transaction, decimal sellerBalance)
        {
            _currentUserID = userID;
            _transaction = transaction;
            _sellerBalance = sellerBalance;

            IsConfirmButtonVisible = transaction.Status == "Pending";

            ConfirmReceiptCommand = new RelayCommand(ConfirmReceipt);
            DismissSuccessCommand = new RelayCommand(DismissSuccess);
        }

        private void ConfirmReceipt()
        {
            ErrorMessage = string.Empty;

            if (Transaction == null)
            {
                ErrorMessage = "No transaction found.";
                return;
            }

            if (Transaction.Status != "Pending")
            {
                ErrorMessage = "This transaction has already been completed.";
                return;
            }

            if (Transaction.BuyerID.ID != _currentUserID)
            {
                ErrorMessage = "Only the buyer can confirm receipt.";
                return;
            }

            ReleaseEscrowToSeller();
            UpdateTransactionStatus("Completed");

            IsConfirmButtonVisible = false;
            IsSuccessVisible = true;
            SuccessMessage = "Receipt confirmed! The seller has been paid.";
        }

        private void ReleaseEscrowToSeller()
        {
            if (Transaction.SellerID == null) return;

            var currentBalance = _userRepo.GetBalance(Transaction.SellerID.ID);
            var newBalance = currentBalance + Transaction.Amount;
            _userRepo.UpdateBalance(Transaction.SellerID.ID, newBalance);
            SellerBalance = newBalance;
        }

        private void UpdateTransactionStatus(string newStatus)
        {
            Transaction.Status = newStatus;
            OnPropertyChanged(nameof(Transaction));
            System.Diagnostics.Debug.WriteLine(
                $"[ConfirmReceiptViewModel] Status set to '{newStatus}' locally. " +
                "Add UpdateStatus to ITransactionRepository to persist to DB.");
        }

        private void DismissSuccess()
        {
            IsSuccessVisible = false;
            SuccessMessage = string.Empty;
        }
    }
}