using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Core.Contracts
{
    /// <summary>
    /// WS ile teklif sorgulama – şablon ve tarih aralığına göre sonuç listesi.
    /// </summary>
    public interface IWsSorguService
    {
        Task<IReadOnlyList<WSTeklifSonucDto>> SorgulaAsync(int? sablonId, DateTime baslangic, DateTime bitis);
    }
}
