using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface ISharedCompanyService
    {
        Task<IReadOnlyList<SharedCompanyDto>> GetAllAsync(Guid? ownerUserId = null);

        Task<SharedCompanyDto> CreateAsync(SharedCompanyDto dto);

        Task<bool> DeleteAsync(Guid id);

        Task<SharedCompanyDto> UpdateAsync(Guid id, SharedCompanyDto dto);

        Task<IReadOnlyList<ReceivedCompanyShareDto>> GetReceivedSharesAsync(Guid receiverUserId);
    }
}
