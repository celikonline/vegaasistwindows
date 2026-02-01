using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class SbmSorgusuForm : Form
    {
        private GroupBox _grpSorguBilgileri;
        private RadioButton _rbTcKimlik;
        private RadioButton _rbPlaka;
        private Label _lblTcVergi;
        private TextBox _txtTcVergi;
        private Label _lblPlaka;
        private TextBox _txtPlaka;
        private Button _btnSorgula;
        private GroupBox _grpSonuclar;
        private DataGridView _dgvSonuclar;
        private Panel _pnlAlt;
        private Button _btnKapat;

        public SbmSorgusuForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "SBM Sorgusu";
            Size = new Size(600, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // GroupBox: Sorgu Bilgileri (üst)
            _grpSorguBilgileri = new GroupBox
            {
                Text = "Sorgu Bilgileri",
                Location = new Point(12, 12),
                Size = new Size(576, 120)
            };

            // RadioButton: TC Kimlik ile Sorgula (varsayılan seçili)
            _rbTcKimlik = new RadioButton
            {
                Text = "TC Kimlik ile Sorgula",
                Location = new Point(12, 20),
                Checked = true
            };
            _rbTcKimlik.CheckedChanged += RbTcKimlik_CheckedChanged;

            // RadioButton: Plaka ile Sorgula
            _rbPlaka = new RadioButton
            {
                Text = "Plaka ile Sorgula",
                Location = new Point(12, 45)
            };
            _rbPlaka.CheckedChanged += RbPlaka_CheckedChanged;

            // Label + TextBox: TC Kimlik No / Vergi No
            _lblTcVergi = new Label
            {
                Text = "TC Kimlik No / Vergi No:",
                Location = new Point(200, 25),
                AutoSize = true
            };
            _txtTcVergi = new TextBox
            {
                Location = new Point(200, 45),
                Width = 200
            };

            // Label + TextBox: Plaka
            _lblPlaka = new Label
            {
                Text = "Plaka:",
                Location = new Point(200, 75),
                AutoSize = true
            };
            _txtPlaka = new TextBox
            {
                Location = new Point(200, 95),
                Width = 200,
                Enabled = false
            };

            _grpSorguBilgileri.Controls.Add(_rbTcKimlik);
            _grpSorguBilgileri.Controls.Add(_rbPlaka);
            _grpSorguBilgileri.Controls.Add(_lblTcVergi);
            _grpSorguBilgileri.Controls.Add(_txtTcVergi);
            _grpSorguBilgileri.Controls.Add(_lblPlaka);
            _grpSorguBilgileri.Controls.Add(_txtPlaka);

            // Button: Sorgula
            _btnSorgula = new Button
            {
                Text = "Sorgula",
                Location = new Point(420, 50),
                Size = new Size(120, 35)
            };
            _btnSorgula.Click += BtnSorgula_Click;
            _grpSorguBilgileri.Controls.Add(_btnSorgula);

            Controls.Add(_grpSorguBilgileri);

            // Alt Panel (önce eklenmeli ki Dock = Fill olan GroupBox üzerine gelmesin)
            _pnlAlt = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom
            };

            // Button: Kapat
            _btnKapat = new Button
            {
                Text = "Kapat",
                Size = new Size(100, 30),
                Location = new Point(488, 10),
                DialogResult = DialogResult.Cancel
            };
            _btnKapat.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            _pnlAlt.Controls.Add(_btnKapat);

            Controls.Add(_pnlAlt);

            // GroupBox: Sonuçlar (alt, Dock = Fill)
            _grpSonuclar = new GroupBox
            {
                Text = "Sonuçlar",
                Dock = DockStyle.Fill
            };

            // DataGridView
            _dgvSonuclar = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Kolonları ekle
            _dgvSonuclar.Columns.Add("PoliceTuru", "Poliçe Türü");
            _dgvSonuclar.Columns.Add("Sirket", "Şirket");
            _dgvSonuclar.Columns.Add("Baslangic", "Başlangıç");
            _dgvSonuclar.Columns.Add("Bitis", "Bitiş");
            _dgvSonuclar.Columns.Add("Durum", "Durum");

            _grpSonuclar.Controls.Add(_dgvSonuclar);
            Controls.Add(_grpSonuclar);
        }

        private void RbTcKimlik_CheckedChanged(object sender, EventArgs e)
        {
            if (_rbTcKimlik.Checked)
            {
                _txtTcVergi.Enabled = true;
                _txtPlaka.Enabled = false;
                _txtPlaka.Clear();
            }
        }

        private void RbPlaka_CheckedChanged(object sender, EventArgs e)
        {
            if (_rbPlaka.Checked)
            {
                _txtTcVergi.Enabled = false;
                _txtTcVergi.Clear();
                _txtPlaka.Enabled = true;
            }
        }

        private void BtnSorgula_Click(object sender, EventArgs e)
        {
            MessageBox.Show("SBM sorgusu başlatılıyor...", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
