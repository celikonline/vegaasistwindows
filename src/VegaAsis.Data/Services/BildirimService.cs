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
    /// <summary>
    /// Bildirim listesi – Duyuru (Announcement) tablosundan okunur.
    /// </summary>
    public class BildirimService : IBildirimService
    {
        private readonly VegaAsisDbContext _db;

        public BildirimService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IReadOnlyList<BildirimDto>> GetAllAsync()
        {
            return await GetAllByRoleAsync(null, null).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BildirimDto>> GetAllByRoleAsync(string role, Guid? userId)
        {
            try
            {
                var normalizedRole = string.IsNullOrWhiteSpace(role) ? null : role.Trim().ToLowerInvariant();
                var query = _db.Announcements.AsQueryable();
                if (!string.IsNullOrEmpty(normalizedRole))
                {
                    query = query.Where(a =>
                        a.TargetRole == null ||
                        a.TargetRole == "" ||
                        a.TargetRole == normalizedRole ||
                        a.TargetRole == "all" ||
                        a.TargetRole == "tumu");
                }

                var list = await query
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                HashSet<Guid> readSet = null;
                if (userId.HasValue)
                {
                    var readIds = await _db.AnnouncementReads
                        .Where(r => r.UserId == userId.Value)
                        .Select(r => r.AnnouncementId)
                        .ToListAsync()
                        .ConfigureAwait(false);
                    readSet = new HashSet<Guid>(readIds);
                }

                return list.Select(a => ToDto(a, readSet)).ToList();
            }
            catch
            {
                return new List<BildirimDto>();
            }
        }

        public async Task MarkAsReadAsync(Guid announcementId, Guid userId)
        {
            if (announcementId == Guid.Empty || userId == Guid.Empty) return;
            try
            {
                var exists = await _db.AnnouncementReads
                    .AnyAsync(r => r.AnnouncementId == announcementId && r.UserId == userId)
                    .ConfigureAwait(false);
                if (exists) return;
                _db.AnnouncementReads.Add(new AnnouncementRead
                {
                    Id = Guid.NewGuid(),
                    AnnouncementId = announcementId,
                    UserId = userId,
                    ReadAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
            catch
            {
                // okundu işaretleme hatası yoksayılır
            }
        }

        private static BildirimDto ToDto(Announcement a, HashSet<Guid> readSet)
        {
            var isRead = readSet != null && readSet.Contains(a.Id);
            return new BildirimDto
            {
                Id = a.Id,
                Tarih = a.CreatedAt,
                Tip = "Duyuru",
                Baslik = a.Title,
                Icerik = a.Content,
                IsRead = isRead
            };
        }
    }
}
