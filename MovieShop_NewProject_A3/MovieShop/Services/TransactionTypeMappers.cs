using System;

namespace MovieShop.Services
{
    public static class TransactionTypeMapper
    {
        public static string ToDisplayString(string type)
        {
            return type switch
            {
                "MoviePurchase" => "🎬 Movie Purchase",
                "TicketPurchase" => "🎟️ Ticket Purchase",
                "EquipmentPurchase" => "🎥 Equipment Purchase",
                "TopUp" => "💳 Top-Up",
                "EquipmentSale" => "💰 Equipment Sale",
                _ => "Unknown Transaction"
            };
        }

        public static string StatusToDisplayString(string status)
        {
            return status switch
            {
                "Pending" => "⏳ Pending",
                "Completed" => "✅ Completed",
                "Failed" => "❌ Failed",
                _ => "Unknown Status"
            };
        }

        public static string FormatAmount(decimal amount)
        {
            return amount >= 0
                ? $"+${amount:0.00}"
                : $"-${Math.Abs(amount):0.00}";
        }
    }
}