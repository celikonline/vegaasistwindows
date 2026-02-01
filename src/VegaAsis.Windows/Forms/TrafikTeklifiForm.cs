using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class TrafikTeklifiForm : Form
    {
        private Panel _leftPanel;
        private Panel _rightPanel;
        private GroupBox _grpAracBilgileri;
        private GroupBox _grpMusteriBilgileri;
        private TextBox _txtPlaka;
        private TextBox _txtBelgeSeriNo;
        private TextBox _txtBelgeSiraNo;
        private TextBox _txtTcKimlikVergiNo;
        private TextBox _txtAdSoyadUnvan;
        private TextBox _txtTelefon;
        private Button _btnSorgula;
        private DataGridView _dgvSonuclar;

        public TrafikTeklifiForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Trafik Teklifi";
            Size = new Size(800, 600);
            StartPosition = FormStartPosition.CenterParent;

            // Sol Panel
            _leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                Padding = new Padding(10)
            };

            // Araç Bilgileri GroupBox
            _grpAracBilgileri = new GroupBox
            {
                Text = "Araç Bilgileri",
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(5)
            };

            var lblPlaka = new Label { Text = "Plaka:", Location = new Point(10, 25), Width = 120 };
            _txtPlaka = new TextBox { Location = new Point(10, 45), Width = 260 };

            var lblBelgeSeriNo = new Label { Text = "Belge Seri No:", Location = new Point(10, 70), Width = 120 };
            _txtBelgeSeriNo = new TextBox { Location = new Point(10, 90), Width = 260 };

            var lblBelgeSiraNo = new Label { Text = "Belge Sıra No:", Location = new Point(10, 115), Width = 120 };
            _txtBelgeSiraNo = new TextBox { Location = new Point(10, 135), Width = 260 };

            _grpAracBilgileri.Controls.AddRange(new Control[] { lblPlaka, _txtPlaka, lblBelgeSeriNo, _txtBelgeSeriNo, lblBelgeSiraNo, _txtBelgeSiraNo });

            // Müşteri Bilgileri GroupBox
            _grpMusteriBilgileri = new GroupBox
            {
                Text = "Müşteri Bilgileri",
                Dock = DockStyle.Top,
                Height = 140,
                Padding = new Padding(5)
            };

            var lblTcKimlikVergiNo = new Label { Text = "TC Kimlik / Vergi No:", Location = new Point(10, 25), Width = 120 };
            _txtTcKimlikVergiNo = new TextBox { Location = new Point(10, 45), Width = 260 };

            var lblAdSoyadUnvan = new Label { Text = "Ad Soyad / Ünvan:", Location = new Point(10, 70), Width = 120 };
            _txtAdSoyadUnvan = new TextBox { Location = new Point(10, 90), Width = 260 };

            var lblTelefon = new Label { Text = "Telefon:", Location = new Point(10, 115), Width = 120 };
            _txtTelefon = new TextBox { Location = new Point(10, 135), Width = 260 };

            _grpMusteriBilgileri.Controls.AddRange(new Control[] { lblTcKimlikVergiNo, _txtTcKimlikVergiNo, lblAdSoyadUnvan, _txtAdSoyadUnvan, lblTelefon, _txtTelefon });

            // Sorgula Butonu
            _btnSorgula = new Button
            {
                Text = "Teklif Al",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            _btnSorgula.Click += BtnSorgula_Click;

            // Sol panel'e kontrolleri ekle
            _leftPanel.Controls.Add(_btnSorgula);
            _leftPanel.Controls.Add(_grpMusteriBilgileri);
            _leftPanel.Controls.Add(_grpAracBilgileri);

            // Sağ Panel
            _rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // DataGridView
            _dgvSonuclar = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // DataGridView kolonlarını oluştur
            _dgvSonuclar.Columns.Add("Sirket", "Şirket");
            _dgvSonuclar.Columns.Add("TrafikPrimi", "Trafik Primi");
            _dgvSonuclar.Columns.Add("IMM", "İMM");
            _dgvSonuclar.Columns.Add("Toplam", "Toplam");
            _dgvSonuclar.Columns.Add("TeklifNo", "Teklif No");

            _rightPanel.Controls.Add(_dgvSonuclar);

            // Ana kontrollere ekle
            Controls.Add(_rightPanel);
            Controls.Add(_leftPanel);
        }

        private void BtnSorgula_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sorgu başlatılıyor...", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
