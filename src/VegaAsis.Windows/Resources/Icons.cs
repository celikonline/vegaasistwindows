using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace VegaAsis.Windows.Resources
{
    public static class Icons
    {
        private static readonly string ResourcePath;

        static Icons()
        {
            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ResourcePath = Path.Combine(exeDir, "Resources");
            if (!Directory.Exists(ResourcePath))
            {
                ResourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            }
        }

        public static Image LoadIcon(string name, int width = 20, int height = 20)
        {
            try
            {
                var path = Path.Combine(ResourcePath, name + ".png");
                if (!File.Exists(path)) return null;
                var img = Image.FromFile(path);
                if (img.Width == width && img.Height == height) return img;
                var resized = new Bitmap(width, height);
                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(img, 0, 0, width, height);
                }
                img.Dispose();
                return resized;
            }
            catch
            {
                return null;
            }
        }

        public static Image Ajanda { get { return LoadIcon("AJANDA"); } }
        public static Image Asistans { get { return LoadIcon("ASISTANS"); } }
        public static Image Ayarlar { get { return LoadIcon("AYARLAR"); } }
        public static Image Bilgiler { get { return LoadIcon("BILGILER"); } }
        public static Image Destek { get { return LoadIcon("DESTEK"); } }
        public static Image Devam { get { return LoadIcon("DEVAM"); } }
        public static Image DisaAl { get { return LoadIcon("DISA-AL"); } }
        public static Image Egitim { get { return LoadIcon("EGITIM"); } }
        public static Image Egm { get { return LoadIcon("EGM"); } }
        public static Image FormTemizleme { get { return LoadIcon("FORM-TEMIZLEME"); } }
        public static Image KaskoDeger { get { return LoadIcon("KASKO-DEGER"); } }
        public static Image Kasko { get { return LoadIcon("KASKO"); } }
        public static Image Konut { get { return LoadIcon("KONUT"); } }
        public static Image Kopyalama { get { return LoadIcon("KOPYALAMA"); } }
        public static Image KuyruklamaSorgu { get { return LoadIcon("KUYRUKLAMA-SORGU"); } }
        public static Image Manuel { get { return LoadIcon("MANUEL"); } }
        public static Image PdfYukle { get { return LoadIcon("PDF-YUKLE"); } }
        public static Image Pdf { get { return LoadIcon("PDF"); } }
        public static Image Png { get { return LoadIcon("PNG"); } }
        public static Image Policeler { get { return LoadIcon("POLICELER"); } }
        public static Image Raporlar { get { return LoadIcon("RAPORLAR"); } }
        public static Image Robot { get { return LoadIcon("ROBOT"); } }
        public static Image RuhsatQr { get { return LoadIcon("RUHSAT-QR"); } }
        public static Image Ram { get { return LoadIcon("RUST"); } }
        public static Image SanalT { get { return LoadIcon("SANALT"); } }
        public static Image Sbm { get { return LoadIcon("SBM"); } }
        public static Image SirketBilgileri { get { return LoadIcon("SIRKET-BILGILERI"); } }
        public static Image SirketEkle { get { return LoadIcon("SIRKET-EKLEM"); } }
        public static Image SorguBaslat { get { return LoadIcon("SORGU-BASLAT"); } }
        public static Image Teklif { get { return LoadIcon("TEKLIF"); } }
        public static Image Teklifler { get { return LoadIcon("TEKLIFLER"); } }
        public static Image Trafik { get { return LoadIcon("TRAFIK"); } }
        public static Image Tramer { get { return LoadIcon("TRAMER"); } }
        public static Image TssAyak { get { return LoadIcon("TSS-AYAK"); } }
        public static Image TssYat { get { return LoadIcon("TSS-YAT"); } }
        public static Image YeniSorgu { get { return LoadIcon("YENI-SORGU"); } }
        public static Image Yonetici { get { return LoadIcon("YONETICI"); } }
    }
}
