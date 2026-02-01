using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IWebUserService
    {
        Task<IReadOnlyList<WebUserDto>> GetAllAsync(Guid? userId = null);

        Task<WebUserDto> CreateAsync(WebUserDto dto);

        Task<WebUserDto> UpdateAsync(Guid id, WebUserDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
