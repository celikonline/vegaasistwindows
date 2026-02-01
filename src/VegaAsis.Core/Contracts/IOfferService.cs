using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IOfferService
    {
        Task<IReadOnlyList<OfferDto>> GetAllAsync(Guid? userId = null);

        Task<OfferDto> CreateAsync(OfferDto dto, Guid userId);

        Task<OfferDto> UpdateAsync(Guid id, OfferDto dto);

        Task<bool> DeleteAsync(Guid id);

        Task<bool> DeleteMultipleAsync(IEnumerable<Guid> ids);

        Task<OfferDto> ToggleCalisildiAsync(Guid id, bool calisildi);
    }
}
