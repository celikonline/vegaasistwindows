using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class ManuelUavtSorguForm : Form
    {
        private GroupBox _grpSorgu;
        private RadioButton _rbTcKimlik;
        private RadioButton _rbPlaka;
        private TextBox _txtTcVergi;
        private TextBox _txtPlaka;
        private Button _btnSorgula;
        private GroupBox _grpSonuclar;
        private DataGridView _dgvSonuclar;
        private Button _btnKapat;

        public ManuelUavtSorguForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Manuel UAVT Sorgusu";
            Size = new Size(620, 480);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(500, 400);

            _grpSorgu = new GroupBox
            {
                Text = "UAVT Sorgu Bilgileri",
                Location = new Point(12, 12),
                Size = new Size(586, 110)
            };
            _rbTcKimlik = new RadioButton { Text = "TC Kimlik / Vergi No ile", Location = new Point(12, 22), Checked = true };
            _rbPlaka = new RadioButton { Text = "Plaka ile", Location = new Point(12, 48) };
            _txtTcVergi = new TextBox { Left = 200, Top = 20, Width = 220 };
            _txtPlaka = new TextBox { Left = 200, Top = 70, Width = 220, Enabled = false };
            _grpSorgu.Controls.Add(new Label { Text = "TC / Vergi No:", Left = 200, Top = 2 });
            _grpSorgu.Controls.Add(_txtTcVergi);
            _grpSorgu.Controls.Add(new Label { Text = "Plaka:", Left = 200, Top = 52 });
            _grpSorgu.Controls.Add(_txtPlaka);
            _grpSorgu.Controls.Add(_rbTcKimlik);
            _grpSorgu.Controls.Add(_rbPlaka);
            _rbTcKimlik.CheckedChanged += (s, e) => { if (_rbTcKimlik.Checked) { _txtTcVergi.Enabled = true; _txtPlaka.Enabled = false; _txtPlaka.Clear(); } };
            _rbPlaka.CheckedChanged += (s, e) => { if (_rbPlaka.Checked) { _txtTcVergi.Enabled = false; _txtTcVergi.Clear(); _txtPlaka.Enabled = true; } };
            _btnSorgula = new Button { Text = "Sorgula", Location = new Point(440, 45), Size = new Size(120, 32) };
            _btnSorgula.Click += BtnSorgula_Click;
            _grpSorgu.Controls.Add(_btnSorgula);
            Controls.Add(_grpSorgu);

            _grpSonuclar = new GroupBox { Text = "Sonuçlar", Dock = DockStyle.Fill };
            _dgvSonuclar = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvSonuclar.Columns.Add("Kaynak", "Kaynak");
            _dgvSonuclar.Columns.Add("Tarih", "Tarih");
            _dgvSonuclar.Columns.Add("Aciklama", "Açıklama");
            _dgvSonuclar.Columns.Add("Durum", "Durum");
            _grpSonuclar.Controls.Add(_dgvSonuclar);
            Controls.Add(_grpSonuclar);

            var pnlAlt = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            _btnKapat = new Button { Text = "Kapat", Size = new Size(90, 28), Location = new Point(502, 10) };
            _btnKapat.Click += (s, e) => Close();
            pnlAlt.Controls.Add(_btnKapat);
            Controls.Add(pnlAlt);
        }

        private void BtnSorgula_Click(object sender, EventArgs e)
        {
            var val = _rbTcKimlik.Checked ? _txtTcVergi.Text?.Trim() : _txtPlaka.Text?.Trim();
            if (string.IsNullOrEmpty(val))
            {
                MessageBox.Show("Lütfen sorgu değeri girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _dgvSonuclar.Rows.Clear();
            _dgvSonuclar.Rows.Add("UAVT", DateTime.Now.ToString("dd.MM.yyyy HH:mm"), _rbTcKimlik.Checked ? "TC/Vergi sorgusu" : "Plaka sorgusu", "Beklemede");
            MessageBox.Show("UAVT sorgusu başlatıldı. (Entegrasyon bekleniyor)", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
