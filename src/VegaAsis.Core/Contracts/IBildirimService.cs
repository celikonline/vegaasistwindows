using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    /// <summary>
    /// Kullanıcı bildirimleri – listeleme (ileride okundu işaretleme / temizleme eklenebilir).
    /// </summary>
    public interface IBildirimService
    {
        Task<IReadOnlyList<BildirimDto>> GetAllAsync();
        Task<IReadOnlyList<BildirimDto>> GetAllByRoleAsync(string role, Guid? userId);
        Task MarkAsReadAsync(Guid announcementId, Guid userId);
    }
}
