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
    public class AnnouncementService : IAnnouncementService
    {
        private readonly VegaAsisDbContext _db;

        public AnnouncementService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IReadOnlyList<AnnouncementDto>> GetAllAsync(Guid? userId = null)
        {
            var query = _db.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .AsQueryable();
            var list = await query.ToListAsync().ConfigureAwait(false);
            return list.Select(ToDto).ToList();
        }

        public async Task<AnnouncementDto> CreateAsync(AnnouncementDto dto, Guid? createdBy = null)
        {
            var entity = new Announcement
            {
                Id = Guid.NewGuid(),
                Title = dto.Title ?? string.Empty,
                Content = dto.Content ?? string.Empty,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                TargetRole = dto.TargetRole
            };
            _db.Announcements.Add(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        private static AnnouncementDto ToDto(Announcement a)
        {
            return new AnnouncementDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                CreatedBy = a.CreatedBy,
                CreatedAt = a.CreatedAt,
                TargetRole = a.TargetRole
            };
        }
    }
}
