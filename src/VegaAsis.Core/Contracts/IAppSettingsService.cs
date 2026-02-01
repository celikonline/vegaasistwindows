using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IAppSettingsService
    {
        Task<AppSettingsDto> GetSettingsAsync();

        Task<AppSettingsDto> UpdateSettingsAsync(AppSettingsDto dto);
    }
}
