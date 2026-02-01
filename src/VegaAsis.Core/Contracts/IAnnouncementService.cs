using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IAnnouncementService
    {
        Task<IReadOnlyList<AnnouncementDto>> GetAllAsync(Guid? userId = null);
        Task<AnnouncementDto> CreateAsync(AnnouncementDto dto, Guid? createdBy = null);
    }
}
