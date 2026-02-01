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
    public class OfferService : IOfferService
    {
        private readonly VegaAsisDbContext _db;

        public OfferService(VegaAsisDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IReadOnlyList<OfferDto>> GetAllAsync(Guid? userId = null)
        {
            var query = _db.Offers.OrderByDescending(o => o.CreatedAt).AsQueryable();
            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            var list = await query.ToListAsync().ConfigureAwait(false);
            return list.Select(ToDto).ToList();
        }

        public async Task<OfferDto> CreateAsync(OfferDto dto, Guid userId)
        {
            var entity = new Offer
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Sirket = dto.Sirket,
                Personel = dto.Personel,
                PoliceTipi = dto.PoliceTipi ?? "TRAFİK",
                Trf = dto.Trf,
                Ksk = dto.Ksk,
                Tss = dto.Tss,
                Dsk = dto.Dsk,
                Knt = dto.Knt,
                Imm = dto.Imm,
                Musteri = dto.Musteri ?? string.Empty,
                TcVergi = dto.TcVergi,
                DogumTarihi = dto.DogumTarihi,
                Plaka = dto.Plaka,
                BelgeSeri = dto.BelgeSeri,
                KayitSekli = dto.KayitSekli ?? "Manuel",
                TaliDisAcente = dto.TaliDisAcente,
                Telefon = dto.Telefon,
                Calisildi = dto.Calisildi,
                Aciklama = dto.Aciklama,
                Policelestirme = dto.Policelestirme ?? "Bilinmiyor",
                AcentemGonderildi = dto.AcentemGonderildi,
                AcentemWebeGonderildi = dto.AcentemWebeGonderildi,
                Meslek = dto.Meslek,
                ArsivDonemi = dto.ArsivDonemi ?? "Aktif Dönem (2022 ve Sonrası)",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Offers.Add(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<OfferDto> UpdateAsync(Guid id, OfferDto dto)
        {
            var entity = await _db.Offers.FindAsync(id).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            entity.Sirket = dto.Sirket;
            entity.Personel = dto.Personel;
            entity.PoliceTipi = dto.PoliceTipi;
            entity.Trf = dto.Trf;
            entity.Ksk = dto.Ksk;
            entity.Tss = dto.Tss;
            entity.Dsk = dto.Dsk;
            entity.Knt = dto.Knt;
            entity.Imm = dto.Imm;
            entity.Musteri = dto.Musteri;
            entity.TcVergi = dto.TcVergi;
            entity.DogumTarihi = dto.DogumTarihi;
            entity.Plaka = dto.Plaka;
            entity.BelgeSeri = dto.BelgeSeri;
            entity.KayitSekli = dto.KayitSekli;
            entity.TaliDisAcente = dto.TaliDisAcente;
            entity.Telefon = dto.Telefon;
            entity.Calisildi = dto.Calisildi;
            entity.Aciklama = dto.Aciklama;
            entity.Policelestirme = dto.Policelestirme;
            entity.AcentemGonderildi = dto.AcentemGonderildi;
            entity.AcentemWebeGonderildi = dto.AcentemWebeGonderildi;
            entity.Meslek = dto.Meslek;
            entity.ArsivDonemi = dto.ArsivDonemi;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.Offers.FindAsync(id).ConfigureAwait(false);
            if (entity == null)
            {
                return false;
            }

            _db.Offers.Remove(entity);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            var idList = ids.ToList();
            if (idList.Count == 0)
            {
                return true;
            }

            var entities = await _db.Offers.Where(o => idList.Contains(o.Id)).ToListAsync().ConfigureAwait(false);
            foreach (var e in entities)
            {
                _db.Offers.Remove(e);
            }

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<OfferDto> ToggleCalisildiAsync(Guid id, bool calisildi)
        {
            var entity = await _db.Offers.FindAsync(id).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            entity.Calisildi = calisildi;
            entity.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return ToDto(entity);
        }

        private static OfferDto ToDto(Offer e)
        {
            return new OfferDto
            {
                Id = e.Id,
                UserId = e.UserId,
                Sirket = e.Sirket,
                Personel = e.Personel,
                PoliceTipi = e.PoliceTipi,
                Trf = e.Trf,
                Ksk = e.Ksk,
                Tss = e.Tss,
                Dsk = e.Dsk,
                Knt = e.Knt,
                Imm = e.Imm,
                Musteri = e.Musteri,
                TcVergi = e.TcVergi,
                DogumTarihi = e.DogumTarihi,
                Plaka = e.Plaka,
                BelgeSeri = e.BelgeSeri,
                SonIslem = e.SonIslem,
                KayitSekli = e.KayitSekli,
                TaliDisAcente = e.TaliDisAcente,
                Telefon = e.Telefon,
                Calisildi = e.Calisildi,
                Aciklama = e.Aciklama,
                Policelestirme = e.Policelestirme,
                AcentemGonderildi = e.AcentemGonderildi,
                AcentemWebeGonderildi = e.AcentemWebeGonderildi,
                Meslek = e.Meslek,
                ArsivDonemi = e.ArsivDonemi,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            };
        }
    }
}
