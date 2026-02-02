using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Windows;

namespace VegaAsis.Windows.Forms
{
    public class TramerSorguForm : Form
    {
        private GroupBox _grpSorgu;
        private RadioButton _rbPlaka;
        private RadioButton _rbSasiNo;
        private TextBox _txtPlaka;
        private TextBox _txtSasiNo;
        private Button _btnSorgula;
        private GroupBox _grpSonuclar;
        private DataGridView _dgvSonuclar;
        private Button _btnKapat;

        public TramerSorguForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Tramer Sorgusu";
            Size = new Size(620, 480);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(500, 400);

            _grpSorgu = new GroupBox
            {
                Text = "Tramer Sorgu Bilgileri",
                Location = new Point(12, 12),
                Size = new Size(586, 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            _rbPlaka = new RadioButton { Text = "Plaka ile Sorgula", Location = new Point(12, 22), Checked = true };
            _rbSasiNo = new RadioButton { Text = "Şasi No ile Sorgula", Location = new Point(12, 48) };
            _txtPlaka = new TextBox { Left = 200, Top = 20, Width = 220 };
            _txtSasiNo = new TextBox { Left = 200, Top = 70, Width = 220, Enabled = false };
            _grpSorgu.Controls.Add(new Label { Text = "Plaka:", Left = 200, Top = 2 });
            _grpSorgu.Controls.Add(_txtPlaka);
            _grpSorgu.Controls.Add(new Label { Text = "Şasi No:", Left = 200, Top = 52 });
            _grpSorgu.Controls.Add(_txtSasiNo);
            _grpSorgu.Controls.Add(_rbPlaka);
            _grpSorgu.Controls.Add(_rbSasiNo);
            _rbPlaka.CheckedChanged += (s, e) => { if (_rbPlaka.Checked) { _txtPlaka.Enabled = true; _txtSasiNo.Enabled = false; _txtSasiNo.Clear(); } };
            _rbSasiNo.CheckedChanged += (s, e) => { if (_rbSasiNo.Checked) { _txtPlaka.Enabled = false; _txtPlaka.Clear(); _txtSasiNo.Enabled = true; } };
            _btnSorgula = new Button { Text = "Sorgula", Location = new Point(440, 45), Size = new Size(120, 32) };
            _btnSorgula.Click += BtnSorgula_Click;
            _grpSorgu.Controls.Add(_btnSorgula);
            Controls.Add(_grpSorgu);

            _grpSonuclar = new GroupBox { Text = "Tramer Sonuçları", Dock = DockStyle.Fill };
            _dgvSonuclar = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvSonuclar.Columns.Add("Plaka", "Plaka");
            _dgvSonuclar.Columns.Add("Marka", "Marka");
            _dgvSonuclar.Columns.Add("Model", "Model");
            _dgvSonuclar.Columns.Add("HasarTarihi", "Hasar Tarihi");
            _dgvSonuclar.Columns.Add("Sirket", "Şirket");
            _grpSonuclar.Controls.Add(_dgvSonuclar);
            Controls.Add(_grpSonuclar);

            var pnlAlt = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            _btnKapat = new Button { Text = "Kapat", Size = new Size(90, 28), Location = new Point(502, 10), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            _btnKapat.Click += (s, e) => Close();
            pnlAlt.Controls.Add(_btnKapat);
            Controls.Add(pnlAlt);

            AcceptButton = _btnSorgula;
            CancelButton = _btnKapat;
            ShowInTaskbar = false;
        }

        private async void BtnSorgula_Click(object sender, EventArgs e)
        {
            var val = _rbPlaka.Checked ? _txtPlaka.Text?.Trim() : _txtSasiNo.Text?.Trim();
            if (string.IsNullOrEmpty(val))
            {
                MessageBox.Show("Lütfen Plaka veya Şasi No girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _dgvSonuclar.Rows.Clear();
            ITramerService service = null;
            try
            {
                service = ServiceLocator.Resolve<ITramerService>();
            }
            catch
            {
                MessageBox.Show("Tramer servisi kullanılamıyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            IReadOnlyList<TramerSonucDto> list = null;
            try
            {
                if (_rbPlaka.Checked)
                    list = await service.SorgulaPlakaAsync(val).ConfigureAwait(true);
                else
                    list = await service.SorgulaSasiNoAsync(val).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorgu hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (list != null)
            {
                foreach (var row in list)
                {
                    _dgvSonuclar.Rows.Add(
                        row.Plaka ?? "-",
                        row.Marka ?? "-",
                        row.Model ?? "-",
                        row.HasarTarihi.HasValue ? row.HasarTarihi.Value.ToString("dd.MM.yyyy") : "-",
                        row.Sirket ?? "-");
                }
            }
            if (_dgvSonuclar.Rows.Count == 0)
                MessageBox.Show("Sonuç bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
