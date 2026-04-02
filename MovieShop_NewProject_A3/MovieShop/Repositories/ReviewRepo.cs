using Microsoft.Data.SqlClient;
using MovieShop.Models;
using System;
using System.Collections.Generic;

namespace MovieShop.Repositories
{
    public sealed class ReviewRepo
    {
        private readonly DatabaseSingleton _db = DatabaseSingleton.Instance;

        public List<MovieReview> GetReviewsForMovie(int movieId)
        {
            var list = new List<MovieReview>();
            const string query = @"SELECT r.ID, r.MovieID, r.UserID, u.Username, r.StarRating, r.Comment, r.CreatedAt
                                   FROM Reviews r
                                   JOIN Users u ON u.ID = r.UserID
                                   WHERE r.MovieID = @mid
                                   ORDER BY r.CreatedAt DESC, r.ID DESC";

            _db.OpenConnection();
            try
            {
                using var cmd = new SqlCommand(query, _db.Connection);
                cmd.Parameters.AddWithValue("@mid", movieId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new MovieReview
                    {
                        ID = reader.GetInt32(0),
                        MovieID = reader.GetInt32(1),
                        UserID = reader.GetInt32(2),
                        Username = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        StarRating = reader.GetInt32(4),
                        Comment = reader.IsDBNull(5) ? null : reader.GetString(5),
                        CreatedAt = reader.GetDateTime(6)
                    });
                }
            }
            finally
            {
                _db.CloseConnection();
            }

            return list;
        }

        public void AddReview(int movieId, int userId, int starRating, string? comment)
        {
            const string query = @"INSERT INTO Reviews (MovieID, UserID, StarRating, Comment) VALUES (@mid, @uid, @star, @comment)";

            _db.OpenConnection();
            try
            {
                using var cmd = new SqlCommand(query, _db.Connection);
                cmd.Parameters.AddWithValue("@mid", movieId);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@star", starRating);
                cmd.Parameters.AddWithValue("@comment", string.IsNullOrWhiteSpace(comment) ? (object)DBNull.Value : comment);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _db.CloseConnection();
            }
        }
    }
}

