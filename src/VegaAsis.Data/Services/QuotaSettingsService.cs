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
    public class QuotaSettingsService : IQuotaSettingsService
    {
        private readonly VegaAsisDbContext _db;

        public QuotaSettingsService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<QuotaSettingsDto> GetByUserAsync(Guid userId)
        {
            var entity = await _db.QuotaSettings
                .FirstOrDefaultAsync(q => q.UserId == userId)
                .ConfigureAwait(false);

            return entity == null ? null : ToDto(entity);
        }

        public async Task<IReadOnlyList<QuotaSettingsDto>> GetAllAsync()
        {
            var entities = await _db.QuotaSettings
                .OrderBy(q => q.UserId)
                .ToListAsync()
                .ConfigureAwait(false);

            return entities.Select(ToDto).ToList();
        }

        public async Task<QuotaSettingsDto> UpdateAsync(QuotaSettingsDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var entity = await _db.QuotaSettings
                .FirstOrDefaultAsync(q => q.UserId == dto.UserId)
                .ConfigureAwait(false);

            if (entity == null)
            {
                // Yeni kayıt oluştur
                entity = new QuotaSettings
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    DailyQueryLimit = dto.DailyQueryLimit,
                    MonthlyQueryLimit = dto.MonthlyQueryLimit,
                    DailyOfferLimit = dto.DailyOfferLimit,
                    MonthlyOfferLimit = dto.MonthlyOfferLimit,
                    DailyPolicyLimit = dto.DailyPolicyLimit,
                    MonthlyPolicyLimit = dto.MonthlyPolicyLimit,
                    CurrentDailyQueries = dto.CurrentDailyQueries,
                    CurrentMonthlyQueries = dto.CurrentMonthlyQueries,
                    CurrentDailyOffers = dto.CurrentDailyOffers,
                    CurrentMonthlyOffers = dto.CurrentMonthlyOffers,
                    CurrentDailyPolicies = dto.CurrentDailyPolicies,
                    CurrentMonthlyPolicies = dto.CurrentMonthlyPolicies,
                    LastDailyReset = dto.LastDailyReset,
                    LastMonthlyReset = dto.LastMonthlyReset,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.QuotaSettings.Add(entity);
            }
            else
            {
                // Mevcut kaydı güncelle
                entity.DailyQueryLimit = dto.DailyQueryLimit;
                entity.MonthlyQueryLimit = dto.MonthlyQueryLimit;
                entity.DailyOfferLimit = dto.DailyOfferLimit;
                entity.MonthlyOfferLimit = dto.MonthlyOfferLimit;
                entity.DailyPolicyLimit = dto.DailyPolicyLimit;
                entity.MonthlyPolicyLimit = dto.MonthlyPolicyLimit;
                entity.CurrentDailyQueries = dto.CurrentDailyQueries;
                entity.CurrentMonthlyQueries = dto.CurrentMonthlyQueries;
                entity.CurrentDailyOffers = dto.CurrentDailyOffers;
                entity.CurrentMonthlyOffers = dto.CurrentMonthlyOffers;
                entity.CurrentDailyPolicies = dto.CurrentDailyPolicies;
                entity.CurrentMonthlyPolicies = dto.CurrentMonthlyPolicies;
                entity.LastDailyReset = dto.LastDailyReset;
                entity.LastMonthlyReset = dto.LastMonthlyReset;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            var entity = await _db.QuotaSettings
                .FirstOrDefaultAsync(q => q.UserId == userId)
                .ConfigureAwait(false);

            if (entity == null)
            {
                return false;
            }

            _db.QuotaSettings.Remove(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> IncrementUsageAsync(Guid userId, string quotaType)
        {
            if (string.IsNullOrWhiteSpace(quotaType))
            {
                return false;
            }

            var entity = await _db.QuotaSettings
                .FirstOrDefaultAsync(q => q.UserId == userId)
                .ConfigureAwait(false);

            if (entity == null)
            {
                return false;
            }

            var now = DateTime.UtcNow;
            var shouldResetDaily = entity.LastDailyReset == null || 
                                   (now.Date > entity.LastDailyReset.Value.Date);
            var shouldResetMonthly = entity.LastMonthlyReset == null || 
                                    (now.Year != entity.LastMonthlyReset.Value.Year || 
                                     now.Month != entity.LastMonthlyReset.Value.Month);

            // Günlük reset kontrolü
            if (shouldResetDaily)
            {
                entity.CurrentDailyQueries = 0;
                entity.CurrentDailyOffers = 0;
                entity.CurrentDailyPolicies = 0;
                entity.LastDailyReset = now;
            }

            // Aylık reset kontrolü
            if (shouldResetMonthly)
            {
                entity.CurrentMonthlyQueries = 0;
                entity.CurrentMonthlyOffers = 0;
                entity.CurrentMonthlyPolicies = 0;
                entity.LastMonthlyReset = now;
            }

            // Kullanımı artır
            switch (quotaType.ToLowerInvariant())
            {
                case "queries":
                case "query":
                    entity.CurrentDailyQueries++;
                    entity.CurrentMonthlyQueries++;
                    break;
                case "offers":
                case "offer":
                    entity.CurrentDailyOffers++;
                    entity.CurrentMonthlyOffers++;
                    break;
                case "policies":
                case "policy":
                    entity.CurrentDailyPolicies++;
                    entity.CurrentMonthlyPolicies++;
                    break;
                default:
                    return false;
            }

            entity.UpdatedAt = now;
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        private static QuotaSettingsDto ToDto(QuotaSettings entity)
        {
            return new QuotaSettingsDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                DailyQueryLimit = entity.DailyQueryLimit,
                MonthlyQueryLimit = entity.MonthlyQueryLimit,
                DailyOfferLimit = entity.DailyOfferLimit,
                MonthlyOfferLimit = entity.MonthlyOfferLimit,
                DailyPolicyLimit = entity.DailyPolicyLimit,
                MonthlyPolicyLimit = entity.MonthlyPolicyLimit,
                CurrentDailyQueries = entity.CurrentDailyQueries,
                CurrentMonthlyQueries = entity.CurrentMonthlyQueries,
                CurrentDailyOffers = entity.CurrentDailyOffers,
                CurrentMonthlyOffers = entity.CurrentMonthlyOffers,
                CurrentDailyPolicies = entity.CurrentDailyPolicies,
                CurrentMonthlyPolicies = entity.CurrentMonthlyPolicies,
                LastDailyReset = entity.LastDailyReset,
                LastMonthlyReset = entity.LastMonthlyReset,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
