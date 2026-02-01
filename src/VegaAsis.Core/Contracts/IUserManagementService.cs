using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IUserManagementService
    {
        bool IsAdmin { get; }
        Task<bool> CheckAdminStatusAsync();
        Task FetchUsersAsync();
        IReadOnlyList<UserDataDto> Users { get; }
        IReadOnlyList<UserSettingsDto> UserSettingsList { get; }
        bool IsLoadingUsers { get; }
        UserSettingsDto GetUserSettings(Guid userId);
        Task<bool> UpdateUserProfileAsync(Guid userId, string fullName, string phone);
        Task<bool> SaveUserSettingsAsync(UserSettingsDto data);
    }
}
