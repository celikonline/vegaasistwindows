using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Data.Entities;

namespace VegaAsis.Data.Services
{
    public class WebUserService : IWebUserService
    {
        private readonly VegaAsisDbContext _db;
        private readonly IAuthService _authService;

        public WebUserService(VegaAsisDbContext db, IAuthService authService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<IReadOnlyList<WebUserDto>> GetAllAsync(Guid? userId = null, bool? unlicensedAgentOnly = null)
        {
            var query = _db.WebUsers.OrderByDescending(w => w.CreatedAt).AsQueryable();
            if (userId.HasValue)
            {
                query = query.Where(w => w.UserId == userId.Value);
            }
            if (unlicensedAgentOnly.HasValue)
            {
                query = query.Where(w => w.UnlicensedAgentOnly == unlicensedAgentOnly.Value);
            }

            var list = await query.ToListAsync().ConfigureAwait(false);
            return list.Select(ToDto).ToList();
        }

        public async Task<WebUserDto> CreateAsync(WebUserDto dto)
        {
            var userId = _authService.GetCurrentUserId;
            if (!userId.HasValue)
            {
                throw new InvalidOperationException("Giriş yapmalısınız.");
            }

            var entity = new WebUser
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                WebUsername = dto.WebUsername ?? string.Empty,
                Username = dto.Username ?? string.Empty,
                FullName = dto.FullName,
                Password = dto.Password,
                Gsm = dto.Gsm,
                Email = dto.Email,
                Description = dto.Description,
                IsLicensed = dto.IsLicensed,
                LicenseOfferOnly = dto.LicenseOfferOnly,
                LicenseOnlinePolicy = dto.LicenseOnlinePolicy,
                LicenseCompanyScreen = dto.LicenseCompanyScreen,
                UnlicensedAgentOnly = dto.UnlicensedAgentOnly,
                ResponsibleStaff = dto.ResponsibleStaff,
                BannedCompanies = dto.BannedCompanies,
                InternalBans = dto.InternalBans,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.WebUsers.Add(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<WebUserDto> UpdateAsync(Guid id, WebUserDto dto)
        {
            var entity = await _db.WebUsers.FindAsync(id).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            entity.WebUsername = dto.WebUsername ?? entity.WebUsername;
            entity.Username = dto.Username ?? entity.Username;
            entity.FullName = dto.FullName;
            entity.Password = dto.Password ?? entity.Password;
            entity.Gsm = dto.Gsm;
            entity.Email = dto.Email;
            entity.Description = dto.Description;
            entity.IsLicensed = dto.IsLicensed;
            entity.LicenseOfferOnly = dto.LicenseOfferOnly;
            entity.LicenseOnlinePolicy = dto.LicenseOnlinePolicy;
            entity.LicenseCompanyScreen = dto.LicenseCompanyScreen;
            entity.UnlicensedAgentOnly = dto.UnlicensedAgentOnly;
            entity.ResponsibleStaff = dto.ResponsibleStaff;
            entity.BannedCompanies = dto.BannedCompanies;
            entity.InternalBans = dto.InternalBans;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.WebUsers.FindAsync(id).ConfigureAwait(false);
            if (entity == null)
            {
                return false;
            }

            _db.WebUsers.Remove(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        private static WebUserDto ToDto(WebUser entity)
        {
            return new WebUserDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                WebUsername = entity.WebUsername,
                Username = entity.Username,
                FullName = entity.FullName,
                Password = entity.Password,
                Gsm = entity.Gsm,
                Email = entity.Email,
                Description = entity.Description,
                IsLicensed = entity.IsLicensed,
                LicenseOfferOnly = entity.LicenseOfferOnly,
                LicenseOnlinePolicy = entity.LicenseOnlinePolicy,
                LicenseCompanyScreen = entity.LicenseCompanyScreen,
                UnlicensedAgentOnly = entity.UnlicensedAgentOnly,
                ResponsibleStaff = entity.ResponsibleStaff,
                BannedCompanies = entity.BannedCompanies,
                InternalBans = entity.InternalBans,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
