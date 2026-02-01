using System.Collections.Generic;

namespace VegaAsis.Core.DTOs
{
    /// <summary>
    /// Otomatik sorgu şablonu – ad, branş, seçili şirket listesi.
    /// </summary>
    public class SablonDto
    {
        public int? Id { get; set; }
        public string Ad { get; set; }
        public string Brans { get; set; }
        public List<string> SirketAdlari { get; set; }
    }
}
