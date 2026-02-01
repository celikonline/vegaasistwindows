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
    public class CompanySettingsService : ICompanySettingsService
    {
        private readonly VegaAsisDbContext _db;
        private readonly IAuthService _authService;
        private List<CompanySettings> _settingsCache = new List<CompanySettings>();

        public CompanySettingsService(VegaAsisDbContext db, IAuthService authService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task LoadSettingsAsync()
        {
            var userId = _authService.GetCurrentUserId;
            if (!userId.HasValue)
            {
                _settingsCache = new List<CompanySettings>();
                return;
            }

            _settingsCache = await _db.CompanySettings
                .Where(c => c.UserId == userId.Value)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<List<string>> GetSelectedCompaniesAsync()
        {
            if (_settingsCache.Count == 0)
            {
                await LoadSettingsAsync().ConfigureAwait(false);
            }

            return _settingsCache.Select(s => s.CompanyName).OrderBy(x => x).ToList();
        }

        public CompanySettingsDto GetCompanySetting(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                return null;
            }

            var entity = _settingsCache.FirstOrDefault(s =>
                string.Equals(s.CompanyName, companyName.Trim(), StringComparison.OrdinalIgnoreCase));

            return entity == null ? null : ToDto(entity);
        }

        public async Task<bool> SaveCompanySettingAsync(CompanySettingsDto dto)
        {
            var userId = _authService.GetCurrentUserId;
            if (!userId.HasValue || string.IsNullOrWhiteSpace(dto?.CompanyName))
            {
                return false;
            }

            try
            {
                var existing = _settingsCache.FirstOrDefault(s =>
                    string.Equals(s.CompanyName, dto.CompanyName.Trim(), StringComparison.OrdinalIgnoreCase));

                if (existing != null)
                {
                    existing.ProxyAddress = dto.ProxyAddress ?? string.Empty;
                    existing.ProxyUser = dto.ProxyUser ?? string.Empty;
                    existing.ProxyPassword = dto.ProxyPassword ?? string.Empty;
                    existing.CompanyUsername = dto.CompanyUsername ?? string.Empty;
                    existing.CompanyPassword = dto.CompanyPassword ?? string.Empty;
                    existing.GoogleSecretKey = dto.GoogleSecretKey ?? string.Empty;
                    existing.KaskoSpecialDiscount = dto.KaskoSpecialDiscount;
                    existing.IpAddresses = dto.IpAddresses ?? string.Empty;
                    existing.TrafikTeklifiKaydet = dto.TrafikTeklifiKaydet;
                    existing.KaskoTeklifiKaydet = dto.KaskoTeklifiKaydet;
                    existing.OtoSessionKaydet = dto.OtoSessionKaydet;
                    existing.IkameAracHizmeti = dto.IkameAracHizmeti;
                    existing.KesilenPaketGetir = dto.KesilenPaketGetir;
                    existing.CompanyBans = SerializeBans(dto.CompanyBans);
                }
                else
                {
                    var entity = new CompanySettings
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId.Value,
                        CompanyName = dto.CompanyName.Trim(),
                        ProxyAddress = dto.ProxyAddress ?? string.Empty,
                        ProxyUser = dto.ProxyUser ?? string.Empty,
                        ProxyPassword = dto.ProxyPassword ?? string.Empty,
                        CompanyUsername = dto.CompanyUsername ?? string.Empty,
                        CompanyPassword = dto.CompanyPassword ?? string.Empty,
                        GoogleSecretKey = dto.GoogleSecretKey ?? string.Empty,
                        KaskoSpecialDiscount = dto.KaskoSpecialDiscount,
                        IpAddresses = dto.IpAddresses ?? string.Empty,
                        TrafikTeklifiKaydet = dto.TrafikTeklifiKaydet,
                        KaskoTeklifiKaydet = dto.KaskoTeklifiKaydet,
                        OtoSessionKaydet = dto.OtoSessionKaydet,
                        IkameAracHizmeti = dto.IkameAracHizmeti,
                        KesilenPaketGetir = dto.KesilenPaketGetir,
                        CompanyBans = SerializeBans(dto.CompanyBans),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _db.CompanySettings.Add(entity);
                    _settingsCache.Add(entity);
                }

                await _db.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCompanySettingAsync(string companyName)
        {
            var userId = _authService.GetCurrentUserId;
            if (!userId.HasValue || string.IsNullOrWhiteSpace(companyName))
            {
                return false;
            }

            try
            {
                var entity = await _db.CompanySettings
                    .FirstOrDefaultAsync(c =>
                        c.UserId == userId.Value &&
                        c.CompanyName == companyName.Trim())
                    .ConfigureAwait(false);

                if (entity == null)
                {
                    return false;
                }

                _db.CompanySettings.Remove(entity);
                _settingsCache.RemoveAll(s =>
                    string.Equals(s.CompanyName, companyName.Trim(), StringComparison.OrdinalIgnoreCase));
                await _db.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string SerializeBans(Dictionary<string, bool> bans)
        {
            if (bans == null || bans.Count == 0)
            {
                return "{}";
            }

            return new JavaScriptSerializer().Serialize(bans);
        }

        private static Dictionary<string, bool> DeserializeBans(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Dictionary<string, bool>();
            }

            try
            {
                var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, bool>>(json);
                return dict ?? new Dictionary<string, bool>();
            }
            catch
            {
                return new Dictionary<string, bool>();
            }
        }

        private static CompanySettingsDto ToDto(CompanySettings e)
        {
            return new CompanySettingsDto
            {
                Id = e.Id,
                CompanyName = e.CompanyName,
                ProxyAddress = e.ProxyAddress ?? string.Empty,
                ProxyUser = e.ProxyUser ?? string.Empty,
                ProxyPassword = e.ProxyPassword ?? string.Empty,
                CompanyUsername = e.CompanyUsername ?? string.Empty,
                CompanyPassword = e.CompanyPassword ?? string.Empty,
                GoogleSecretKey = e.GoogleSecretKey ?? string.Empty,
                KaskoSpecialDiscount = e.KaskoSpecialDiscount,
                IpAddresses = e.IpAddresses ?? string.Empty,
                TrafikTeklifiKaydet = e.TrafikTeklifiKaydet,
                KaskoTeklifiKaydet = e.KaskoTeklifiKaydet,
                OtoSessionKaydet = e.OtoSessionKaydet,
                IkameAracHizmeti = e.IkameAracHizmeti,
                KesilenPaketGetir = e.KesilenPaketGetir,
                CompanyBans = DeserializeBans(e.CompanyBans)
            };
        }
    }
}
