using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardRent.Domain;
using BoardRent.DTOs;
using BoardRent.Repositories;
using BoardRent.Utils;

namespace BoardRent.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFailedLoginRepository _failedLoginRepository;

        public AdminService(IUserRepository userRepository, IFailedLoginRepository failedLoginRepository)
        {
            _userRepository = userRepository;
            _failedLoginRepository = failedLoginRepository;
        }

        private bool IsAuthorized()
        {
            var session = SessionContext.GetInstance();
            return session.IsLoggedIn && session.Role == "Administrator";
        }

        public async Task<ServiceResult<List<UserProfileDto>>> GetAllUsersAsync(int page, int pageSize)
        {
            if (!IsAuthorized())
                return ServiceResult<List<UserProfileDto>>.Fail("Unauthorized access.");

            var users = await _userRepository.GetAllAsync(page, pageSize);
            
            var dtos = users.Select(u => new UserProfileDto
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                AvatarUrl = u.AvatarUrl,
                Role = u.Roles?.FirstOrDefault()?.Name ?? "User",
                IsSuspended = u.IsSuspended,
                Country = u.Country,
                City = u.City,
                StreetName = u.StreetName,
                StreetNumber = u.StreetNumber
            }).ToList();

            return ServiceResult<List<UserProfileDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<bool>> SuspendUserAsync(Guid userId)
        {
            if (!IsAuthorized())
                return ServiceResult<bool>.Fail("Unauthorized access.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<bool>.Fail("User not found.");

            user.IsSuspended = true;
            await _userRepository.UpdateAsync(user);

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> UnsuspendUserAsync(Guid userId)
        {
            if (!IsAuthorized())
                return ServiceResult<bool>.Fail("Unauthorized access.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<bool>.Fail("User not found.");

            user.IsSuspended = false;
            await _userRepository.UpdateAsync(user);

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> ResetPasswordAsync(Guid userId, string newPassword)
        {
            if (!IsAuthorized())
                return ServiceResult<bool>.Fail("Unauthorized access.");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return ServiceResult<bool>.Fail("Password must be at least 6 characters long.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<bool>.Fail("User not found.");

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> UnlockAccountAsync(Guid userId)
        {
            if (!IsAuthorized())
                return ServiceResult<bool>.Fail("Unauthorized access.");

            await _failedLoginRepository.ResetAsync(userId);

            return ServiceResult<bool>.Ok(true);
        }
    }
}
