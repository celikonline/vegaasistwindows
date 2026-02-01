using System;
using System.Collections.Generic;

namespace VegaAsis.Windows.Models
{
    public enum SorguDurum
    {
        Idle,
        Running,
        Paused
    }

    public class SorguSession
    {
        public Guid? TeklifId { get; set; }
        public SorguDurum Durum { get; set; }
        public string Plaka { get; set; }
        public string TcVergi { get; set; }
        public string BelgeSeri { get; set; }
        public string BelgeNo { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public string MusteriAdi { get; set; }
        public string Meslek { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string KullanimTarzi { get; set; }
        public string Marka { get; set; }
        public string Tip { get; set; }
        public int? ModelYili { get; set; }
        public List<string> SeciliSirketler { get; set; }
        public string AktifBrans { get; set; }
        public string TrafikSigortaSirketi { get; set; }
        public string TrafikAcenteKodu { get; set; }
        public string TrafikPoliceNo { get; set; }
        public DateTime? TrafikBaslangicTarihi { get; set; }
        public DateTime? TrafikBitisTarihi { get; set; }
        public bool KisaVadeliPolice { get; set; }

        public SorguSession()
        {
            Durum = SorguDurum.Idle;
            SeciliSirketler = new List<string>();
        }

        public void Reset()
        {
            TeklifId = null;
            Durum = SorguDurum.Idle;
            Plaka = null;
            TcVergi = null;
            BelgeSeri = null;
            BelgeNo = null;
            DogumTarihi = null;
            MusteriAdi = null;
            Meslek = null;
            Il = null;
            Ilce = null;
            KullanimTarzi = null;
            Marka = null;
            Tip = null;
            ModelYili = null;
            SeciliSirketler.Clear();
            AktifBrans = null;
            TrafikSigortaSirketi = null;
            TrafikAcenteKodu = null;
            TrafikPoliceNo = null;
            TrafikBaslangicTarihi = null;
            TrafikBitisTarihi = null;
            KisaVadeliPolice = false;
        }
    }
}
