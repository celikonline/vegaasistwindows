using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using VegaAsis.Windows.Data;
using VegaAsis.Windows.Forms;
using VegaAsis.Windows.Models;

namespace VegaAsis.Windows.UserControls
{
    public class IndexViewControl : UserControl
    {
        private static readonly Color VegaGreen = Color.FromArgb(46, 125, 50);
        private static readonly Color VegaGreenLight = Color.FromArgb(102, 187, 106);
        private static readonly Color VegaRed = Color.FromArgb(198, 40, 40);
        private static readonly Color VegaOrange = Color.FromArgb(239, 108, 0);

        private DataGridView _companyGrid;
        private Panel _rightActionPanel;
        private TabControl _aracBilgileriTabs;
        private SplitContainer _mainSplit;
        private bool _splitterInitialized;
        private bool _allExpanded;
        private Button _btnDigierFiyatlar;
        private ComboBox _cmbTarayici;
        private ComboBox _cmbPolicyType;
        private ComboBox _cmbIl;
        private ComboBox _cmbIlce;
        private Panel _selectedCompanyPanel;
        private List<CompanyRow> _companyData;
        private Button _btnDuraklat;
        private SorguSession _sorguSession = new SorguSession();
        private readonly Dictionary<string, Control> _formFields = new Dictionary<string, Control>();

        public SorguSession Session { get { return _sorguSession; } }

        public List<FiyatSatir> GetFiyatSatirlari()
        {
            if (_companyData == null) return new List<FiyatSatir>();
            return _companyData.Select(c => new FiyatSatir
            {
                Sirket = c.Name,
                Trafik = c.Price,
                Yuzde = c.Percentage,
                Uyari = c.Warning
            }).ToList();
        }

        public event EventHandler SbmSorgusuRequested;
        public event EventHandler KuyrukSorgusuRequested;
        public event EventHandler WebcamQRRequested;
        public event EventHandler SirketEkleRequested;
        public event EventHandler AdminPanelRequested;
        public event EventHandler<string> BranchFormRequested;
        public event EventHandler TekliflerRequested;
        public event EventHandler PolicelerimRequested;
        public event EventHandler SirketlerRobotRequested;
        public event EventHandler RaporlarRequested;
        public event EventHandler DestekTalepleriRequested;
        public event EventHandler AjandaYenilemeRequested;
        public event EventHandler DuyurularRequested;
        public event EventHandler CanliUretimRequested;
        public event EventHandler CanliDestekRequested;
        public event EventHandler PdfExportRequested;
        public event EventHandler PdfUploadRequested;
        public event EventHandler SorguBaslatRequested;
        public event EventHandler DuraklatRequested;
        public event EventHandler YeniSorguKaydetRequested;
        public event EventHandler<BranchCellClickEventArgs> BranchCellClickRequested;

        public IndexViewControl()
        {
            SuspendLayout();
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            MinimumSize = new Size(1000, 600);

            BuildTopHeader();
            BuildToolbar();
            BuildMainContent();
            BuildStatusBar();
            Load += (s, e) => LoadCompanyData();

            ResumeLayout(true);
        }

        private void LoadCompanyData()
        {
            _companyData = GetCompanyData();
            _companyGrid.SelectionChanged -= CompanyGrid_SelectionChanged;
            _companyGrid.SelectionChanged += CompanyGrid_SelectionChanged;
            RefreshCompanyGrid();
            RefreshSelectedCompanyPanel();
        }

        private void RefreshCompanyGrid()
        {
            if (_companyGrid == null || _companyData == null) return;
            _companyGrid.Rows.Clear();
            foreach (var c in _companyData)
            {
                var trafikStr = c.Price.HasValue ? c.Price.Value.ToString("N2", new CultureInfo("tr-TR")) : "0.00";
                var yuzdeStr = c.Percentage.HasValue ? "%" + c.Percentage.Value : "%0";
                var idx = _companyGrid.Rows.Add(false, c.Name, trafikStr, "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", yuzdeStr, c.Warning ?? "");
                var row = _companyGrid.Rows[idx];
                row.Cells["Secim"].Value = c.Price.HasValue;
                row.Tag = new RowTag { Company = c, SubPrice = null };

                if (_allExpanded && c.SubPrices != null && c.SubPrices.Length > 0)
                {
                    foreach (var sp in c.SubPrices)
                    {
                        var spTrafik = sp.Price.ToString("N2", new CultureInfo("tr-TR"));
                        var subIdx = _companyGrid.Rows.Add(false, "  \u2192 " + (sp.Branch ?? sp.Product), spTrafik, "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "", sp.Description ?? "");
                        var subRow = _companyGrid.Rows[subIdx];
                        subRow.Tag = new RowTag { Company = c, SubPrice = sp };
                        subRow.DefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);
                        subRow.DefaultCellStyle.ForeColor = Color.Gray;
                    }
                }
            }
        }

        private void CompanyGrid_SelectionChanged(object sender, EventArgs e)
        {
            RefreshSelectedCompanyPanel();
        }

