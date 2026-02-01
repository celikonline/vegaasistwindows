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
    public class PolicyService : IPolicyService
    {
        private readonly VegaAsisDbContext _db;

        public PolicyService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IReadOnlyList<PolicyDto>> GetAllAsync(Guid? userId = null)
        {
            var query = _db.Policies.OrderByDescending(p => p.CreatedAt).AsQueryable();
            if (userId.HasValue)
            {
                query = query.Where(p => p.UserId == userId.Value);
            }

            var list = await query.ToListAsync().ConfigureAwait(false);
            return list.Select(ToDto).ToList();
        }

        public async Task<PolicyDto> GetByIdAsync(Guid id)
        {
            var entity = await _db.Policies.FirstOrDefaultAsync(p => p.Id == id).ConfigureAwait(false);
            return entity == null ? null : ToDto(entity);
        }

        public async Task<PolicyDto> CreateAsync(PolicyDto policy)
        {
            var entity = new Policy
            {
                Id = Guid.NewGuid(),
                UserId = policy.UserId,
                PoliceTuru = policy.PoliceTuru,
                PoliceNo = policy.PoliceNo,
                YenilemeNo = policy.YenilemeNo,
                ZeyilNo = policy.ZeyilNo,
                Sirket = policy.Sirket,
                Musteri = policy.Musteri,
                TcVergi = policy.TcVergi,
                Plaka = policy.Plaka,
                Prim = policy.Prim,
                DovizTipi = policy.DovizTipi,
                BaslangicTarihi = policy.BaslangicTarihi,
                BitisTarihi = policy.BitisTarihi,
                KayitTarihi = DateTime.UtcNow,
                KayitTipi = policy.KayitTipi,
                Personel = policy.Personel,
                KullaniciAdi = policy.KullaniciAdi,
                AnaAcente = policy.AnaAcente,
                KesenAcente = policy.KesenAcente,
                AcentemAlinmaDurumu = policy.AcentemAlinmaDurumu,
                Aciklama = policy.Aciklama,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Policies.Add(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<PolicyDto> UpdateAsync(PolicyDto policy)
        {
            var entity = await _db.Policies.FirstOrDefaultAsync(p => p.Id == policy.Id).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            entity.PoliceTuru = policy.PoliceTuru;
            entity.PoliceNo = policy.PoliceNo;
            entity.YenilemeNo = policy.YenilemeNo;
            entity.ZeyilNo = policy.ZeyilNo;
            entity.Sirket = policy.Sirket;
            entity.Musteri = policy.Musteri;
            entity.TcVergi = policy.TcVergi;
            entity.Plaka = policy.Plaka;
            entity.Prim = policy.Prim;
            entity.DovizTipi = policy.DovizTipi;
            entity.BaslangicTarihi = policy.BaslangicTarihi;
            entity.BitisTarihi = policy.BitisTarihi;
            entity.KayitTipi = policy.KayitTipi;
            entity.Personel = policy.Personel;
            entity.KullaniciAdi = policy.KullaniciAdi;
            entity.AnaAcente = policy.AnaAcente;
            entity.KesenAcente = policy.KesenAcente;
            entity.AcentemAlinmaDurumu = policy.AcentemAlinmaDurumu;
            entity.Aciklama = policy.Aciklama;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.Policies.FirstOrDefaultAsync(p => p.Id == id).ConfigureAwait(false);
            if (entity == null)
            {
                return false;
            }

            _db.Policies.Remove(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<int> DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            var idList = ids.ToList();
            var entities = await _db.Policies.Where(p => idList.Contains(p.Id)).ToListAsync().ConfigureAwait(false);
            if (entities.Count == 0)
            {
                return 0;
            }

            _db.Policies.RemoveRange(entities);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return entities.Count;
        }

        private static PolicyDto ToDto(Policy p)
        {
            return new PolicyDto
            {
                Id = p.Id,
                UserId = p.UserId,
                PoliceTuru = p.PoliceTuru,
                PoliceNo = p.PoliceNo,
                YenilemeNo = p.YenilemeNo,
                ZeyilNo = p.ZeyilNo,
                Sirket = p.Sirket,
                Musteri = p.Musteri,
                TcVergi = p.TcVergi,
                Plaka = p.Plaka,
                Prim = p.Prim,
                DovizTipi = p.DovizTipi,
                BaslangicTarihi = p.BaslangicTarihi,
                BitisTarihi = p.BitisTarihi,
                KayitTarihi = p.KayitTarihi,
                KayitTipi = p.KayitTipi,
                Personel = p.Personel,
                KullaniciAdi = p.KullaniciAdi,
                AnaAcente = p.AnaAcente,
                KesenAcente = p.KesenAcente,
                AcentemAlinmaDurumu = p.AcentemAlinmaDurumu,
                Aciklama = p.Aciklama,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}
