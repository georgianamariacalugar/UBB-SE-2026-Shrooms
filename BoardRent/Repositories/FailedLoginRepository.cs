using BoardRent.Data;
using BoardRent.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace BoardRent.Repositories
{
    public class FailedLoginRepository : IFailedLoginRepository
    {
        private readonly AppDbContext _dbContext;

        public FailedLoginRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FailedLoginAttempt?> GetByUserIdAsync(Guid userId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM FailedLoginAttempt WHERE UserId = @UserId";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            if (reader.Read())
            {
                return MapFailedLoginAttempt(reader);
            }

            return null;
        }

        private async Task CreateAsync(FailedLoginAttempt attempt)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO FailedLoginAttempt (UserId, FailedAttempts, LockedUntil)
                VALUES (@UserId, @FailedAttempts, @LockedUntil)";

            command.Parameters.AddWithValue("@UserId", attempt.UserId);
            command.Parameters.AddWithValue("@FailedAttempts", attempt.FailedAttempts);
            command.Parameters.AddWithValue("@LockedUntil", attempt.LockedUntil ?? (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        private async Task UpdateAsync(FailedLoginAttempt attempt)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE FailedLoginAttempt 
                SET FailedAttempts = @FailedAttempts, LockedUntil = @LockedUntil
                WHERE UserId = @UserId";

            command.Parameters.AddWithValue("@UserId", attempt.UserId);
            command.Parameters.AddWithValue("@FailedAttempts", attempt.FailedAttempts);
            command.Parameters.AddWithValue("@LockedUntil", attempt.LockedUntil ?? (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        private async Task DeleteAsync(Guid userId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM FailedLoginAttempt WHERE UserId = @UserId";
            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task IncrementAsync(Guid userId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                IF EXISTS (SELECT 1 FROM FailedLoginAttempt WHERE UserId = @UserId)
                    UPDATE FailedLoginAttempt SET FailedAttempts = FailedAttempts + 1, LockedUntil = CASE WHEN FailedAttempts + 1 >= 5 THEN DATEADD(minute, 15, GETUTCDATE()) ELSE NULL END WHERE UserId = @UserId
                ELSE
                    INSERT INTO FailedLoginAttempt (UserId, FailedAttempts, LockedUntil) VALUES (@UserId, 1, NULL)";

            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task ResetAsync(Guid userId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE FailedLoginAttempt 
                SET FailedAttempts = 0, LockedUntil = NULL
                WHERE UserId = @UserId";

            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        private FailedLoginAttempt MapFailedLoginAttempt(SqlDataReader reader)
        {
            return new FailedLoginAttempt
            {
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                FailedAttempts = reader.GetInt32(reader.GetOrdinal("FailedAttempts")),
                LockedUntil = reader.IsDBNull(reader.GetOrdinal("LockedUntil")) ? null : reader.GetDateTime(reader.GetOrdinal("LockedUntil"))
            };
        }
    }
}
