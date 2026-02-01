using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace VegaAsis.Windows.Forms
{
    public class RaporGrafikForm : Form
    {
        private Chart _chart;
        private readonly string _raporAdi;
        private Button _btnExport;
        private Button _btnKapat;

        public RaporGrafikForm(string raporAdi)
        {
            _raporAdi = raporAdi ?? "Grafik";
            InitializeComponent();
            OlusturGrafik();
        }

        private void InitializeComponent()
        {
            Text = "Grafik: " + _raporAdi;
            Size = new Size(800, 550);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(500, 400);

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

        private void OlusturGrafik()
        {
            _chart.Series.Clear();
            _chart.Titles.Clear();
            _chart.Titles.Add(new Title(_raporAdi) { Font = new Font("Segoe UI", 12F, FontStyle.Bold) });

            var series = _chart.Series.Add("Veri");
            series.ChartType = SeriesChartType.Column;

            if (_raporAdi.IndexOf("İl Bazlı", StringComparison.OrdinalIgnoreCase) >= 0 || (_raporAdi.IndexOf("İl", StringComparison.OrdinalIgnoreCase) >= 0 && _raporAdi.IndexOf("Grafik", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                series.ChartType = SeriesChartType.Column;
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
                series.ChartType = SeriesChartType.Column;
                series.Points.AddXY("ANADOLU", 180);
                series.Points.AddXY("AK SİGORTA", 165);
                series.Points.AddXY("AXA", 142);
                series.Points.AddXY("HDI", 128);
                series.Points.AddXY("ATLAS", 95);
                series.Points.AddXY("SOMPO", 78);
                series.Points.AddXY("CORPUS", 65);
            }
            else if (_raporAdi.IndexOf("Meslek", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.ChartType = SeriesChartType.Pie;
                series.Points.AddXY("Serbest Meslek", 35);
                series.Points.AddXY("Memur", 28);
                series.Points.AddXY("Emekli", 22);
                series.Points.AddXY("Öğrenci", 10);
                series.Points.AddXY("Diğer", 5);
            }
            else if (_raporAdi.IndexOf("Kullanım Tarzı", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.ChartType = SeriesChartType.Pie;
                series.Points.AddXY("Özel Kullanım", 52);
                series.Points.AddXY("Ticari Kullanım", 28);
                series.Points.AddXY("Resmi Kullanım", 12);
                series.Points.AddXY("Diğer", 8);
            }
            else if (_raporAdi.IndexOf("Marka", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.ChartType = SeriesChartType.Column;
                series.Points.AddXY("Fiat", 145);
                series.Points.AddXY("Renault", 132);
                series.Points.AddXY("Volkswagen", 118);
                series.Points.AddXY("Ford", 95);
                series.Points.AddXY("Toyota", 88);
                series.Points.AddXY("Hyundai", 72);
            }
            else if (_raporAdi.IndexOf("Otorizasyon", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.ChartType = SeriesChartType.Column;
                series.Points.AddXY("Ocak", 72);
                series.Points.AddXY("Şubat", 78);
                series.Points.AddXY("Mart", 75);
                series.Points.AddXY("Nisan", 82);
                series.Points.AddXY("Mayıs", 79);
                series.Points.AddXY("Haziran", 85);
            }
            else if (_raporAdi.IndexOf("Komisyon", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.ChartType = SeriesChartType.Column;
                series.Points.AddXY("Trafik", 12500);
                series.Points.AddXY("Kasko", 8200);
                series.Points.AddXY("DASK", 4100);
                series.Points.AddXY("TSS", 2800);
                series.Points.AddXY("İMM", 1900);
            }
            else if (_raporAdi.IndexOf("Ürün Grafiği", StringComparison.OrdinalIgnoreCase) >= 0 || _raporAdi.IndexOf("Ürün", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.ChartType = SeriesChartType.Pie;
                series.Points.AddXY("Trafik", 45);
                series.Points.AddXY("Kasko", 25);
                series.Points.AddXY("DASK", 15);
                series.Points.AddXY("TSS", 8);
                series.Points.AddXY("İMM", 7);
            }
            else if (_raporAdi.IndexOf("Doğum Tarihi", StringComparison.OrdinalIgnoreCase) >= 0 || _raporAdi.IndexOf("Portföy", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.ChartType = SeriesChartType.Column;
                series.Points.AddXY("18-25", 42);
                series.Points.AddXY("26-35", 128);
                series.Points.AddXY("36-45", 185);
                series.Points.AddXY("46-55", 142);
                series.Points.AddXY("56-65", 78);
                series.Points.AddXY("65+", 35);
            }
            else if (_raporAdi.IndexOf("Kesilen Poliçe", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.ChartType = SeriesChartType.Column;
                series.Points.AddXY("Ocak", 28);
                series.Points.AddXY("Şubat", 32);
                series.Points.AddXY("Mart", 25);
                series.Points.AddXY("Nisan", 38);
                series.Points.AddXY("Mayıs", 41);
                series.Points.AddXY("Haziran", 35);
            }
            else
            {
                series.ChartType = SeriesChartType.Column;
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
                    dlg.Filter = "PNG görüntü (*.png)|*.png|Tüm dosyalar (*.*)|*.*";
                    dlg.FileName = _raporAdi + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm");
                    dlg.DefaultExt = "png";
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        var path = dlg.FileName;
                        if (!path.ToLowerInvariant().EndsWith(".png"))
                            path += ".png";
                        _chart.SaveImage(path, ChartImageFormat.Png);
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
