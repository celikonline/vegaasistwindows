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
    public class AppointmentService : IAppointmentService
    {
        private readonly VegaAsisDbContext _db;

        public AppointmentService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IReadOnlyList<AppointmentDto>> GetAllAsync(Guid? userId = null)
        {
            var query = _db.Appointments
                .OrderBy(a => a.StartDate)
                .ThenBy(a => a.StartTime)
                .AsQueryable();
            if (userId.HasValue)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            var list = await query.ToListAsync().ConfigureAwait(false);
            return list.Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<AppointmentDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? userId = null)
        {
            var query = _db.Appointments
                .Where(a => a.StartDate >= startDate && a.StartDate <= endDate)
                .OrderBy(a => a.StartDate)
                .ThenBy(a => a.StartTime)
                .AsQueryable();
            
            if (userId.HasValue)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            var list = await query.ToListAsync().ConfigureAwait(false);
            return list.Select(ToDto).ToList();
        }

        public async Task<AppointmentDto> GetByIdAsync(Guid id)
        {
            var entity = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(false);
            return entity == null ? null : ToDto(entity);
        }

        public async Task<AppointmentDto> CreateAsync(AppointmentDto appointment)
        {
            var entity = new Appointment
            {
                Id = Guid.NewGuid(),
                UserId = appointment.UserId,
                Title = appointment.Title,
                Description = appointment.Description,
                Location = appointment.Location,
                Label = appointment.Label,
                TimeDisplay = appointment.TimeDisplay,
                Source = appointment.Source ?? "manual",
                StartDate = appointment.StartDate,
                StartTime = appointment.StartTime,
                EndDate = appointment.EndDate,
                EndTime = appointment.EndTime,
                AllDay = appointment.AllDay,
                ReminderMinutes = appointment.ReminderMinutes,
                ReminderSent = false,
                CustomerEmail = appointment.CustomerEmail,
                CustomerPhone = appointment.CustomerPhone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Appointments.Add(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<AppointmentDto> UpdateAsync(AppointmentDto appointment)
        {
            var entity = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == appointment.Id).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            entity.Title = appointment.Title;
            entity.Description = appointment.Description;
            entity.Location = appointment.Location;
            entity.Label = appointment.Label;
            entity.TimeDisplay = appointment.TimeDisplay;
            entity.StartDate = appointment.StartDate;
            entity.StartTime = appointment.StartTime;
            entity.EndDate = appointment.EndDate;
            entity.EndTime = appointment.EndTime;
            entity.AllDay = appointment.AllDay;
            entity.ReminderMinutes = appointment.ReminderMinutes;
            entity.CustomerEmail = appointment.CustomerEmail;
            entity.CustomerPhone = appointment.CustomerPhone;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(false);
            if (entity == null)
            {
                return false;
            }

            _db.Appointments.Remove(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        private static AppointmentDto ToDto(Appointment a)
        {
            return new AppointmentDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Title = a.Title,
                Description = a.Description,
                Location = a.Location,
                Label = a.Label,
                TimeDisplay = a.TimeDisplay,
                Source = a.Source,
                StartDate = a.StartDate,
                StartTime = a.StartTime,
                EndDate = a.EndDate,
                EndTime = a.EndTime,
                AllDay = a.AllDay,
                ReminderMinutes = a.ReminderMinutes,
                ReminderSent = a.ReminderSent,
                CustomerEmail = a.CustomerEmail,
                CustomerPhone = a.CustomerPhone,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            };
        }
    }
}
