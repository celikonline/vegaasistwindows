using System;

namespace VegaAsis.Core.DTOs
{
    /// <summary>
    /// Bildirim listesi öğesi – tarih, tip, başlık, içerik.
    /// </summary>
    public class BildirimDto
    {
        public Guid? Id { get; set; }
        public DateTime? Tarih { get; set; }
        public string Tip { get; set; }
        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public bool IsRead { get; set; }
    }
}
