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

            if (_raporAdi.IndexOf("İl", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.Points.AddXY("İstanbul", 420);
                series.Points.AddXY("Ankara", 310);
                series.Points.AddXY("İzmir", 280);
                series.Points.AddXY("Bursa", 190);
                series.Points.AddXY("Antalya", 165);
            }
            else if (_raporAdi.IndexOf("Şirket", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                series.Points.AddXY("ANADOLU", 180);
                series.Points.AddXY("AK SİGORTA", 165);
                series.Points.AddXY("AXA", 142);
                series.Points.AddXY("HDI", 128);
                series.Points.AddXY("ATLAS", 95);
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
