using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Data.Entities;

namespace VegaAsis.Data.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly VegaAsisDbContext _db;
        private readonly IAuthService _authService;
        private readonly List<UserDataDto> _users = new List<UserDataDto>();
        private readonly List<UserSettingsDto> _userSettingsList = new List<UserSettingsDto>();
        private bool _isLoadingUsers;
        private bool _isAdmin;

        public UserManagementService(VegaAsisDbContext db, IAuthService authService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public bool IsAdmin
        {
            get { return _isAdmin; }
        }

        public IReadOnlyList<UserDataDto> Users
        {
            get { return _users; }
        }

        public IReadOnlyList<UserSettingsDto> UserSettingsList
        {
            get { return _userSettingsList; }
        }

        public bool IsLoadingUsers
        {
            get { return _isLoadingUsers; }
        }

        public async Task<bool> CheckAdminStatusAsync()
        {
            var userId = _authService.GetCurrentUserId;
            if (!userId.HasValue)
            {
                _isAdmin = false;
                return false;
            }

            var hasAdmin = await _db.UserRoles
                .AnyAsync(r => r.UserId == userId.Value && r.Role == "admin")
                .ConfigureAwait(false);
            _isAdmin = hasAdmin;
            return hasAdmin;
        }

        public async Task FetchUsersAsync()
        {
            var userId = _authService.GetCurrentUserId;
            if (!userId.HasValue)
            {
                return;
            }

            var isAdmin = await CheckAdminStatusAsync().ConfigureAwait(false);
            if (!isAdmin)
            {
                _users.Clear();
                _userSettingsList.Clear();
                return;
            }

            _isLoadingUsers = true;
            try
            {
                var profiles = await _db.Profiles.ToListAsync().ConfigureAwait(false);
                _users.Clear();
                _users.AddRange(profiles.Select(p => new UserDataDto
                {
                    UserId = p.UserId,
                    Email = p.Email,
                    FullName = p.FullName,
                    Phone = p.Phone,
                    CompanyName = p.CompanyName,
                    Role = p.Role
                }));

                var settingsEntities = await _db.UserSettings.ToListAsync().ConfigureAwait(false);
                _userSettingsList.Clear();
                _userSettingsList.AddRange(settingsEntities.Select(ToUserSettingsDto));
            }
            finally
            {
                _isLoadingUsers = false;
            }
        }

        public UserSettingsDto GetUserSettings(Guid userId)
        {
            return _userSettingsList.FirstOrDefault(s => s.UserId == userId);
        }

        public async Task<bool> UpdateUserProfileAsync(Guid userId, string fullName, string phone)
        {
            if (!_isAdmin)
            {
                return false;
            }

            try
            {
                var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId).ConfigureAwait(false);
                if (profile == null)
                {
                    return false;
                }

                profile.FullName = fullName ?? profile.FullName;
                profile.Phone = phone ?? profile.Phone;
                profile.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync().ConfigureAwait(false);

                var dto = _users.FirstOrDefault(u => u.UserId == userId);
                if (dto != null)
                {
                    dto.FullName = profile.FullName;
                    dto.Phone = profile.Phone;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveUserSettingsAsync(UserSettingsDto data)
        {
            if (!_isAdmin || data == null)
            {
                return false;
            }

            try
            {
                var existing = await _db.UserSettings
                    .FirstOrDefaultAsync(s => s.UserId == data.UserId)
                    .ConfigureAwait(false);

                if (existing != null)
                {
                    existing.Gsm = data.Gsm ?? string.Empty;
                    existing.Description = data.Description ?? string.Empty;
                    existing.GroupName = data.GroupName ?? string.Empty;
                    existing.IsOfficeWorker = data.IsOfficeWorker;
                    existing.CanViewWebService = data.CanViewWebService;
                    existing.CanSaveSession = data.CanSaveSession;
                    existing.HideCompanyDetails = data.HideCompanyDetails;
                    existing.AllowedIps = SerializeStringList(data.AllowedIps);
                    existing.BannedCompanies = SerializeStringList(data.BannedCompanies);
                    existing.CompanyRestrictions = SerializeCompanyRestrictions(data.CompanyRestrictions);
                    existing.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    var entity = new UserSettings
                    {
                        Id = Guid.NewGuid(),
                        UserId = data.UserId,
                        Gsm = data.Gsm ?? string.Empty,
                        Description = data.Description ?? string.Empty,
                        GroupName = data.GroupName ?? string.Empty,
                        IsOfficeWorker = data.IsOfficeWorker,
                        CanViewWebService = data.CanViewWebService,
                        CanSaveSession = data.CanSaveSession,
                        HideCompanyDetails = data.HideCompanyDetails,
                        AllowedIps = SerializeStringList(data.AllowedIps),
                        BannedCompanies = SerializeStringList(data.BannedCompanies),
                        CompanyRestrictions = SerializeCompanyRestrictions(data.CompanyRestrictions),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _db.UserSettings.Add(entity);
                }

                await _db.SaveChangesAsync().ConfigureAwait(false);
                await FetchUsersAsync().ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static UserSettingsDto ToUserSettingsDto(UserSettings s)
        {
            return new UserSettingsDto
            {
                Id = s.Id,
                UserId = s.UserId,
                Gsm = s.Gsm,
                Description = s.Description,
                GroupName = s.GroupName,
                IsOfficeWorker = s.IsOfficeWorker,
                CanViewWebService = s.CanViewWebService,
                CanSaveSession = s.CanSaveSession,
                HideCompanyDetails = s.HideCompanyDetails,
                AllowedIps = DeserializeStringList(s.AllowedIps),
                BannedCompanies = DeserializeStringList(s.BannedCompanies),
                CompanyRestrictions = DeserializeCompanyRestrictions(s.CompanyRestrictions)
            };
        }

        private static string SerializeStringList(List<string> list)
        {
            if (list == null || list.Count == 0)
            {
                return "[]";
            }

            return new JavaScriptSerializer().Serialize(list);
        }

        private static List<string> DeserializeStringList(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<string>();
            }

            try
            {
                var list = new JavaScriptSerializer().Deserialize<List<string>>(json);
                return list ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private static string SerializeCompanyRestrictions(Dictionary<string, Dictionary<string, bool>> restrictions)
        {
            if (restrictions == null || restrictions.Count == 0)
            {
                return "{}";
            }

            return new JavaScriptSerializer().Serialize(restrictions);
        }

        private static Dictionary<string, Dictionary<string, bool>> DeserializeCompanyRestrictions(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Dictionary<string, Dictionary<string, bool>>();
            }

            try
            {
                var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, Dictionary<string, bool>>>(json);
                return dict ?? new Dictionary<string, Dictionary<string, bool>>();
            }
            catch
            {
                return new Dictionary<string, Dictionary<string, bool>>();
            }
        }
    }
}
