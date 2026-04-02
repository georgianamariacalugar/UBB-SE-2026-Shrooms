using System;

namespace MovieShop.Models
{
    public sealed class MovieReview
    {
        public int ID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; } = "";
        public decimal StarRating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string DisplayStarRating => $"{StarRating:0.0}/10";
        public string DisplayCreatedAt => CreatedAt.ToString("yyyy-MM-dd HH:mm");
    }
}

