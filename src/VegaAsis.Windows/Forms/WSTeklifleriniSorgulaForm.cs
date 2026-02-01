using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Windows;

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
        private List<SablonDto> _sablonlar = new List<SablonDto>();

        public WSTeklifleriniSorgulaForm()
        {
            InitializeComponent();
            Load += WSTeklifleriniSorgulaForm_Load;
        }

        private async void WSTeklifleriniSorgulaForm_Load(object sender, EventArgs e)
        {
            if (ServiceLocator.IsInitialized)
            {
                try
                {
                    var sablonService = ServiceLocator.Resolve<ISablonService>();
                    var list = await sablonService.GetAllAsync().ConfigureAwait(true);
                    _sablonlar = list != null ? list.ToList() : new List<SablonDto>();
                    _cmbSablon.Items.Clear();
                    _cmbSablon.DisplayMember = "Ad";
                    foreach (var s in _sablonlar)
                        _cmbSablon.Items.Add(s);
                    if (_cmbSablon.Items.Count > 0)
                        _cmbSablon.SelectedIndex = 0;
                }
                catch
                {
                    _cmbSablon.Items.AddRange(new object[] { "Varsayılan Trafik", "Kasko Hızlı", "DASK Toplu" });
                    if (_cmbSablon.Items.Count > 0) _cmbSablon.SelectedIndex = 0;
                }
            }
            else
            {
                _cmbSablon.Items.AddRange(new object[] { "Varsayılan Trafik", "Kasko Hızlı", "DASK Toplu" });
                if (_cmbSablon.Items.Count > 0) _cmbSablon.SelectedIndex = 0;
            }
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

            AcceptButton = _btnSorgula;
            CancelButton = _btnKapat;
            ShowInTaskbar = false;
        }

        private async void BtnSorgula_Click(object sender, EventArgs e)
        {
            if (_dtpBitis.Value < _dtpBaslangic.Value)
            {
                MessageBox.Show("Bitiş tarihi başlangıçtan önce olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _lblDurum.Text = "Sorgulanıyor...";
            _progressBar.Style = ProgressBarStyle.Marquee;
            _progressBar.Visible = true;
            _dgvSonuclar.Rows.Clear();

            if (ServiceLocator.IsInitialized)
            {
                try
                {
                    int? sablonId = null;
                    if (_cmbSablon.SelectedItem is SablonDto sd)
                        sablonId = sd.Id;
                    var wsService = ServiceLocator.Resolve<IWsSorguService>();
                    var sonuclar = await wsService.SorgulaAsync(sablonId, _dtpBaslangic.Value.Date, _dtpBitis.Value.Date).ConfigureAwait(true);
                    foreach (var row in sonuclar ?? Enumerable.Empty<WSTeklifSonucDto>())
                    {
                        var tarihStr = row.Tarih.HasValue ? row.Tarih.Value.ToString("dd.MM.yyyy") : "";
                        _dgvSonuclar.Rows.Add(tarihStr, row.Plaka ?? "", row.Sirket ?? "", row.Brans ?? "", row.Fiyat ?? "");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sorgu hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                _dgvSonuclar.Rows.Add(DateTime.Now.ToString("dd.MM.yyyy"), "06ABC01", "ANADOLU", "TRAFİK", "1.250,00");
                _dgvSonuclar.Rows.Add(DateTime.Now.ToString("dd.MM.yyyy"), "34XYZ99", "HDI", "KASKO", "3.400,00");
            }

            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.Value = 100;
            _lblDurum.Text = "Tamamlandı.";
        }
    }
}
