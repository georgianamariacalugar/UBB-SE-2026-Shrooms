using MovieShop.Models;
using MovieShop.Repositories;
using System;

namespace MovieShop.Services
{
    public class EquipmentPurchaseService : IEquipmentPurchaseService
    {
        private readonly IEquipmentRepository _equipmentRepo;
        private readonly IUserRepository _userRepo;

        public EquipmentPurchaseService(IEquipmentRepository equipmentRepo, IUserRepository userRepo)
        {
            _equipmentRepo = equipmentRepo;
            _userRepo = userRepo;
        }

        public bool CanAfford(int userId, decimal price)
        {
            var balance = _userRepo.GetBalance(userId);
            SessionManager.CurrentUserBalance = balance;
            return balance >= price;
        }

        public void PurchaseEquipment(int equipmentId, int userId, decimal price, string shippingAddress)
        {
            var balance = _userRepo.GetBalance(userId);
            if (balance < price)
                throw new InvalidOperationException(
                    $"Insufficient funds. Balance: {balance:C} — Price: {price:C}");

            _equipmentRepo.PurchaseEquipment(equipmentId, userId, price, shippingAddress);

            SessionManager.CurrentUserBalance = _userRepo.GetBalance(userId);
        }
    }
}