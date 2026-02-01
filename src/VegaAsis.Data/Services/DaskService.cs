using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Data.Services
{
    /// <summary>
    /// DASK teminat servisi. Şu an boş liste döner; ileride veritabanı veya DASK API entegrasyonu eklenebilir.
    /// Stub kaldırıldığında Program.cs'te IDaskService olarak bu sınıf kayıtlı.
    /// </summary>
    public class DaskService : IDaskService
    {
        public Task<IReadOnlyList<DaskTeminatDto>> GetTeminatlarAsync(string il = null, string ilce = null)
        {
            // İleride: EF ile dask_teminatlar tablosundan veya DASK API'den doldurulabilir
            var list = new List<DaskTeminatDto>();
            return Task.FromResult<IReadOnlyList<DaskTeminatDto>>(list);
        }
    }
}
