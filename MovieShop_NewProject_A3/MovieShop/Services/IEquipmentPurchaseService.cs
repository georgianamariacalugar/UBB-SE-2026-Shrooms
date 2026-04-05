using MovieShop.Models;

namespace MovieShop.Services
{
	public interface IEquipmentPurchaseService
	{
		bool CanAfford(int userId, decimal price);

		void PurchaseEquipment(int equipmentId, int userId, decimal price, string shippingAddress);
	}
}
