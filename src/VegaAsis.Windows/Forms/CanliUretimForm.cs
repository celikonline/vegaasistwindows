using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class CanliUretimForm : Form
    {
        private FlowLayoutPanel _summaryPanel;
        private SplitContainer _splitContainer;
        private DataGridView _dgvSirketUretim;
        private DataGridView _dgvSonUretimler;

        public CanliUretimForm()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void InitializeComponent()
        {
            Text = "Canlı Üretim";
            Size = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 600);

            // Üst Panel - Özet Kartları
            _summaryPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            // Özet Kartları Oluştur
            CreateSummaryCard("Günlük Üretim", "0 TL", _summaryPanel);
            CreateSummaryCard("Aylık Üretim", "0 TL", _summaryPanel);
            CreateSummaryCard("Aktif Poliçe", "0", _summaryPanel);
            CreateSummaryCard("Bekleyen Teklif", "0", _summaryPanel);

            // SplitContainer
            _splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300,
                SplitterWidth = 5
            };

            // Panel1 - Şirket Bazlı Üretim Tablosu
            _dgvSirketUretim = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };

            _dgvSirketUretim.Columns.Add("Sirket", "Şirket");
            _dgvSirketUretim.Columns.Add("Trafik", "Trafik");
            _dgvSirketUretim.Columns.Add("Kasko", "Kasko");
            _dgvSirketUretim.Columns.Add("TSS", "TSS");
            _dgvSirketUretim.Columns.Add("Toplam", "Toplam");

            _splitContainer.Panel1.Controls.Add(_dgvSirketUretim);

            // Panel2 - Son Üretimler Listesi
            _dgvSonUretimler = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };

            _dgvSonUretimler.Columns.Add("Tarih", "Tarih");
            _dgvSonUretimler.Columns.Add("Sirket", "Şirket");
            _dgvSonUretimler.Columns.Add("Musteri", "Müşteri");
            _dgvSonUretimler.Columns.Add("Brans", "Branş");
            _dgvSonUretimler.Columns.Add("Prim", "Prim");

            _splitContainer.Panel2.Controls.Add(_dgvSonUretimler);

            // Form'a kontrolleri ekle
            Controls.Add(_splitContainer);
            Controls.Add(_summaryPanel);
        }

        private void CreateSummaryCard(string title, string value, FlowLayoutPanel parent)
        {
            var card = new Panel
            {
                Width = 200,
                Height = 80,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5)
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular),
                ForeColor = Color.Gray,
                Location = new Point(10, 10),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, 35),
                AutoSize = true
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            parent.Controls.Add(card);
        }

        private void LoadSampleData()
        {
            // Şirket Bazlı Üretim - Örnek Veri
            _dgvSirketUretim.Rows.Clear();
            _dgvSirketUretim.Rows.Add("Anadolu Sigorta", "15.000 TL", "8.500 TL", "3.200 TL", "26.700 TL");
            _dgvSirketUretim.Rows.Add("Allianz Sigorta", "12.300 TL", "9.100 TL", "2.800 TL", "24.200 TL");
            _dgvSirketUretim.Rows.Add("Groupama Sigorta", "10.500 TL", "7.200 TL", "1.900 TL", "19.600 TL");
            _dgvSirketUretim.Rows.Add("HDI Sigorta", "8.700 TL", "6.500 TL", "2.100 TL", "17.300 TL");

            // Son Üretimler - Örnek Veri
            _dgvSonUretimler.Rows.Clear();
            _dgvSonUretimler.Rows.Add("01.02.2026 14:30", "Anadolu Sigorta", "Ahmet Yılmaz", "Trafik", "2.500 TL");
            _dgvSonUretimler.Rows.Add("01.02.2026 14:15", "Allianz Sigorta", "Mehmet Demir", "Kasko", "3.200 TL");
            _dgvSonUretimler.Rows.Add("01.02.2026 13:45", "Groupama Sigorta", "Ayşe Kaya", "TSS", "1.800 TL");
            _dgvSonUretimler.Rows.Add("01.02.2026 13:20", "HDI Sigorta", "Fatma Şahin", "Trafik", "1.900 TL");
            _dgvSonUretimler.Rows.Add("01.02.2026 12:55", "Anadolu Sigorta", "Ali Çelik", "Kasko", "4.100 TL");
            _dgvSonUretimler.Rows.Add("01.02.2026 12:30", "Allianz Sigorta", "Zeynep Öztürk", "Trafik", "2.200 TL");
        }
    }
}
