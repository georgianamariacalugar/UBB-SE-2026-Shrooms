using Microsoft.Data.SqlClient;
using MovieShop.Repositories;
using System.Collections.Generic;
using System.Linq;
using System;
using MovieShop.Services;
using MovieShop.Models;

public class MovieReviewService : IMovieReviewService
{
    private readonly IDatabaseSingleton _db;
    private readonly IReviewRepository _reviewRepo;

    public MovieReviewService(
        IDatabaseSingleton db,
        IReviewRepository reviewRepo)
    {
        _db = db;
        _reviewRepo = reviewRepo;
    }

    public List<MovieReview> GetReviewsForMovie(int movieId)
    {
        return _reviewRepo.GetReviewsForMovie(movieId);
    }
    public int GetReviewCount(int movieId)
    {
        _db.OpenConnection();
        try
        {
            const string query = @"SELECT COUNT(*) FROM Reviews WHERE MovieID = @mid";
            using var cmd = new SqlCommand(query, _db.Connection);
            cmd.Parameters.AddWithValue("@mid", movieId);

            using var reader = cmd.ExecuteReader();
            int count = (int)cmd.ExecuteScalar();

            return count;
        }
        finally
        {
            _db.CloseConnection();
        }
    }

    public Dictionary<int, int> GetReviewCounts(IEnumerable<int> movieIds)
    {
        var result = new Dictionary<int, int>();

        var ids = movieIds.Distinct().ToList();
        if (ids.Count == 0)
            return result;

        var paramNames = ids.Select((_, i) => $"@id{i}").ToArray();
        var inClause = string.Join(",", paramNames);

        string query = $@"SELECT MovieID, COUNT(*) 
        FROM Reviews 
        WHERE MovieID IN ({inClause}) 
        GROUP BY MovieID";

        _db.OpenConnection();
        try
        {
            using var cmd = new SqlCommand(query, _db.Connection);

            for (int i = 0; i < ids.Count; i++)
                cmd.Parameters.AddWithValue(paramNames[i], ids[i]);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int movieId = reader.GetInt32(0);
                int count = reader.GetInt32(1);

                result[movieId] = count;
            }
        }
        finally
        {
            _db.CloseConnection();
        }

        return result;
    }

    public string BuildStarDistributionTooltip(int movieId)
    {
        var counts = new int[11];

        _db.OpenConnection();
        try
        {
            const string query = @"SELECT StarRating FROM Reviews WHERE MovieID = @mid";
            using var cmd = new SqlCommand(query, _db.Connection);
            cmd.Parameters.AddWithValue("@mid", movieId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var rating = reader.GetInt32(0);
                var bucket = (int)Math.Floor((double)rating);

                if (bucket < 1) bucket = 1;
                if (bucket > 10) bucket = 10;

                counts[bucket]++;
            }
        }
        finally
        {
            _db.CloseConnection();
        }

        int total = counts.Skip(1).Sum();
        if (total == 0)
            return "No reviews yet.";

        var lines = new List<string> { "Rating distribution:" };
        for (int i = 10; i >= 1; i--)
            lines.Add($"{i}: {counts[i]}");

        return string.Join("\n", lines);
    }

    public void AddReview(int movieId, int userId, int rating, string? comment)
    {
        if (userId <= 0)
            throw new InvalidOperationException("You must be logged in to add a review.");

        if (rating < 1 || rating > 10)
            throw new InvalidOperationException("Rating must be between 1 and 10.");

        _reviewRepo.AddReview(movieId, userId, rating, comment);
    }
}