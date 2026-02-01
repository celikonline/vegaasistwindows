using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Data.Services
{
    /// <summary>
    /// Otomatik sorgu şablonları – şu an bellek içi liste; ileride DB tablosu eklenebilir.
    /// </summary>
    public class SablonService : ISablonService
    {
        private readonly List<SablonDto> _list = new List<SablonDto>();
        private int _nextId = 1;
        private readonly object _lock = new object();

        public SablonService()
        {
            _list.Add(new SablonDto { Id = _nextId++, Ad = "Varsayılan Trafik", Brans = "TRAFİK", SirketAdlari = new List<string> { "ANADOLU", "AK SİGORTA" } });
            _list.Add(new SablonDto { Id = _nextId++, Ad = "Kasko Hızlı", Brans = "KASKO", SirketAdlari = new List<string> { "HDI", "AXA" } });
            _list.Add(new SablonDto { Id = _nextId++, Ad = "DASK Toplu", Brans = "DASK", SirketAdlari = new List<string> { "ANADOLU", "ATLAS" } });
        }

        public Task<IReadOnlyList<SablonDto>> GetAllAsync()
        {
            lock (_lock)
            {
                var copy = _list.Select(x => new SablonDto
                {
                    Id = x.Id,
                    Ad = x.Ad,
                    Brans = x.Brans,
                    SirketAdlari = x.SirketAdlari == null ? new List<string>() : new List<string>(x.SirketAdlari)
                }).ToList();
                return Task.FromResult<IReadOnlyList<SablonDto>>(copy);
            }
        }

        public Task<SablonDto> GetByIdAsync(int id)
        {
            lock (_lock)
            {
                var item = _list.FirstOrDefault(x => x.Id == id);
                if (item == null)
                    return Task.FromResult<SablonDto>(null);
                return Task.FromResult(new SablonDto
                {
                    Id = item.Id,
                    Ad = item.Ad,
                    Brans = item.Brans,
                    SirketAdlari = item.SirketAdlari == null ? new List<string>() : new List<string>(item.SirketAdlari)
                });
            }
        }

        public Task<int> SaveAsync(SablonDto dto)
        {
            if (dto == null)
                return Task.FromResult(0);

            lock (_lock)
            {
                if (dto.Id.HasValue && dto.Id.Value > 0)
                {
                    var idx = _list.FindIndex(x => x.Id == dto.Id.Value);
                    if (idx >= 0)
                    {
                        _list[idx] = new SablonDto
                        {
                            Id = dto.Id,
                            Ad = (dto.Ad ?? string.Empty).Trim(),
                            Brans = (dto.Brans ?? string.Empty).Trim(),
                            SirketAdlari = dto.SirketAdlari ?? new List<string>()
                        };
                        return Task.FromResult(dto.Id.Value);
                    }
                }

                var id = _nextId++;
                _list.Add(new SablonDto
                {
                    Id = id,
                    Ad = (dto.Ad ?? string.Empty).Trim(),
                    Brans = (dto.Brans ?? string.Empty).Trim(),
                    SirketAdlari = dto.SirketAdlari ?? new List<string>()
                });
                return Task.FromResult(id);
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            lock (_lock)
            {
                var idx = _list.FindIndex(x => x.Id == id);
                if (idx < 0)
                    return Task.FromResult(false);
                _list.RemoveAt(idx);
                return Task.FromResult(true);
            }
        }
    }
}