        private void BtnSorguBaslat_Click(object sender, EventArgs e)
        {
            SyncToSession();
            _sorguSession.Durum = SorguDurum.Running;
            _sorguSession.AktifBrans = _cmbPolicyType?.SelectedItem as string;
            _btnDuraklat.Enabled = true;
            _btnDuraklat.Text = "Duraklat";
            SorguBaslatRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnDuraklat_Click(object sender, EventArgs e)
        {
            if (_sorguSession.Durum == SorguDurum.Running)
            {
                _sorguSession.Durum = SorguDurum.Paused;
                _btnDuraklat.Text = "Devam Et";
            }
            else if (_sorguSession.Durum == SorguDurum.Paused)
            {
                _sorguSession.Durum = SorguDurum.Running;
                _btnDuraklat.Text = "Duraklat";
            }
            DuraklatRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnYeniSorgu_Click(object sender, EventArgs e)
        {
            _sorguSession.Reset();
            LoadFromSession();
            _btnDuraklat.Enabled = false;
            _btnDuraklat.Text = "Duraklat / Devam Et";
            YeniSorguKaydetRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SyncToSession()
        {
            _sorguSession.Plaka = GetText("Plaka");
            _sorguSession.TcVergi = GetText("TcVergi");
            _sorguSession.BelgeSeri = GetText("BelgeSeri");
            _sorguSession.BelgeNo = GetText("BelgeNo");
            _sorguSession.MusteriAdi = GetText("Musteri");
            _sorguSession.Meslek = GetComboText("Meslek");
            _sorguSession.Il = _cmbIl?.SelectedItem as string;
            _sorguSession.Ilce = _cmbIlce?.SelectedItem as string;
            _sorguSession.KullanimTarzi = GetComboText("KullanimTarzi");
            _sorguSession.Marka = GetComboText("Marka");
            _sorguSession.Tip = GetComboText("Tip");
            int modYil;
            if (int.TryParse(GetText("ModelYili"), out modYil))
                _sorguSession.ModelYili = modYil;
            _sorguSession.TrafikSigortaSirketi = GetComboText("TrafikSigortaSirketi");
            _sorguSession.TrafikAcenteKodu = GetText("TrafikAcenteKodu");
            _sorguSession.TrafikPoliceNo = GetText("TrafikPoliceNo");
            DateTime dt;
            if (DateTime.TryParse(GetText("DogumTarihi"), out dt))
                _sorguSession.DogumTarihi = dt;
            if (DateTime.TryParse(GetText("TrafikBaslangicTarihi"), out dt))
                _sorguSession.TrafikBaslangicTarihi = dt;
            if (DateTime.TryParse(GetText("TrafikBitisTarihi"), out dt))
                _sorguSession.TrafikBitisTarihi = dt;
            _sorguSession.KisaVadeliPolice = GetCheckBoxValue("KisaVadeli");

            _sorguSession.KaskoSigortaSirketi = GetComboText("KaskoSigortaSirketi");
            _sorguSession.KaskoAcenteKodu = GetText("KaskoAcenteKodu");
            _sorguSession.KaskoPoliceNo = GetText("KaskoPoliceNo");
            if (DateTime.TryParse(GetText("KaskoBaslangicTarihi"), out dt))
                _sorguSession.KaskoBaslangicTarihi = dt;
            if (DateTime.TryParse(GetText("KaskoBitisTarihi"), out dt))
                _sorguSession.KaskoBitisTarihi = dt;

            _sorguSession.SeciliSirketler.Clear();
            if (_companyGrid != null)
            {
                foreach (DataGridViewRow row in _companyGrid.Rows)
                {
                    if (row.IsNewRow) continue;
                    var cell = row.Cells["Secim"];
                    if (cell?.Value is bool && (bool)cell.Value)
                    {
                        var name = row.Cells["Sirket"]?.Value?.ToString();
                        var tag = row.Tag as RowTag;
                        if (tag?.Company != null && tag.SubPrice == null)
                            name = tag.Company.Name;
                        if (!string.IsNullOrWhiteSpace(name) && !_sorguSession.SeciliSirketler.Contains(name))
                            _sorguSession.SeciliSirketler.Add(name.Trim());
                    }
                }
            }
        }

        private void LoadFromSession()
        {
            SetText("Plaka", "");
            SetText("TcVergi", "");
            SetText("BelgeSeri", "");
            SetText("BelgeNo", "");
            SetText("Musteri", "");
            SetCombo("Meslek", "Seçiniz");
            if (_cmbIl != null && _cmbIl.Items.Count > 0) _cmbIl.SelectedIndex = 0;
            if (_cmbIlce != null) _cmbIlce.Items.Clear();
            SetCombo("KullanimTarzi", "KULLANIM TARZI SEÇİNİZ");
            SetCombo("Marka", null);
            SetCombo("Tip", null);
            SetText("ModelYili", "");
            SetCombo("TrafikSigortaSirketi", "Seçiniz");
            SetText("TrafikAcenteKodu", "");
            SetText("TrafikPoliceNo", "");
            SetText("TrafikBaslangicTarihi", _sorguSession.TrafikBaslangicTarihi.HasValue ? _sorguSession.TrafikBaslangicTarihi.Value.ToString("dd.MM.yyyy") : "");
            SetText("TrafikBitisTarihi", _sorguSession.TrafikBitisTarihi.HasValue ? _sorguSession.TrafikBitisTarihi.Value.ToString("dd.MM.yyyy") : "");
            SetText("DogumTarihi", "");
            SetCheckBoxValue("KisaVadeli", _sorguSession.KisaVadeliPolice);
            SetCombo("KaskoSigortaSirketi", "Seçiniz");
            SetText("KaskoAcenteKodu", _sorguSession.KaskoAcenteKodu ?? "");
            SetText("KaskoPoliceNo", _sorguSession.KaskoPoliceNo ?? "");
            SetText("KaskoBaslangicTarihi", _sorguSession.KaskoBaslangicTarihi.HasValue ? _sorguSession.KaskoBaslangicTarihi.Value.ToString("dd.MM.yyyy") : "");
            SetText("KaskoBitisTarihi", _sorguSession.KaskoBitisTarihi.HasValue ? _sorguSession.KaskoBitisTarihi.Value.ToString("dd.MM.yyyy") : "");
        }

        private bool GetCheckBoxValue(string key)
        {
            var c = _formFields.ContainsKey(key) ? _formFields[key] : null;
            var chk = c as CheckBox;
            return chk != null && chk.Checked;
        }

        private void SetCheckBoxValue(string key, bool value)
        {
            var c = _formFields.ContainsKey(key) ? _formFields[key] : null;
            var chk = c as CheckBox;
            if (chk != null) chk.Checked = value;
        }

        private string GetText(string key)
        {
            var c = _formFields.ContainsKey(key) ? _formFields[key] : null;
            return (c as TextBox)?.Text?.Trim() ?? "";
        }

        private string GetComboText(string key)
        {
            var c = _formFields.ContainsKey(key) ? _formFields[key] : null;
            return (c as ComboBox)?.SelectedItem?.ToString() ?? "";
        }

        private void SetText(string key, string value)
        {
            var c = _formFields.ContainsKey(key) ? _formFields[key] : null;
            var tb = c as TextBox;
            if (tb != null) tb.Text = value ?? "";
        }

        private void SetCombo(string key, string value)
        {
            var c = _formFields.ContainsKey(key) ? _formFields[key] : null;
            var cmb = c as ComboBox;
            if (cmb == null) return;
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (string.Equals(cmb.Items[i].ToString(), value ?? "", StringComparison.OrdinalIgnoreCase))
                {
                    cmb.SelectedIndex = i;
                    return;
                }
            }
            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
        }

        private void CompanyGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var row = _companyGrid.Rows[e.RowIndex];
            var tag = row.Tag as RowTag;
            if (tag == null) return;
            var company = tag.Company;
            var companyName = company?.Name ?? row.Cells["Sirket"]?.Value?.ToString();
            var col = _companyGrid.Columns[e.ColumnIndex];

            string branch = null;
            if (col.Name == "Trafik") branch = "TRAFİK";
            else if (col.Name == "Kasko") branch = "KASKO";
            else if (col.Name == "Sbm") branch = "SBM";
            else if (col.Name == "TssAy" || col.Name == "TssYat") branch = "TSS";
            else if (col.Name == "Konut") branch = "KONUT";
            else if (col.Name == "Dask") branch = "DASK";
            else if (col.Name == "Imm") branch = "İMM";

            if (string.IsNullOrEmpty(branch) && tag.SubPrice != null)
                branch = tag.SubPrice.Branch ?? tag.SubPrice.Product;
            if (!string.IsNullOrEmpty(branch) && BranchCellClickRequested != null)
            {
                BranchCellClickRequested.Invoke(this, new BranchCellClickEventArgs(branch, companyName ?? ""));
            }
        }

        private void CompanyGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = _companyGrid.Rows[e.RowIndex];
            var tag = row.Tag as RowTag;
            if (tag == null) return;
            if (tag.SubPrice != null) return;
            var company = tag.Company;

            var activeBranch = _cmbPolicyType?.SelectedItem as string;
            var unavailable = company.UnavailableBranches != null && !string.IsNullOrEmpty(activeBranch) &&
                company.UnavailableBranches.Any(b => string.Equals(b, activeBranch, StringComparison.OrdinalIgnoreCase));

            if (_companyGrid.Columns["Sirket"] != null && e.ColumnIndex == _companyGrid.Columns["Sirket"].Index)
            {
                e.Value = unavailable ? company.Name + " (Yok)" : company.Name;
                if (unavailable)
                {
                    e.CellStyle.Font = new Font(_companyGrid.Font, FontStyle.Strikeout);
                    e.CellStyle.ForeColor = Color.Gray;
                }
            }
            else
            {
                var colName = _companyGrid.Columns[e.ColumnIndex].Name;
                var branchForColumn = GetBranchForColumn(colName);
                if (branchForColumn != null && company.UnavailableBranches != null &&
                    company.UnavailableBranches.Any(b => string.Equals(b, branchForColumn, StringComparison.OrdinalIgnoreCase)))
                {
                    e.CellStyle.Font = new Font(_companyGrid.Font, FontStyle.Strikeout);
                    e.CellStyle.ForeColor = Color.Gray;
                }
            }
        }

