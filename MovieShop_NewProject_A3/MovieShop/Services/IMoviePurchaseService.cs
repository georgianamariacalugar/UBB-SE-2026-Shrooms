using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieShop.Models;

namespace MovieShop.Services
{
    public record BuyButtonProps(string Content, bool IsEnabled, string? ToolTip, double Opacity);
    public interface IMoviePurchaseService
    {

        BuyButtonProps GetBuyButtonProps(
            Movie movie,
            int userId,
            bool isLoggedIn,
            decimal balance
        );

        void PurchaseMovie(int userId, Movie movie);
    }
}