using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    /// <summary>
    /// UAVT (Ulusal Araç Veri Tabanı / benzeri) sorgulama servisi – TC/Vergi no veya plaka ile.
    /// </summary>
    public interface IUavtService
    {
        /// <summary>TC Kimlik / Vergi No ile UAVT sorgusu; sonuç listesi döner.</summary>
        Task<IReadOnlyList<UavtSonucDto>> SorgulaTcVergiAsync(string tcVergi);

        /// <summary>Plaka ile UAVT sorgusu; sonuç listesi döner.</summary>
        Task<IReadOnlyList<UavtSonucDto>> SorgulaPlakaAsync(string plaka);
    }
}
