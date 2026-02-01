using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    /// <summary>
    /// Otomatik sorgu şablonları – listele, kaydet, sil.
    /// </summary>
    public interface ISablonService
    {
        Task<IReadOnlyList<SablonDto>> GetAllAsync();
        Task<SablonDto> GetByIdAsync(int id);
        Task<int> SaveAsync(SablonDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
