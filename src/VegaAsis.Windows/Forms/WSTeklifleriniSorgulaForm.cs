using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class WSTeklifleriniSorgulaForm : Form
    {
        private ComboBox _cmbSablon;
        private DateTimePicker _dtpBaslangic;
        private DateTimePicker _dtpBitis;
        private Button _btnSorgula;
        private DataGridView _dgvSonuclar;
        private Button _btnKapat;
        private ProgressBar _progressBar;
        private Label _lblDurum;

        public WSTeklifleriniSorgulaForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "WS Tekliflerini Sorgula";
            Size = new Size(700, 480);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(600, 400);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(12) };
            int y = 12;
            topPanel.Controls.Add(new Label { Text = "Şablon:", Left = 12, Top = y });
            _cmbSablon = new ComboBox { Left = 100, Top = y - 2, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbSablon.Items.AddRange(new[] { "Varsayılan Trafik", "Kasko Hızlı", "DASK Toplu" });
            if (_cmbSablon.Items.Count > 0) _cmbSablon.SelectedIndex = 0;
            topPanel.Controls.Add(_cmbSablon);
            y += 35;
            topPanel.Controls.Add(new Label { Text = "Başlangıç:", Left = 12, Top = y });
            _dtpBaslangic = new DateTimePicker { Left = 100, Top = y - 2, Width = 150 };
            topPanel.Controls.Add(_dtpBaslangic);
            topPanel.Controls.Add(new Label { Text = "Bitiş:", Left = 270, Top = y });
            _dtpBitis = new DateTimePicker { Left = 320, Top = y - 2, Width = 150 };
            topPanel.Controls.Add(_dtpBitis);
            _btnSorgula = new Button { Text = "Sorgula", Size = new Size(100, 28), Location = new Point(490, y - 4) };
            _btnSorgula.Click += BtnSorgula_Click;
            topPanel.Controls.Add(_btnSorgula);
            _lblDurum = new Label { Text = "", Left = 12, Top = 70, AutoSize = true };
            topPanel.Controls.Add(_lblDurum);
            _progressBar = new ProgressBar { Left = 100, Top = 68, Width = 490, Height = 20 };
            topPanel.Controls.Add(_progressBar);
            Controls.Add(topPanel);

            _dgvSonuclar = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvSonuclar.Columns.Add("Tarih", "Tarih");
            _dgvSonuclar.Columns.Add("Plaka", "Plaka");
            _dgvSonuclar.Columns.Add("Sirket", "Şirket");
            _dgvSonuclar.Columns.Add("Brans", "Branş");
            _dgvSonuclar.Columns.Add("Fiyat", "Fiyat");
            Controls.Add(_dgvSonuclar);

            var pnlAlt = new Panel { Dock = DockStyle.Bottom, Height = 45 };
            _btnKapat = new Button { Text = "Kapat", Size = new Size(80, 28), Location = new Point(608, 8) };
            _btnKapat.Click += (s, e) => Close();
            pnlAlt.Controls.Add(_btnKapat);
            Controls.Add(pnlAlt);
        }

        private void BtnSorgula_Click(object sender, EventArgs e)
        {
            _lblDurum.Text = "Sorgulanıyor...";
            _progressBar.Style = ProgressBarStyle.Marquee;
            _dgvSonuclar.Rows.Clear();
            _dgvSonuclar.Rows.Add(DateTime.Now.ToString("dd.MM.yyyy"), "06ABC01", "ANADOLU", "TRAFİK", "1.250,00");
            _dgvSonuclar.Rows.Add(DateTime.Now.ToString("dd.MM.yyyy"), "34XYZ99", "HDI", "KASKO", "3.400,00");
            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.Value = 100;
            _lblDurum.Text = "Tamamlandı.";
            MessageBox.Show("WS teklif sorgusu tamamlandı. (Entegrasyon bekleniyor)", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
