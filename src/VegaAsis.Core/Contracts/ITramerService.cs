using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    /// <summary>
    /// Tramer (hasar) sorgulama servisi – plaka veya şasi numarası ile.
    /// </summary>
    public interface ITramerService
    {
        /// <summary>Plaka ile Tramer sorgusu; sonuç listesi döner.</summary>
        Task<IReadOnlyList<TramerSonucDto>> SorgulaPlakaAsync(string plaka);

        /// <summary>Şasi numarası ile Tramer sorgusu; sonuç listesi döner.</summary>
        Task<IReadOnlyList<TramerSonucDto>> SorgulaSasiNoAsync(string sasiNo);
    }
}
