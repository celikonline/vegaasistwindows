using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class TopluKullaniciBilgileriForm : Form
    {
        private Panel _topPanel;
        private Label _lblBaslik;
        private Label _lblToplam;
        private DataGridView _dgvKullanicilar;
        private Panel _bottomPanel;
        private Button _btnExcel;
        private Button _btnKapat;

        public TopluKullaniciBilgileriForm()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void InitializeComponent()
        {
            Text = "Toplu Kullanıcı Bilgileri";
            Size = new Size(700, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Üst Panel
            _topPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                Padding = new Padding(10, 10, 10, 5),
                BackColor = SystemColors.Control
            };

            _lblBaslik = new Label
            {
                Text = "Seçili Kullanıcılar",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            _lblToplam = new Label
            {
                Text = "Toplam: 0 kullanıcı",
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Location = new Point(10, 35)
            };

            _topPanel.Controls.Add(_lblBaslik);
            _topPanel.Controls.Add(_lblToplam);

            // DataGridView
            _dgvKullanicilar = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                MultiSelect = false,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.None
            };

            // Kolonları ekle
            _dgvKullanicilar.Columns.Add("KullaniciAdi", "Kullanıcı Adı");
            _dgvKullanicilar.Columns.Add("AdSoyad", "Ad Soyad");
            _dgvKullanicilar.Columns.Add("Email", "Email");
            _dgvKullanicilar.Columns.Add("Rol", "Rol");
            _dgvKullanicilar.Columns.Add("Grup", "Grup");
            _dgvKullanicilar.Columns.Add("SonGiris", "Son Giriş");

            // Kolon genişliklerini ayarla
            _dgvKullanicilar.Columns["KullaniciAdi"].Width = 120;
            _dgvKullanicilar.Columns["AdSoyad"].Width = 150;
            _dgvKullanicilar.Columns["Email"].Width = 180;
            _dgvKullanicilar.Columns["Rol"].Width = 100;
            _dgvKullanicilar.Columns["Grup"].Width = 100;
            _dgvKullanicilar.Columns["SonGiris"].Width = 150;

            // Alt Panel
            _bottomPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10),
                BackColor = SystemColors.Control
            };

            _btnExcel = new Button
            {
                Text = "Excel'e Aktar",
                Width = 120,
                Height = 30
            };
            _btnExcel.Click += BtnExcel_Click;

            _btnKapat = new Button
            {
                Text = "Kapat",
                DialogResult = DialogResult.Cancel,
                Width = 80,
                Height = 30
            };

            _bottomPanel.Controls.Add(_btnExcel);
            _bottomPanel.Controls.Add(_btnKapat);

            // Butonları konumlandır
            void PositionButtons(object s, EventArgs e)
            {
                _btnKapat.Location = new Point(_bottomPanel.Width - _btnKapat.Width - 10, (_bottomPanel.Height - _btnKapat.Height) / 2);
                _btnExcel.Location = new Point(_btnKapat.Left - _btnExcel.Width - 10, (_bottomPanel.Height - _btnExcel.Height) / 2);
            }

            _bottomPanel.Layout += PositionButtons;
            PositionButtons(null, EventArgs.Empty);

            // Ana Panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(_dgvKullanicilar);
            mainPanel.Controls.Add(_topPanel);
            mainPanel.Controls.Add(_bottomPanel);
            mainPanel.Controls.SetChildIndex(_topPanel, 0);
            mainPanel.Controls.SetChildIndex(_bottomPanel, 1);
            mainPanel.Controls.SetChildIndex(_dgvKullanicilar, 2);

            Controls.Add(mainPanel);

            CancelButton = _btnKapat;
        }

        private void LoadSampleData()
        {
            _dgvKullanicilar.Rows.Clear();

            // Örnek veri ekle
            _dgvKullanicilar.Rows.Add("ahmet.yilmaz", "Ahmet Yılmaz", "ahmet.yilmaz@example.com", "Admin", "Yönetim", "2026-01-28 14:30");
            _dgvKullanicilar.Rows.Add("mehmet.demir", "Mehmet Demir", "mehmet.demir@example.com", "Kullanıcı", "Satış", "2026-01-29 09:15");
            _dgvKullanicilar.Rows.Add("ayse.kaya", "Ayşe Kaya", "ayse.kaya@example.com", "Editör", "Pazarlama", "2026-01-30 11:45");
            _dgvKullanicilar.Rows.Add("fatma.celik", "Fatma Çelik", "fatma.celik@example.com", "Kullanıcı", "Destek", "2026-01-31 16:20");
            _dgvKullanicilar.Rows.Add("ali.ozkan", "Ali Özkan", "ali.ozkan@example.com", "Moderatör", "IT", "2026-02-01 08:00");

            _lblToplam.Text = $"Toplam: {_dgvKullanicilar.Rows.Count} kullanıcı";
        }

        private void BtnExcel_Click(object sender, EventArgs e)
        {
            // Excel'e aktarma işlevi buraya eklenecek
            MessageBox.Show("Excel'e aktarma özelliği yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
