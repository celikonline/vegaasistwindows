using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.Forms
{
    public class RaporGrafikForm : Form
    {
        private Chart _chart;
        private readonly string _raporAdi;
        private Button _btnExport;
        private Button _btnKapat;
        private DateTimePicker _dtpBaslangic;
        private DateTimePicker _dtpBitis;
        private ComboBox _cmbGrafikTipi;
        private Panel _pnlFiltre;
        private readonly IOfferService _offerService;
        private readonly IPolicyService _policyService;

        public RaporGrafikForm(string raporAdi, IOfferService offerService = null, IPolicyService policyService = null)
        {
            _raporAdi = raporAdi ?? "Grafik";
            _offerService = offerService;
            _policyService = policyService;
            InitializeComponent();
            VarsayilanGrafikTipiniSec();
            _dtpBaslangic.ValueChanged += (s, e) => OlusturGrafik();
            _dtpBitis.ValueChanged += (s, e) => OlusturGrafik();
            OlusturGrafik();
        }

        private void VarsayilanGrafikTipiniSec()
        {
            if (_cmbGrafikTipi == null) return;
            if (_raporAdi.IndexOf("Meslek", StringComparison.OrdinalIgnoreCase) >= 0 ||
                _raporAdi.IndexOf("Kullanım Tarzı", StringComparison.OrdinalIgnoreCase) >= 0 ||
                _raporAdi.IndexOf("Ürün Grafiği", StringComparison.OrdinalIgnoreCase) >= 0 ||
                (_raporAdi.IndexOf("Ürün", StringComparison.OrdinalIgnoreCase) >= 0 && _raporAdi.IndexOf("Grafik", StringComparison.OrdinalIgnoreCase) >= 0))
                _cmbGrafikTipi.SelectedIndex = 1;
            else
                _cmbGrafikTipi.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            Text = "Grafik: " + _raporAdi;
            Size = new Size(800, 620);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(500, 450);

            _pnlFiltre = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(8, 6, 8, 6),
                BackColor = Color.FromArgb(248, 248, 248),
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblBaslangic = new Label { Text = "Başlangıç:", AutoSize = true, Location = new Point(8, 10) };
            _dtpBaslangic = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-6),
                Location = new Point(75, 8),
                Width = 100
            };
            var lblBitis = new Label { Text = "Bitiş:", AutoSize = true, Location = new Point(185, 10) };
            _dtpBitis = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Location = new Point(225, 8),
                Width = 100
            };
            var lblTip = new Label { Text = "Grafik tipi:", AutoSize = true, Location = new Point(335, 10) };
            _cmbGrafikTipi = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(405, 8),
                Width = 100
            };
            _cmbGrafikTipi.Items.AddRange(new object[] { "Sütun", "Pasta", "Çubuk" });
            _cmbGrafikTipi.SelectedIndex = 0;
            _cmbGrafikTipi.SelectedIndexChanged += (s, e) => OlusturGrafik();
            var btnYenile = new Button { Text = "Yenile", Size = new Size(70, 26), Location = new Point(515, 6) };
            btnYenile.Click += (s, e) => OlusturGrafik();
            _pnlFiltre.Controls.AddRange(new Control[] { lblBaslangic, _dtpBaslangic, lblBitis, _dtpBitis, lblTip, _cmbGrafikTipi, btnYenile });
            Controls.Add(_pnlFiltre);

            _chart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            var area = new ChartArea("Main");
            area.AxisX.Title = "Kategori";
            area.AxisY.Title = "Değer";
            _chart.ChartAreas.Add(area);
            _chart.Legends.Add(new Legend("Default"));
            Controls.Add(_chart);

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _btnExport = new Button { Text = "Grafiği Kaydet", Size = new Size(110, 28), Location = new Point(10, 10) };
            _btnExport.Click += BtnExport_Click;
            var btnYazdir = new Button { Text = "Yazdır", Size = new Size(80, 28), Location = new Point(130, 10) };
            btnYazdir.Click += BtnYazdir_Click;
            _btnKapat = new Button { Text = "Kapat", Size = new Size(80, 28), Location = new Point(220, 10) };
            _btnKapat.Click += (s, e) => Close();
            pnlBottom.Controls.Add(_btnExport);
            pnlBottom.Controls.Add(btnYazdir);
            pnlBottom.Controls.Add(_btnKapat);
            Controls.Add(pnlBottom);
        }

        private SeriesChartType SecilenGrafikTipi()
        {
            var secim = _cmbGrafikTipi?.SelectedItem?.ToString() ?? "Sütun";
            if (secim.IndexOf("Pasta", StringComparison.OrdinalIgnoreCase) >= 0) return SeriesChartType.Pie;
            if (secim.IndexOf("Çubuk", StringComparison.OrdinalIgnoreCase) >= 0) return SeriesChartType.Bar;
            return SeriesChartType.Column;
        }

        private void EksenBasliklariniAyarla(SeriesChartType chartTipi)
        {
            var area = _chart.ChartAreas["Main"];
            if (area == null) return;
            string axisX = "Kategori";
            string axisY = "Değer";
            if (_raporAdi.IndexOf("İl", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = "İl"; axisY = "Teklif Sayısı"; }
            else if (_raporAdi.IndexOf("Şirket", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = "Şirket"; axisY = "Adet"; }
            else if (_raporAdi.IndexOf("Meslek", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = ""; axisY = "%"; }
            else if (_raporAdi.IndexOf("Kullanım Tarzı", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = ""; axisY = "%"; }
            else if (_raporAdi.IndexOf("Marka", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = "Marka"; axisY = "Adet"; }
            else if (_raporAdi.IndexOf("Otorizasyon", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = "Ay"; axisY = "%"; }
            else if (_raporAdi.IndexOf("Komisyon", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = "Branş"; axisY = "TL"; }
            else if (_raporAdi.IndexOf("Ürün", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = ""; axisY = "%"; }
            else if (_raporAdi.IndexOf("Doğum Tarihi", StringComparison.OrdinalIgnoreCase) >= 0 || _raporAdi.IndexOf("Portföy", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = "Yaş Grubu"; axisY = "Adet"; }
            else if (_raporAdi.IndexOf("Kesilen Poliçe", StringComparison.OrdinalIgnoreCase) >= 0) { axisX = "Ay"; axisY = "Adet"; }
            area.AxisX.Title = axisX;
            area.AxisY.Title = axisY;
        }

        private List<OfferDto> GetFiltreliTeklifler()
        {
            if (_offerService == null || _dtpBaslangic == null || _dtpBitis == null) return new List<OfferDto>();
            var list = _offerService.GetAllAsync(null).ConfigureAwait(true).GetAwaiter().GetResult();
            if (list == null) return new List<OfferDto>();
            var bas = _dtpBaslangic.Value.Date;
            var bit = _dtpBitis.Value.Date.AddDays(1);
            return list.Where(o => o.CreatedAt >= bas && o.CreatedAt < bit).ToList();
        }

        private List<PolicyDto> GetFiltreliPoliceler()
        {
            if (_policyService == null || _dtpBaslangic == null || _dtpBitis == null) return new List<PolicyDto>();
            var list = _policyService.GetAllAsync(null).ConfigureAwait(true).GetAwaiter().GetResult();
            if (list == null) return new List<PolicyDto>();
            var bas = _dtpBaslangic.Value.Date;
            var bit = _dtpBitis.Value.Date.AddDays(1);
            return list.Where(p => (p.BitisTarihi ?? p.CreatedAt) >= bas && (p.BitisTarihi ?? p.CreatedAt) < bit).ToList();
        }

        private void OlusturGrafik()
        {
            _chart.Series.Clear();
            _chart.Titles.Clear();
            var basStr = _dtpBaslangic != null ? _dtpBaslangic.Value.ToString("dd.MM.yyyy") : "";
            var bitStr = _dtpBitis != null ? _dtpBitis.Value.ToString("dd.MM.yyyy") : "";
            var titleText = string.IsNullOrEmpty(basStr) ? _raporAdi : _raporAdi + " (" + basStr + " - " + bitStr + ")";
            _chart.Titles.Add(new Title(titleText) { Font = new Font("Segoe UI", 12F, FontStyle.Bold) });

            var chartTipi = SecilenGrafikTipi();
            EksenBasliklariniAyarla(chartTipi);

            var series = _chart.Series.Add("Veri");
            series.ChartType = chartTipi;

            if (_raporAdi.IndexOf("İl Bazlı", StringComparison.OrdinalIgnoreCase) >= 0 || (_raporAdi.IndexOf("İl", StringComparison.OrdinalIgnoreCase) >= 0 && _raporAdi.IndexOf("Grafik", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                series.Points.AddXY("İstanbul", 420);
                series.Points.AddXY("Ankara", 310);
                series.Points.AddXY("İzmir", 280);
                series.Points.AddXY("Bursa", 190);
                series.Points.AddXY("Antalya", 165);
                series.Points.AddXY("Adana", 98);
                series.Points.AddXY("Kocaeli", 87);
            }
            else if (_raporAdi.IndexOf("Şirket Bazlı", StringComparison.OrdinalIgnoreCase) >= 0 || _raporAdi.IndexOf("Şirket", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var teklifler = GetFiltreliTeklifler();
                var gruplar = teklifler.Where(o => !string.IsNullOrWhiteSpace(o.Sirket)).GroupBy(o => (o.Sirket ?? "").Trim().ToUpperInvariant()).OrderByDescending(g => g.Count()).Take(12).ToList();
                if (gruplar.Count > 0)
                {
                    foreach (var g in gruplar)
                        series.Points.AddXY(g.Key, g.Count());
                }
                else
                {
                    series.Points.AddXY("ANADOLU", 180);
                    series.Points.AddXY("AK SİGORTA", 165);
                    series.Points.AddXY("AXA", 142);
                    series.Points.AddXY("HDI", 128);
                    series.Points.AddXY("ATLAS", 95);
                }
            }
            else if (_raporAdi.IndexOf("Meslek", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var teklifler = GetFiltreliTeklifler();
                var gruplar = teklifler.Where(o => !string.IsNullOrWhiteSpace(o.Meslek)).GroupBy(o => (o.Meslek ?? "").Trim()).OrderByDescending(g => g.Count()).Take(10).ToList();
                if (gruplar.Count > 0)
                {
                    foreach (var g in gruplar)
                        series.Points.AddXY(g.Key, g.Count());
                }
                else
                {
                    series.Points.AddXY("Serbest Meslek", 35);
                    series.Points.AddXY("Memur", 28);
                    series.Points.AddXY("Emekli", 22);
                    series.Points.AddXY("Öğrenci", 10);
                    series.Points.AddXY("Diğer", 5);
                }
            }
            else if (_raporAdi.IndexOf("Kullanım Tarzı", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var teklifler = GetFiltreliTeklifler();
                var gruplar = teklifler.Where(o => !string.IsNullOrWhiteSpace(o.KullanimTarzi)).GroupBy(o => (o.KullanimTarzi ?? "").Trim()).OrderByDescending(g => g.Count()).Take(10).ToList();
                if (gruplar.Count > 0)
                {
                    foreach (var g in gruplar)
                        series.Points.AddXY(g.Key, g.Count());
                }
                else
                {
                    series.Points.AddXY("Özel Kullanım", 52);
                    series.Points.AddXY("Ticari Kullanım", 28);
                    series.Points.AddXY("Resmi Kullanım", 12);
                    series.Points.AddXY("Diğer", 8);
                }
            }
            else if (_raporAdi.IndexOf("Marka", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var teklifler = GetFiltreliTeklifler();
                var gruplar = teklifler.Where(o => !string.IsNullOrWhiteSpace(o.AracMarkasi)).GroupBy(o => (o.AracMarkasi ?? "").Trim()).OrderByDescending(g => g.Count()).Take(12).ToList();
                if (gruplar.Count > 0)
                {
                    foreach (var g in gruplar)
                        series.Points.AddXY(g.Key, g.Count());
                }
                else
                {
                    series.Points.AddXY("Fiat", 145);
                    series.Points.AddXY("Renault", 132);
                    series.Points.AddXY("Volkswagen", 118);
                    series.Points.AddXY("Ford", 95);
                    series.Points.AddXY("Toyota", 88);
                    series.Points.AddXY("Hyundai", 72);
                }
            }
            else if (_raporAdi.IndexOf("Otorizasyon", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.Points.AddXY("Ocak", 72);
                series.Points.AddXY("Şubat", 78);
                series.Points.AddXY("Mart", 75);
                series.Points.AddXY("Nisan", 82);
                series.Points.AddXY("Mayıs", 79);
                series.Points.AddXY("Haziran", 85);
            }
            else if (_raporAdi.IndexOf("Komisyon", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.Points.AddXY("Trafik", 12500);
                series.Points.AddXY("Kasko", 8200);
                series.Points.AddXY("DASK", 4100);
                series.Points.AddXY("TSS", 2800);
                series.Points.AddXY("İMM", 1900);
            }
            else if (_raporAdi.IndexOf("Ürün Grafiği", StringComparison.OrdinalIgnoreCase) >= 0 || _raporAdi.IndexOf("Ürün", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var teklifler = GetFiltreliTeklifler();
                var gruplar = teklifler.Where(o => !string.IsNullOrWhiteSpace(o.PoliceTipi)).GroupBy(o => (o.PoliceTipi ?? "").Trim()).OrderByDescending(g => g.Count()).ToList();
                if (gruplar.Count > 0)
                {
                    foreach (var g in gruplar)
                        series.Points.AddXY(g.Key, g.Count());
                }
                else
                {
                    series.Points.AddXY("Trafik", 45);
                    series.Points.AddXY("Kasko", 25);
                    series.Points.AddXY("DASK", 15);
                    series.Points.AddXY("TSS", 8);
                    series.Points.AddXY("İMM", 7);
                }
            }
            else if (_raporAdi.IndexOf("Doğum Tarihi", StringComparison.OrdinalIgnoreCase) >= 0 || _raporAdi.IndexOf("Portföy", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var teklifler = GetFiltreliTeklifler();
                var yasGruplari = new Dictionary<string, int> { { "18-25", 0 }, { "26-35", 0 }, { "36-45", 0 }, { "46-55", 0 }, { "56-65", 0 }, { "65+", 0 } };
                var now = DateTime.Today;
                foreach (var o in teklifler.Where(x => x.DogumTarihi.HasValue))
                {
                    var yas = now.Year - o.DogumTarihi.Value.Year;
                    if (yas < 26) yasGruplari["18-25"]++;
                    else if (yas < 36) yasGruplari["26-35"]++;
                    else if (yas < 46) yasGruplari["36-45"]++;
                    else if (yas < 56) yasGruplari["46-55"]++;
                    else if (yas < 66) yasGruplari["56-65"]++;
                    else yasGruplari["65+"]++;
                }
                if (yasGruplari.Values.Sum() > 0)
                {
                    foreach (var kv in yasGruplari.OrderBy(kv => kv.Key))
                        series.Points.AddXY(kv.Key, kv.Value);
                }
                else
                {
                    series.Points.AddXY("18-25", 42);
                    series.Points.AddXY("26-35", 128);
                    series.Points.AddXY("36-45", 185);
                    series.Points.AddXY("46-55", 142);
                    series.Points.AddXY("56-65", 78);
                    series.Points.AddXY("65+", 35);
                }
            }
            else if (_raporAdi.IndexOf("Kesilen Poliçe", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var policeler = GetFiltreliPoliceler();
                var aylik = policeler.Where(p => p.BitisTarihi.HasValue).GroupBy(p => new DateTime(p.BitisTarihi.Value.Year, p.BitisTarihi.Value.Month, 1)).OrderBy(g => g.Key).Take(12).ToList();
                if (aylik.Count > 0)
                {
                    var ayAdlari = new[] { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };
                    foreach (var g in aylik)
                        series.Points.AddXY(ayAdlari[g.Key.Month - 1] + " " + g.Key.Year, g.Count());
                }
                else
                {
                    series.Points.AddXY("Ocak", 28);
                    series.Points.AddXY("Şubat", 32);
                    series.Points.AddXY("Mart", 25);
                    series.Points.AddXY("Nisan", 38);
                    series.Points.AddXY("Mayıs", 41);
                    series.Points.AddXY("Haziran", 35);
                }
            }
            else
            {
                series.Points.AddXY("Ocak", 45);
                series.Points.AddXY("Şubat", 52);
                series.Points.AddXY("Mart", 48);
                series.Points.AddXY("Nisan", 61);
                series.Points.AddXY("Mayıs", 55);
                series.Points.AddXY("Haziran", 70);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new SaveFileDialog())
                {
                    dlg.Filter = "PNG görüntü (*.png)|*.png|JPEG görüntü (*.jpg)|*.jpg|Tüm dosyalar (*.*)|*.*";
                    dlg.FileName = _raporAdi.Replace(" ", "_") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm");
                    dlg.DefaultExt = "png";
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        var path = dlg.FileName;
                        var ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
                        ChartImageFormat format = ChartImageFormat.Png;
                        if (ext == ".jpg" || ext == ".jpeg")
                        {
                            format = ChartImageFormat.Jpeg;
                            if (string.IsNullOrEmpty(ext)) path += ".jpg";
                        }
                        else if (string.IsNullOrEmpty(ext) || (ext != ".png" && ext != ".jpg" && ext != ".jpeg"))
                            path += ".png";
                        _chart.SaveImage(path, format);
                        MessageBox.Show("Grafik kaydedildi: " + path, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnYazdir_Click(object sender, EventArgs e)
        {
            try
            {
                using (var pd = new PrintDocument())
                {
                    pd.PrintPage += (s, ev) =>
                    {
                        using (var bmp = new Bitmap(_chart.Width, _chart.Height))
                        {
                            _chart.DrawToBitmap(bmp, new Rectangle(0, 0, _chart.Width, _chart.Height));
                            var rect = ev.MarginBounds;
                            ev.Graphics.DrawImage(bmp, rect);
                        }
                    };
                    using (var dlg = new PrintPreviewDialog())
                    {
                        dlg.Document = pd;
                        dlg.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yazdırma hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
