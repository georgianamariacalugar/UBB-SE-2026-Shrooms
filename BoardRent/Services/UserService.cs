using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardRent.DTOs;
using BoardRent.Repositories;
using BoardRent.Utils;

namespace BoardRent.Services
{
    class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResult<UserProfileDto>> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            Debug.WriteLine($"Avatar path: {user.AvatarUrl}");
            Debug.WriteLine($"File exists: {File.Exists(user.AvatarUrl)}");

            return new ServiceResult<UserProfileDto>
            {
                Data = new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = user.AvatarUrl,
                    Role = user.Roles.FirstOrDefault()?.Name ?? "User",
                    IsSuspended = user.IsSuspended,
                    Country = user.Country,
                    City = user.City,
                    StreetName = user.StreetName,
                    StreetNumber = user.StreetNumber
                }
            };
        }

        public async Task<ServiceResult<bool>> UpdateProfileAsync(Guid userId, UserProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.DisplayName) || dto.DisplayName.Length < 2 || dto.DisplayName.Length > 50)
                errors.Add("DisplayName|Display name must be between 2 and 50 characters long");

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.PhoneNumber, @"^\+?\d{7,15}$"))
                    errors.Add("PhoneNumber|Phone number format is invalid");
            }

            if (string.IsNullOrWhiteSpace(dto.StreetNumber) || dto.StreetNumber.Length > 10)
                errors.Add("StreetNumber|Street number must be a valid value");

            if (errors.Any())
                return ServiceResult<bool>.Fail(string.Join(";", errors));

            user.DisplayName = dto.DisplayName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Country = dto.Country;
            user.City = dto.City;
            user.StreetName = dto.StreetName;
            user.StreetNumber = dto.StreetNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<bool>.Fail("User not found");

            if (!PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash))
                return ServiceResult<bool>.Fail("Current password is incorrect");

            if (newPassword.Length < 8)
                return ServiceResult<bool>.Fail("New password too short");

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            SessionContext.GetInstance().Clear();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<string> UploadAvatarAsync(Guid userId, string filePath)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var fileName = $"{userId}_{Path.GetFileName(filePath)}";
            var saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BoardRent", "Avatars");
            Directory.CreateDirectory(saveFolder);
            var savePath = Path.Combine(saveFolder, fileName);
            File.Copy(filePath, savePath, true);

            user.AvatarUrl = savePath;
            //Debug.WriteLine($"Avatar path: {user.AvatarUrl}");
            //Debug.WriteLine($"File exists: {File.Exists(user.AvatarUrl)}");

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return savePath;
        }

        public async Task RemoveAvatarAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.AvatarUrl = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }
    }
}