        private static string GetBranchForColumn(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) return null;
            switch (columnName)
            {
                case "Trafik": return "TRAFİK";
                case "Sbm": return "SBM";
                case "Kasko": return "KASKO";
                case "TssAy":
                case "TssYat": return "TSS";
                case "Konut": return "KONUT";
                case "Dask": return "DASK";
                case "Imm": return "İMM";
                default: return null;
            }
        }

        public void AddCompany(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName)) return;
            var name = companyName.Trim();
            if (_companyData == null) _companyData = new List<CompanyRow>();
            if (_companyData.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))) return;

            var newCompany = new CompanyRow { Name = name, Status = "none", Percentage = 0 };
            _companyData.Add(newCompany);
            var idx = _companyGrid.Rows.Add(false, name, "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "%0", "");
            var row = _companyGrid.Rows[idx];
            row.Tag = new RowTag { Company = newCompany, SubPrice = null };
        }

        private void RefreshSelectedCompanyPanel()
        {
            if (_selectedCompanyPanel == null || _companyData == null) return;
            _selectedCompanyPanel.Controls.Clear();
            if (_companyGrid.SelectedRows.Count == 0)
            {
                _selectedCompanyPanel.Controls.Add(new Label { Text = "Şirket seçin", Font = new Font("Segoe UI", 9F), ForeColor = Color.Gray, Location = new Point(8, 24) });
                return;
            }
            var row = _companyGrid.SelectedRows[0];
            var tag = row.Tag as RowTag;
            var company = tag?.Company;
            if (company == null)
            {
                _selectedCompanyPanel.Controls.Add(new Label { Text = "Şirket seçin", Font = new Font("Segoe UI", 9F), ForeColor = Color.Gray, Location = new Point(8, 24) });
                return;
            }
            var lblName = new Label { Text = company.Name, Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true, Location = new Point(8, 8) };
            var statusText = company.Status == "green" ? "Aktif" : company.Status == "red" ? "Pasif" : company.Status == "orange" ? "Beklemede" : "Belirsiz";
            var lblStatus = new Label { Text = statusText, Font = new Font("Segoe UI", 9F), ForeColor = Color.Gray, AutoSize = true, Location = new Point(8, 28) };
            _selectedCompanyPanel.Controls.Add(lblName);
            _selectedCompanyPanel.Controls.Add(lblStatus);

            int py = 48;
            if (company.Price.HasValue)
            {
                var priceStr = company.Price.Value.ToString("N2", new CultureInfo("tr-TR")) + " ₺";
                var lblPrice = new Label { Text = "Trafik: " + priceStr, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = VegaGreen, AutoSize = true, Location = new Point(8, py) };
                _selectedCompanyPanel.Controls.Add(lblPrice);
                py += 24;
            }

            if (company.SubPrices != null && company.SubPrices.Length > 0)
            {
                var lblSub = new Label { Text = "Diğer Fiyatlar:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(8, py) };
                _selectedCompanyPanel.Controls.Add(lblSub);
                py += 18;
                foreach (var sp in company.SubPrices)
                {
                    var spText = string.Format("{0}: {1:N2} ₺ (Kom: {2:N2})", sp.Branch, sp.Price, sp.Commission);
                    var lblSp = new Label { Text = spText, Font = new Font("Segoe UI", 8F), ForeColor = Color.Gray, AutoSize = true, Location = new Point(12, py), MaximumSize = new Size(_selectedCompanyPanel.Width - 20, 0), AutoEllipsis = true };
                    _selectedCompanyPanel.Controls.Add(lblSp);
                    py += 16;
                }
                _selectedCompanyPanel.Height = Math.Max(70, py + 8);
            }
        }

        private static List<CompanyRow> GetCompanyData()
        {
            return new List<CompanyRow>
            {
                new CompanyRow { Name = "ACIBADEM", Status = "red", Percentage = 0 },
                new CompanyRow { Name = "AK SİGORTA", Status = "none", Percentage = 80 },
                new CompanyRow { Name = "ANA SİGORTA", Status = "green", Price = 8184.64m, Percentage = 100, SubPrices = new[] { new SubPriceRow { Branch = "TRAFİK", Product = "TRAFİK", Price = 8184.64m, Commission = 734.20m, PolicyNo = "07806799..." } } },
                new CompanyRow { Name = "ANADOLU", Status = "green", Price = 15208.43m, Percentage = 100, Warning = "155/173696 Teklif olarak kaydedilmiştir." },
                new CompanyRow { Name = "ATLAS", Status = "red", Percentage = 0, UnavailableBranches = new[] { "TRAFİK" } },
                new CompanyRow { Name = "AXA", Status = "red", Percentage = 0 },
                new CompanyRow { Name = "CORPUS", Status = "green", Price = 10892.41m, Percentage = 100, Warning = "Teklif olarak kaydedilmiştir.", SubPrices = new[] { new SubPriceRow { Branch = "TRAFİK", Product = "TRAFİK", Price = 10892.41m, Commission = 1374.20m, PolicyNo = "165774433", Description = "Teklif olarak kaydedilmiştir." } } },
                new CompanyRow { Name = "DOGA", Status = "green", Percentage = 100, Warning = "7581 - 310 - Üründe çalışma yapılmaktadır." },
                new CompanyRow { Name = "HDI", Status = "none", Percentage = 0 },
                new CompanyRow { Name = "HDI PLUS", Status = "green", Price = 15948.73m, Percentage = 100, SubPrices = new[] { new SubPriceRow { Branch = "TRAFİK", Product = "TRAFİK", Price = 15948.73m, Commission = 1475.30m, PolicyNo = "30100025..." } } },
                new CompanyRow { Name = "HEPIYI", Status = "green", Price = 15351.27m, Percentage = 100, SubPrices = new[] { new SubPriceRow { Branch = "TRAFİK", Product = "TRAFİK", Price = 15351.27m, Commission = 1475.30m, PolicyNo = "30100025..." } } },
                new CompanyRow { Name = "HEDVA", Status = "none", Percentage = 0 },
                new CompanyRow { Name = "QUICK_PORT", Status = "green", Price = 15940.57m, Percentage = 100 },
                new CompanyRow { Name = "RAY", Status = "green", Price = 16198.84m, Percentage = 100 },
                new CompanyRow { Name = "SOMPO", Status = "red", Price = 26368.99m, Percentage = 100, Warning = "Ek Teminatlı Trafik Teklifidir!", SubPrices = new[] { new SubPriceRow { Branch = "TRAFİK", Product = "TRAFİK", Price = 26368.99m, Commission = 2429.37m, PolicyNo = "31100053...", Description = "Ek Teminatlı Trafik Teklifidir!" } } },
                new CompanyRow { Name = "TURKNIPPON", Status = "none", Percentage = 0, UnavailableBranches = new[] { "TRAFİK" } },
                new CompanyRow { Name = "ZURICH", Status = "none", Percentage = 0, UnavailableBranches = new[] { "TRAFİK" } }
            };
        }

        private class SubPriceRow
        {
            public string Branch { get; set; }
            public string Product { get; set; }
            public decimal Price { get; set; }
            public decimal Commission { get; set; }
            public string PolicyNo { get; set; }
            public string Description { get; set; }
        }

        private class CompanyRow
        {
            public string Name { get; set; }
            public string Status { get; set; }
            public decimal? Price { get; set; }
            public int? Percentage { get; set; }
            public string Warning { get; set; }
            public string[] UnavailableBranches { get; set; }
            public SubPriceRow[] SubPrices { get; set; }
        }

        private class RowTag
        {
            public CompanyRow Company { get; set; }
            public SubPriceRow SubPrice { get; set; }
        }

        private void BuildTopHeader()
        {
            var topHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                MinimumSize = new Size(0, 52),
                BackColor = Color.White,
                Padding = new Padding(8, 4, 8, 4)
            };

            var lblTitle = new Label
            {
                Text = "Vega Hızlı Teklif Sistemi",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = VegaGreen,
                AutoSize = true,
                Location = new Point(12, 8)
            };
            var lblAcente = new Label
            {
                Text = "Ana Acente: SAFAOĞULLARI SİGORTA - 95666",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(12, 30)
            };

            var navFlow = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Location = new Point(320, 6),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                WrapContents = true,
                MaximumSize = new Size(0, 80),
                Padding = new Padding(0)
            };
            string[] navItems = { "ANA EKRAN", "ŞİRKETLER (ROBOT)", "TEKLİFLER", "POLİÇELERİM", "Raporlar", "Destek Talepleri", "Ajanda / Yenileme", "Duyurular" };
            foreach (var text in navItems)
            {
                var btn = CreateNavButton(text, text == "ANA EKRAN");
                if (text == "TEKLİFLER") btn.Click += (s, e) => TekliflerRequested?.Invoke(this, EventArgs.Empty);
                else if (text == "POLİÇELERİM") btn.Click += (s, e) => PolicelerimRequested?.Invoke(this, EventArgs.Empty);
                else if (text == "ŞİRKETLER (ROBOT)") btn.Click += (s, e) => SirketlerRobotRequested?.Invoke(this, EventArgs.Empty);
                else if (text == "Raporlar") btn.Click += (s, e) => RaporlarRequested?.Invoke(this, EventArgs.Empty);
                else if (text == "Destek Talepleri") btn.Click += (s, e) => DestekTalepleriRequested?.Invoke(this, EventArgs.Empty);
                else if (text == "Ajanda / Yenileme") btn.Click += (s, e) => AjandaYenilemeRequested?.Invoke(this, EventArgs.Empty);
                else if (text == "Duyurular") btn.Click += (s, e) => DuyurularRequested?.Invoke(this, EventArgs.Empty);
                navFlow.Controls.Add(btn);
            }

            var rightPanel = new Panel { Dock = DockStyle.Right, Width = 340, Padding = new Padding(8), MinimumSize = new Size(260, 0) };
            var rightTable = new TableLayoutPanel
            {
                Dock = DockStyle.Right,
                ColumnCount = 1,
                RowCount = 2,
                AutoSize = true,
                Padding = new Padding(0)
            };
            rightTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            rightTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            rightTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var btnGiris = new Button
            {
                Text = "→ Giriş Yap",
                BackColor = VegaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 32),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnGiris.FlatAppearance.BorderSize = 0;
            var rightRow2 = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Margin = new Padding(0, 6, 0, 0),
                Padding = new Padding(0)
            };
            var btnCanliDestek = new Button
            {
                Text = "CANLI DESTEK",
                BackColor = VegaOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 32),
                Cursor = Cursors.Hand
            };
            btnCanliDestek.FlatAppearance.BorderSize = 0;
            var btnCanliUretim = new Button
            {
                Text = "CANLI ÜRETİM",
                BackColor = VegaRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 32),
                Cursor = Cursors.Hand
            };
            btnCanliUretim.FlatAppearance.BorderSize = 0;
            btnCanliDestek.Click += (s, e) => CanliDestekRequested?.Invoke(this, EventArgs.Empty);
            btnCanliUretim.Click += (s, e) => CanliUretimRequested?.Invoke(this, EventArgs.Empty);
            rightRow2.Controls.Add(btnCanliUretim);
            rightRow2.Controls.Add(btnCanliDestek);
            rightTable.Controls.Add(btnGiris, 0, 0);
            rightTable.Controls.Add(rightRow2, 0, 1);
            rightPanel.Controls.Add(rightTable);
            topHeader.Controls.Add(lblTitle);
            topHeader.Controls.Add(lblAcente);
            topHeader.Controls.Add(navFlow);
            topHeader.Controls.Add(rightPanel);
            Controls.Add(topHeader);
        }

        private Button CreateNavButton(string text, bool active)
        {
            var btn = new Button
            {
                Text = text,
                BackColor = active ? VegaGreen : VegaGreenLight,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(text.Length > 12 ? 130 : 110, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 8, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void BuildToolbar()
        {
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 44,
                MinimumSize = new Size(0, 44),
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(8, 6, 8, 6),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true
            };
            var chkWebServis = new CheckBox { Text = "Web Servis Teklifi Çalış", AutoSize = true, Margin = new Padding(0, 8, 12, 0) };
            var chkKasko = new CheckBox { Text = "Kasko Özel Fiyat", AutoSize = true, Margin = new Padding(0, 8, 12, 0) };
            var chkKisaVadeli = new CheckBox { Text = "Kısa Vadeli Poliçe Çalış", AutoSize = true, Margin = new Padding(0, 8, 12, 0) };
            _formFields["KisaVadeli"] = chkKisaVadeli;
            _cmbTarayici = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120, Margin = new Padding(0, 6, 12, 0) };
            _cmbTarayici.Items.Add("Tarayıcı 1");
            _cmbTarayici.Items.Add("Tarayıcı 2");
            _cmbTarayici.SelectedIndex = 0;

            _btnDigierFiyatlar = new Button
            {
                Text = "Diğer Fiyatları Göster",
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 26),
                Margin = new Padding(0, 6, 8, 0),
                Cursor = Cursors.Hand
            };
            _btnDigierFiyatlar.Click += (s, e) =>
            {
                _allExpanded = !_allExpanded;
                _btnDigierFiyatlar.Text = _allExpanded ? "Diğer Fiyatları Gizle" : "Diğer Fiyatları Göster";
                RefreshCompanyGrid();
                RefreshSelectedCompanyPanel();
            };

            var btnSirketEkle = new Button
            {
                Text = "+",
                BackColor = VegaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(28, 26),
                Margin = new Padding(0, 6, 4, 0),
                Cursor = Cursors.Hand
            };
            btnSirketEkle.FlatAppearance.BorderSize = 0;
            btnSirketEkle.Click += (s, e) => SirketEkleRequested?.Invoke(this, EventArgs.Empty);

            var btnAyarlar = new Button
            {
                Text = "⚙",
                FlatStyle = FlatStyle.Flat,
                Size = new Size(28, 26),
                Margin = new Padding(0, 6, 0, 0),
                Cursor = Cursors.Hand
            };
            btnAyarlar.Click += (s, e) => AdminPanelRequested?.Invoke(this, EventArgs.Empty);

            toolbar.Controls.AddRange(new Control[] { chkWebServis, chkKasko, chkKisaVadeli, _cmbTarayici, _btnDigierFiyatlar, btnSirketEkle, btnAyarlar });
            Controls.Add(toolbar);
        }

        private void BuildMainContent()
        {
            _mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Panel1MinSize = 50,
                Panel2MinSize = 50,
                BackColor = Color.White
            };
            _mainSplit.Resize += MainSplit_Resize;

            _mainSplit.Panel1.SuspendLayout();
            var leftToolbar = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Color.FromArgb(245, 245, 245) };
            var btnTrafik = new Button
            {
                Text = "TRAFİK",
                BackColor = VegaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 28),
                Location = new Point(8, 4)
            };
            btnTrafik.FlatAppearance.BorderSize = 0;
            var cmbTip = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 100, Location = new Point(96, 6) };
            cmbTip.Items.Add("Tümü");
            cmbTip.SelectedIndex = 0;
            leftToolbar.Controls.Add(btnTrafik);
            leftToolbar.Controls.Add(cmbTip);

            _companyGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 34,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(6, 4, 6, 4),
                    SelectionBackColor = Color.FromArgb(230, 245, 230)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(6, 6, 6, 6)
                }
            };
            _companyGrid.RowTemplate.Height = 32;
            _companyGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            _companyGrid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Secim", HeaderText = "", Width = 32 });
            _companyGrid.Columns.Add("Sirket", "Şirket");
            _companyGrid.Columns.Add("Trafik", "Trafik");
            _companyGrid.Columns.Add("Sbm", "Sbm");
            _companyGrid.Columns.Add("Kasko", "Kasko");
            _companyGrid.Columns.Add("TssAy", "TSS Ay.");
            _companyGrid.Columns.Add("TssYat", "TSS Yat.");
            _companyGrid.Columns.Add("Konut", "Konut");
            _companyGrid.Columns.Add("Dask", "Dask");
            _companyGrid.Columns.Add("Imm", "IMM");
            _companyGrid.Columns.Add("Yuzde", "%");
            var colUyari = new DataGridViewTextBoxColumn { Name = "Uyari", HeaderText = "Uyarı", MinimumWidth = 220 };
            _companyGrid.Columns.Add(colUyari);
            _companyGrid.Columns["Sirket"].FillWeight = 120;
            _companyGrid.Columns["Uyari"].FillWeight = 180;
            if (_companyGrid.Columns["Secim"] != null)
                _companyGrid.Columns["Secim"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _companyGrid.EnableHeadersVisualStyles = false;
            _companyGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            _companyGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = _companyGrid.ColumnHeadersDefaultCellStyle.BackColor;
            _companyGrid.CellFormatting += CompanyGrid_CellFormatting;
            _companyGrid.CellDoubleClick += CompanyGrid_CellDoubleClick;

            _mainSplit.Panel1.Controls.Add(_companyGrid);
            _mainSplit.Panel1.Controls.Add(leftToolbar);
            _mainSplit.Panel1.ResumeLayout(true);

            _mainSplit.Panel2.SuspendLayout();
            BuildRightPanel(_mainSplit.Panel2);
            _mainSplit.Panel2.ResumeLayout(true);

            Controls.Add(_mainSplit);
        }

        private void MainSplit_Resize(object sender, EventArgs e)
        {
            if (_splitterInitialized) return;
            const int panel1Min = 400;
            const int panel2Min = 320;
            int w = _mainSplit.Width;
            if (w < panel1Min + panel2Min) return;
            _splitterInitialized = true;
            _mainSplit.Resize -= MainSplit_Resize;
            _mainSplit.Panel1MinSize = panel1Min;
            _mainSplit.Panel2MinSize = panel2Min;
            int splitDist = (int)(w * 0.65);
            _mainSplit.SplitterDistance = Math.Max(panel1Min, Math.Min(splitDist, w - panel2Min));
        }

        private void BuildRightPanel(Panel rightPanel)
        {
            rightPanel.BackColor = Color.FromArgb(250, 250, 250);

            var topBar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, BackColor = Color.White, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
            var lblTeklifId = new Label { Text = "Teklif ID: 37111587", AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(0, 10, 12, 0) };
            _cmbPolicyType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 110, Height = 26, Margin = new Padding(0, 6, 8, 0) };
            _cmbPolicyType.Items.AddRange(new object[] { "TRAFİK", "KASKO", "DASK", "TSS", "KONUT", "İMM" });
            _cmbPolicyType.SelectedIndex = 0;
            _cmbPolicyType.SelectedIndexChanged += (s, e) =>
            {
                _companyGrid?.Invalidate();
                RefreshSelectedCompanyPanel();
            };
            var chkDigerFiyat = new CheckBox { Text = "Diğer Fiyatları Göster", AutoSize = true, Margin = new Padding(0, 10, 12, 0) };
            var chkAltFiyat = new CheckBox { Text = "Alt Fiyatları Göster", AutoSize = true, Margin = new Padding(0, 10, 12, 0) };
            var btnPdf = new Button { Text = "PDF Aktar", FlatStyle = FlatStyle.Flat, Size = new Size(80, 26), Margin = new Padding(0, 6, 0, 0) };
            btnPdf.Click += (s, e) => PdfExportRequested?.Invoke(this, EventArgs.Empty);
            topBar.Controls.AddRange(new Control[] { lblTeklifId, _cmbPolicyType, chkDigerFiyat, chkAltFiyat, btnPdf });
            rightPanel.Controls.Add(topBar);

            var rightContent = new Panel { Dock = DockStyle.Fill };
            var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(8) };

            _rightActionPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 120,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(4)
            };
            int ay = 8;
            var btnSorguBaslat = new Button
            {
                Text = "Sorguyu Başlat",
                BackColor = VegaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(112, 40),
                Location = new Point(4, ay),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F)
            };
            btnSorguBaslat.FlatAppearance.BorderSize = 0;
            ay += 46;
            _btnDuraklat = new Button { Text = "Duraklat / Devam Et", FlatStyle = FlatStyle.Flat, Size = new Size(112, 32), Location = new Point(4, ay), Font = new Font("Segoe UI", 8F), Enabled = false };
            ay += 38;
            var btnYeniSorgu = new Button { Text = "Yeni Sorgu Kaydet", FlatStyle = FlatStyle.Flat, Size = new Size(112, 36), Location = new Point(4, ay), Font = new Font("Segoe UI", 8F) };
            ay += 42;
            var btnSbm = new Button { Text = "SBM Sorgula", FlatStyle = FlatStyle.Flat, Size = new Size(112, 36), Location = new Point(4, ay), Font = new Font("Segoe UI", 8F) };
            ay += 42;
            var btnKuyruk = new Button { Text = "Kuyruk Sorgusu", FlatStyle = FlatStyle.Flat, Size = new Size(112, 36), Location = new Point(4, ay), Font = new Font("Segoe UI", 8F) };
            btnSorguBaslat.Click += BtnSorguBaslat_Click;
            _btnDuraklat.Click += BtnDuraklat_Click;
            btnYeniSorgu.Click += BtnYeniSorgu_Click;
            btnSbm.Click += (s, e) => SbmSorgusuRequested?.Invoke(this, EventArgs.Empty);
            btnKuyruk.Click += (s, e) => KuyrukSorgusuRequested?.Invoke(this, EventArgs.Empty);
            _rightActionPanel.Controls.AddRange(new Control[] { btnSorguBaslat, _btnDuraklat, btnYeniSorgu, btnSbm, btnKuyruk });
            rightContent.Controls.Add(_rightActionPanel);

            int y = 8;
            _selectedCompanyPanel = new Panel { Location = new Point(8, y), Size = new Size(280, 70), MinimumSize = new Size(200, 70), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, BackColor = Color.FromArgb(230, 245, 230) };
            scrollPanel.Controls.Add(_selectedCompanyPanel);
            y += 78;

            var grpMusteri = new GroupBox
            {
                Text = "MÜŞTERİ BİLGİLERİ",
                Location = new Point(8, y),
                Size = new Size(320, 220),
                MinimumSize = new Size(260, 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            int fy = 24;
            AddFormRow(grpMusteri, "Plaka", "06BFT02", ref fy, true, "Varsa", "Getir", fieldKey: "Plaka");
            AddFormRow(grpMusteri, "TC / Vergi", "13627046986", ref fy, fieldKey: "TcVergi");
            AddFormRow(grpMusteri, "Belge Seri", "GL", ref fy, secondText: "152109", fieldKey: "BelgeSeri", fieldKey2: "BelgeNo");
            AddFormRow(grpMusteri, "Doğum T.", "01.01.1987", ref fy, fieldKey: "DogumTarihi");
            AddFormRow(grpMusteri, "DT Şirketi", "", ref fy, isCombo: true);
            AddFormRow(grpMusteri, "Müşteri", "SELİM TANRISEVEN", ref fy, fieldKey: "Musteri");
            AddFormRowWithItems(grpMusteri, "Meslek", ref fy, Professions.List, "Meslek");
            AddFormRow(grpMusteri, "Telefon", "", ref fy, checkText: "Kullan");
            AddFormRowIlIlce(grpMusteri, ref fy);
            AddFormRow(grpMusteri, "Açıklama", "", ref fy, multiLine: true, fieldKey: "Aciklama");
            grpMusteri.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            scrollPanel.Controls.Add(grpMusteri);
            y += 228;

            _aracBilgileriTabs = new TabControl
            {
                Location = new Point(8, y),
                Size = new Size(320, 200),
                MinimumSize = new Size(260, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            var tabArac = new TabPage("Araç Bilgileri");
            int ty = 8;
            AddFormRowWithItems(tabArac, "Kullanım Tarzı", ref ty, KullanimTarziOptions.List, "KullanimTarzi");
            AddFormRowWithItems(tabArac, "Kullanım Amacı", ref ty, new[] { "Seçiniz", "ÖZEL KULLANIM", "TİCARİ KULLANIM", "RESMİ KULLANIM" });
            AddFormRowMarkaTip(tabArac, ref ty, "Marka", "Tip");
            AddFormRow(tabArac, "Model Yılı", "2000", ref ty, fieldKey: "ModelYili");
            AddFormRow(tabArac, "Kasko Değeri", "", ref ty);
            AddFormRow(tabArac, "Tescil Tarihi", "19.01.2012", ref ty);
            AddFormRow(tabArac, "Koltuk Sayısı", "1", ref ty);
            AddFormRow(tabArac, "Motor No", "SE706025", ref ty);
            AddFormRow(tabArac, "Şasi No", "", ref ty);
            var chkLpg = new CheckBox { Text = "LPG", AutoSize = true, Location = new Point(120, ty) };
            tabArac.Controls.Add(chkLpg);
            _aracBilgileriTabs.TabPages.Add(tabArac);
            var tabTrafik = new TabPage("Trafik Pol. Bilgisi");
            int ty2 = 8;
            AddFormRowWithItems(tabTrafik, "Sigorta Şirketi", ref ty2, SigortaSirketleri.List, "TrafikSigortaSirketi");
            AddFormRow(tabTrafik, "Acente Kodu", "", ref ty2, fieldKey: "TrafikAcenteKodu");
            AddFormRow(tabTrafik, "Poliçe No", "", ref ty2, fieldKey: "TrafikPoliceNo");
            AddFormRow(tabTrafik, "Başlangıç T.", "", ref ty2, fieldKey: "TrafikBaslangicTarihi");
            AddFormRow(tabTrafik, "Bitiş T.", "", ref ty2, fieldKey: "TrafikBitisTarihi");
            AddFormRowKalanGunFromFields(tabTrafik, ref ty2, "TrafikBaslangicTarihi", "TrafikBitisTarihi", "TrafikKalanGun");
            AddFormRow(tabTrafik, "Kademe", "", ref ty2);
            AddFormRow(tabTrafik, "Hasarsızlık %", "", ref ty2);
            AddFormRow(tabTrafik, "Yenileme No", "", ref ty2);
            _aracBilgileriTabs.TabPages.Add(tabTrafik);
            var tabKasko = new TabPage("Kasko Pol. Bilgisi");
            int ty3 = 8;
            AddFormRowWithItems(tabKasko, "Sigorta Şirketi", ref ty3, SigortaSirketleri.List, "KaskoSigortaSirketi");
            AddFormRow(tabKasko, "Acente Kodu", "", ref ty3, fieldKey: "KaskoAcenteKodu");
            AddFormRow(tabKasko, "Poliçe No", "", ref ty3, fieldKey: "KaskoPoliceNo");
            AddFormRow(tabKasko, "Başlangıç T.", "", ref ty3, fieldKey: "KaskoBaslangicTarihi");
            AddFormRow(tabKasko, "Bitiş T.", "", ref ty3, fieldKey: "KaskoBitisTarihi");
            AddFormRowKalanGunFromFields(tabKasko, ref ty3, "KaskoBaslangicTarihi", "KaskoBitisTarihi", "KaskoKalanGun");
            AddFormRow(tabKasko, "Kademe", "", ref ty3, fieldKey: "KaskoKademe");
            AddFormRow(tabKasko, "Hasarsızlık %", "", ref ty3, fieldKey: "KaskoHasarsizlik");
            AddFormRow(tabKasko, "Yenileme No", "", ref ty3, fieldKey: "KaskoYenilemeNo");
            _aracBilgileriTabs.TabPages.Add(tabKasko);
            scrollPanel.Controls.Add(_aracBilgileriTabs);
            y += 200;
            const int SectionGap = 12;
            y += SectionGap;

            var bottomBtns = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Location = new Point(8, y),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Padding = new Padding(0),
                Margin = new Padding(0, 0, 0, SectionGap)
            };
            var btnWebcam = new Button { Text = "Webcam QR Oku", FlatStyle = FlatStyle.Flat, Size = new Size(110, 28) };
            btnWebcam.Click += (s, e) => WebcamQRRequested?.Invoke(this, EventArgs.Empty);
            var btnPdfYukle = new Button { Text = "PDF Yükle", FlatStyle = FlatStyle.Flat, Size = new Size(80, 28) };
            btnPdfYukle.Click += (s, e) => PdfUploadRequested?.Invoke(this, EventArgs.Empty);
            bottomBtns.Controls.Add(btnWebcam);
            bottomBtns.Controls.Add(btnPdfYukle);
            bottomBtns.Controls.Add(new Button { Text = "Temizle", FlatStyle = FlatStyle.Flat, Size = new Size(70, 28) });
            scrollPanel.Controls.Add(bottomBtns);

            rightContent.Controls.Add(scrollPanel);
            rightPanel.Controls.Add(rightContent);
        }

        private const int FormLabelWidth = 110;
        private const int FormInputLeft = 124;
        private const int FormInputGap = 6;
        private const int FormRowHeight = 28;
        private const int FormRowHeightMulti = 44;

        private void AddFormRow(Control parent, string labelText, string value, ref int y, bool withCheck = false, string checkText = "", string secondCheckText = "", string secondText = "", bool isCombo = false, bool secondCombo = false, bool multiLine = false, string fieldKey = null, string fieldKey2 = null)
        {
            int rowTop = y;
            int inputTop = rowTop - 2;
            var lbl = new Label { Text = labelText + ":", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 2), AutoEllipsis = true };
            parent.Controls.Add(lbl);
            Control input;
            int firstInputWidth = string.IsNullOrEmpty(secondText) ? 180 : 72;
            if (isCombo)
            {
                input = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = inputTop, Width = 180 };
                ((ComboBox)input).Items.Add("Seçiniz");
                ((ComboBox)input).SelectedIndex = 0;
            }
            else if (multiLine)
            {
                input = new TextBox { Multiline = true, Left = FormInputLeft, Top = inputTop, Width = 240, Height = 36 };
            }
            else
            {
                input = new TextBox { Left = FormInputLeft, Top = inputTop, Width = firstInputWidth, Text = value };
            }
            parent.Controls.Add(input);
            if (!string.IsNullOrEmpty(fieldKey)) _formFields[fieldKey] = input;
            int nx = FormInputLeft + 184;
            Control txt2 = null;
            if (withCheck)
            {
                var chk1 = new CheckBox { Text = checkText, AutoSize = true, Location = new Point(nx, rowTop + 2) };
                parent.Controls.Add(chk1);
                nx += 60;
                var chk2 = new CheckBox { Text = secondCheckText, AutoSize = true, Location = new Point(nx, rowTop + 2) };
                parent.Controls.Add(chk2);
            }
            if (!string.IsNullOrEmpty(secondText))
            {
                int secondLeft = FormInputLeft + firstInputWidth + FormInputGap;
                txt2 = new TextBox { Left = secondLeft, Top = inputTop, Width = 78, Text = secondText };
                parent.Controls.Add(txt2);
                if (!string.IsNullOrEmpty(fieldKey2)) _formFields[fieldKey2] = txt2;
            }
            if (secondCombo)
            {
                var cmb2 = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft + 184 + FormInputGap, Top = inputTop, Width = 100 };
                cmb2.Items.Add("Seçiniz");
                cmb2.SelectedIndex = 0;
                parent.Controls.Add(cmb2);
            }
            y += multiLine ? FormRowHeightMulti : FormRowHeight;
        }

        private void AddFormRowWithItems(Control parent, string labelText, ref int y, string[] items, string fieldKey = null)
        {
            int rowTop = y;
            var lbl = new Label { Text = labelText + ":", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 2), AutoEllipsis = true };
            parent.Controls.Add(lbl);
            var cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = rowTop - 2, Width = 180 };
            if (items != null && items.Length > 0)
            {
                foreach (var item in items)
                {
                    cmb.Items.Add(item);
                }
                cmb.SelectedIndex = 0;
            }
            parent.Controls.Add(cmb);
            if (!string.IsNullOrEmpty(fieldKey)) _formFields[fieldKey] = cmb;
            y += FormRowHeight;
        }

        private void AddFormRowKalanGunFromFields(Control parent, ref int y, string startKey, string endKey, string resultKey)
        {
            int rowTop = y;
            var lblKalan = new Label { Text = "Kalan Gün:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 2), AutoEllipsis = true };
            parent.Controls.Add(lblKalan);
            var txtKalan = new TextBox { Left = FormInputLeft, Top = rowTop - 2, Width = 180, Text = "0", ReadOnly = true };
            parent.Controls.Add(txtKalan);
            if (!string.IsNullOrEmpty(resultKey)) _formFields[resultKey] = txtKalan;

            EventHandler updateKalan = (s, e) =>
            {
                var txtStart = _formFields.ContainsKey(startKey) ? _formFields[startKey] as TextBox : null;
                var txtEnd = _formFields.ContainsKey(endKey) ? _formFields[endKey] as TextBox : null;
                if (txtStart == null || txtEnd == null || txtKalan == null) return;
                DateTime baslangic, bitis;
                if (DateTime.TryParse(txtStart.Text, out baslangic) && DateTime.TryParse(txtEnd.Text, out bitis))
                {
                    var kalan = (bitis - DateTime.Today).Days;
                    txtKalan.Text = kalan >= 0 ? kalan.ToString() : "0";
                }
                else
                    txtKalan.Text = "0";
            };

            if (_formFields.ContainsKey(startKey))
            {
                var txtStart = _formFields[startKey] as TextBox;
                if (txtStart != null) txtStart.TextChanged += updateKalan;
            }
            if (_formFields.ContainsKey(endKey))
            {
                var txtEnd = _formFields[endKey] as TextBox;
                if (txtEnd != null) txtEnd.TextChanged += updateKalan;
            }
            updateKalan(null, null);
            y += FormRowHeight;
        }

        private void AddFormRowWithKalanGun(Control parent, ref int y)
        {
            int rowTop = y;
            var lblBaslangic = new Label { Text = "Başlangıç T.:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 2), AutoEllipsis = true };
            parent.Controls.Add(lblBaslangic);
            var txtBaslangic = new TextBox { Left = FormInputLeft, Top = rowTop - 2, Width = 180 };
            txtBaslangic.TextChanged += (s, e) => HesaplaKalanGun(parent);
            parent.Controls.Add(txtBaslangic);
            y += FormRowHeight;

            rowTop = y;
            var lblBitis = new Label { Text = "Bitiş T.:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 2), AutoEllipsis = true };
            parent.Controls.Add(lblBitis);
            var txtBitis = new TextBox { Left = FormInputLeft, Top = rowTop - 2, Width = 180 };
            txtBitis.TextChanged += (s, e) => HesaplaKalanGun(parent);
            parent.Controls.Add(txtBitis);
            y += FormRowHeight;

            rowTop = y;
            var lblKalan = new Label { Text = "Kalan Gün:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 2), AutoEllipsis = true };
            parent.Controls.Add(lblKalan);
            var txtKalan = new TextBox { Left = FormInputLeft, Top = rowTop - 2, Width = 180, Text = "0", ReadOnly = true };
            parent.Controls.Add(txtKalan);
            parent.Tag = new object[] { txtBaslangic, txtBitis, txtKalan };
            y += FormRowHeight;
        }

        private void HesaplaKalanGun(Control parent)
        {
            if (parent.Tag == null) return;
            var arr = parent.Tag as object[];
            if (arr == null || arr.Length < 3) return;
            var txtBaslangic = arr[0] as TextBox;
            var txtBitis = arr[1] as TextBox;
            var txtKalan = arr[2] as TextBox;
            if (txtBaslangic == null || txtBitis == null || txtKalan == null) return;

            DateTime baslangic, bitis;
            if (DateTime.TryParse(txtBaslangic.Text, out baslangic) && DateTime.TryParse(txtBitis.Text, out bitis))
            {
                var kalan = (bitis - DateTime.Today).Days;
                txtKalan.Text = kalan >= 0 ? kalan.ToString() : "0";
            }
            else
            {
                txtKalan.Text = "0";
            }
        }

        private void AddFormRowMarkaTip(Control parent, ref int y, string fieldKeyMarka = null, string fieldKeyTip = null)
        {
            int rowTop = y;
            var lblMarka = new Label { Text = "Marka:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 2), AutoEllipsis = true };
            parent.Controls.Add(lblMarka);
            var cmbMarka = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = rowTop - 2, Width = 180 };
            cmbMarka.Items.AddRange(VehicleBrandsAndTypes.GetBrandDisplays());
            if (cmbMarka.Items.Count > 0) cmbMarka.SelectedIndex = 0;
            parent.Controls.Add(cmbMarka);
            if (!string.IsNullOrEmpty(fieldKeyMarka)) _formFields[fieldKeyMarka] = cmbMarka;
            y += FormRowHeight;

            rowTop = y;
            var lblTip = new Label { Text = "Tip:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 2), AutoEllipsis = true };
            parent.Controls.Add(lblTip);
            var cmbTip = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = rowTop - 2, Width = 180 };
            var brandDisplays = VehicleBrandsAndTypes.GetBrandDisplays();
            if (brandDisplays.Length > 0)
            {
                foreach (var t in VehicleBrandsAndTypes.GetTypesByBrandDisplay(brandDisplays[0]))
                    cmbTip.Items.Add(t);
                if (cmbTip.Items.Count > 0) cmbTip.SelectedIndex = 0;
            }
            parent.Controls.Add(cmbTip);
            if (!string.IsNullOrEmpty(fieldKeyTip)) _formFields[fieldKeyTip] = cmbTip;

            cmbMarka.SelectedIndexChanged += (s, e) =>
            {
                var brand = cmbMarka.SelectedItem as string;
                if (string.IsNullOrEmpty(brand)) return;
                cmbTip.Items.Clear();
                foreach (var t in VehicleBrandsAndTypes.GetTypesByBrandDisplay(brand))
                    cmbTip.Items.Add(t);
                if (cmbTip.Items.Count > 0) cmbTip.SelectedIndex = 0;
            };
            y += FormRowHeight;
        }

        private void AddFormRowIlIlce(Control parent, ref int y)
        {
            int rowTop = y;
            var lbl = new Label { Text = "İl / İlçe:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 2), AutoEllipsis = true };
            parent.Controls.Add(lbl);
            _cmbIl = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = rowTop - 2, Width = 120 };
            _cmbIl.Items.AddRange(TurkeyLocations.GetCityNames());
            if (_cmbIl.Items.Count > 0) _cmbIl.SelectedIndex = 0;
            _cmbIl.SelectedIndexChanged += (s, e) =>
            {
                var city = _cmbIl.SelectedItem as string;
                if (string.IsNullOrEmpty(city)) return;
                _cmbIlce.Items.Clear();
                var districts = TurkeyLocations.GetDistrictsByCity(city);
                foreach (var d in districts) _cmbIlce.Items.Add(d);
                if (_cmbIlce.Items.Count > 0) _cmbIlce.SelectedIndex = 0;
            };
            parent.Controls.Add(_cmbIl);
            _cmbIlce = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft + 120 + FormInputGap, Top = rowTop - 2, Width = 100 };
            if (_cmbIl.SelectedItem != null)
            {
                var districts = TurkeyLocations.GetDistrictsByCity(_cmbIl.SelectedItem.ToString());
                foreach (var d in districts) _cmbIlce.Items.Add(d);
                if (_cmbIlce.Items.Count > 0) _cmbIlce.SelectedIndex = 0;
            }
            parent.Controls.Add(_cmbIlce);
            y += FormRowHeight;
        }

        private void BuildStatusBar()
        {
            const int StatusItemGap = 24;
            var statusBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(12, 6, 12, 6)
            };
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(0) };
            string[] items = { "Tramer", "Egm", "Sanal T.", "Dışa Al", "Ram", "Eğitim (YENİ)" };
            Color[] colors = { Color.LimeGreen, Color.LimeGreen, Color.Orange, Color.Orange, Color.LimeGreen, Color.Red };
            for (int i = 0; i < items.Length; i++)
            {
                var p = new Panel { Size = new Size(10, 10), BackColor = colors[i], Margin = new Padding(0, 4, 6, 0) };
                var lbl = new Label { Text = items[i], AutoSize = true, Margin = new Padding(0, 2, StatusItemGap, 0) };
                flow.Controls.Add(p);
                flow.Controls.Add(lbl);
            }
            statusBar.Controls.Add(flow);
            Controls.Add(statusBar);
        }
    }
}
