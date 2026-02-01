using System;
using System.Drawing;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;

namespace VegaAsis.Windows.Forms
{
    public class RaporlarForm : Form
    {
        private static readonly string[] UretimItems = { "Günlük Üretim Raporu", "Aylık Üretim Raporu", "Şirket Bazlı Üretim", "Personel Bazlı Üretim" };
        private static readonly string[] PoliceItems = { "Aktif Poliçeler", "Yenileme Bekleyenler", "İptal Edilen Poliçeler", "Vadesi Geçen Poliçeler" };
        private static readonly string[] FinansalItems = { "Prim Tahsilat Raporu", "Komisyon Raporu", "Alacak Raporu", "Borç Raporu" };
        private static readonly string[] AnalizItems = { "Branş Dağılımı", "Şirket Dağılımı", "Müşteri Analizi", "Trend Analizi" };
        public static readonly string[] GrafikItems = { "İl Bazlı Grafik", "Şirket Bazlı Grafik", "Mesleklere Göre Teklif Grafiği", "Teklif Kullanım Tarzı Grafiği", "Teklif Marka Grafiği", "Teklif Otorizasyon Oranları", "Teklif Komisyon Kazancı", "Ürün Grafiği", "Doğum Tarihi Portföy", "Kesilen Poliçeler" };

        private readonly IOfferService _offerService;
        private readonly IPolicyService _policyService;

        public RaporlarForm(IOfferService offerService = null, IPolicyService policyService = null)
        {
            _offerService = offerService;
            _policyService = policyService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Raporlar";
            Size = new Size(950, 620);
            StartPosition = FormStartPosition.CenterParent;
            Padding = new Padding(20);

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(8)
            };

            flow.Controls.Add(CreateReportCard("Üretim Raporları", "Üretim ve satış raporları", UretimItems));
            flow.Controls.Add(CreateReportCard("Poliçe Raporları", "Poliçe detay raporları", PoliceItems));
            flow.Controls.Add(CreateReportCard("Finansal Raporlar", "Gelir/gider raporları", FinansalItems));
            flow.Controls.Add(CreateReportCard("Analiz Raporları", "İstatistik ve analizler", AnalizItems));
            flow.Controls.Add(CreateReportCard("Grafik Raporları", "İstatistik grafikleri (Open Hızlı Teklif uyumlu)", GrafikItems, 420));

            Controls.Add(flow);
        }

        private Panel CreateReportCard(string title, string description, string[] items, int height = 220)
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(16),
                Size = new Size(420, height),
                Margin = new Padding(8),
                Cursor = Cursors.Hand,
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true,
                Location = new Point(12, 12)
            };
            panel.Controls.Add(titleLabel);
            if (!string.IsNullOrEmpty(description))
            {
                var descLabel = new Label
                {
                    Text = description,
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.FromArgb(102, 102, 102),
                    AutoSize = true,
                    Location = new Point(12, 32),
                    MaximumSize = new Size(panel.Size.Width - 24, 0)
                };
                panel.Controls.Add(descLabel);
            }

            int y = string.IsNullOrEmpty(description) ? 40 : 52;
            foreach (var item in items)
            {
                var link = new LinkLabel
                {
                    Text = item,
                    AutoSize = true,
                    Location = new Point(12, y),
                    ForeColor = Color.FromArgb(102, 102, 102),
                    Cursor = Cursors.Hand,
                    Tag = item
                };
                link.LinkBehavior = LinkBehavior.NeverUnderline;
                link.Click += (s, e) => AcRaporDetay((string)((LinkLabel)s).Tag);
                link.LinkClicked += (s, e) => e.Link.Visited = true;
                panel.Controls.Add(link);
                y += 36;
            }

            var originalColor = panel.BackColor;
            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(250, 250, 250);
            panel.MouseLeave += (s, e) => panel.BackColor = originalColor;

            return panel;
        }

        private void AcRaporDetay(string raporAdi)
        {
            var isGrafik = Array.IndexOf(GrafikItems, raporAdi) >= 0;
            if (isGrafik)
            {
                using (var f = new RaporGrafikForm(raporAdi, _offerService, _policyService))
                {
                    f.ShowDialog(this);
                }
            }
            else
            {
                using (var f = new PlaceholderForm(raporAdi))
                {
                    f.ShowDialog(this);
                }
            }
        }
    }
}
