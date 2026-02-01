using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IPolicyService
    {
        Task<IReadOnlyList<PolicyDto>> GetAllAsync(Guid? userId = null);
        Task<PolicyDto> GetByIdAsync(Guid id);
        Task<PolicyDto> CreateAsync(PolicyDto policy);
        Task<PolicyDto> UpdateAsync(PolicyDto policy);
        Task<bool> DeleteAsync(Guid id);
        Task<int> DeleteMultipleAsync(IEnumerable<Guid> ids);
    }
}
