using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IQuotaSettingsService
    {
        Task<QuotaSettingsDto> GetByUserAsync(Guid userId);
        Task<IReadOnlyList<QuotaSettingsDto>> GetAllAsync();
        Task<QuotaSettingsDto> UpdateAsync(QuotaSettingsDto dto);
        Task<bool> DeleteAsync(Guid userId);
        Task<bool> IncrementUsageAsync(Guid userId, string quotaType);
    }
}
