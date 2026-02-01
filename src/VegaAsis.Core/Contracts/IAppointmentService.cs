using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    public interface IAppointmentService
    {
        Task<IReadOnlyList<AppointmentDto>> GetAllAsync(Guid? userId = null);
        Task<IReadOnlyList<AppointmentDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? userId = null);
        Task<AppointmentDto> GetByIdAsync(Guid id);
        Task<AppointmentDto> CreateAsync(AppointmentDto appointment);
        Task<AppointmentDto> UpdateAsync(AppointmentDto appointment);
        Task<bool> DeleteAsync(Guid id);
    }
}
