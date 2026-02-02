using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using VegaAsis.Windows.Data;
using VegaAsis.Windows.Forms;
using VegaAsis.Windows.Models;
using VegaAsis.Windows.Resources;

namespace VegaAsis.Windows.UserControls
{
    public class IndexViewControl : UserControl
    {
        private static readonly Color ThemeRed = Color.FromArgb(211, 47, 47);
        private static readonly Color ThemeRedLight = Color.FromArgb(239, 83, 80);
        private static readonly Color VegaGreen = Color.FromArgb(46, 125, 50);
        private static readonly Color VegaRed = Color.FromArgb(198, 40, 40);
        private static readonly Color VegaOrange = Color.FromArgb(239, 108, 0);

        private DataGridView _companyGrid;
        private Panel _rightActionPanel;
        private TabControl _aracBilgileriTabs;
        private SplitContainer _mainSplit;
        private bool _splitterInitialized;
        private bool _allExpanded;
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
                var idx = _companyGrid.Rows.Add(false, false, null, c.Name, trafikStr, "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", yuzdeStr, null, null, null, null, yuzdeStr);
                var row = _companyGrid.Rows[idx];
                row.Cells["Secim"].Value = c.Price.HasValue;
                row.Tag = new RowTag { Company = c, SubPrice = null };

                if (_allExpanded && c.SubPrices != null && c.SubPrices.Length > 0)
                {
                    foreach (var sp in c.SubPrices)
                    {
                        var spTrafik = sp.Price.ToString("N2", new CultureInfo("tr-TR"));
                        var subIdx = _companyGrid.Rows.Add(false, false, null, "  \u2192 " + (sp.Branch ?? sp.Product), spTrafik, "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "", null, null, null, null, "");
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

        private void CompanyGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = _companyGrid.Rows[e.RowIndex];
            if (!row.Selected) return;
            var rect = row.HeaderCell.ContentBounds;
            if (rect.Width <= 0 || rect.Height <= 0) return;
            e.Graphics.FillRectangle(new SolidBrush(_companyGrid.RowHeadersDefaultCellStyle.BackColor), rect);
            using (var f = new Font("Segoe UI", 9F))
            {
                var arrow = "\u25B6";
                var sz = e.Graphics.MeasureString(arrow, f);
                e.Graphics.DrawString(arrow, f, Brushes.Black, rect.Left + (rect.Width - sz.Width) / 2f, rect.Top + (rect.Height - sz.Height) / 2f);
            }
            e.Handled = true;
        }

        private void CompanyGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var row = _companyGrid.Rows[e.RowIndex];
            var tag = row.Tag as RowTag;
            var colName = _companyGrid.Columns[e.ColumnIndex].Name;

            if (colName == "Logo" && tag != null && tag.SubPrice == null)
            {
                e.Handled = true;
                e.Graphics.FillRectangle(new SolidBrush(e.CellStyle.BackColor), e.CellBounds);
                var company = tag.Company;
                if (company != null)
                {
                    int size = 18;
                    int cx = e.CellBounds.Left + (e.CellBounds.Width - size) / 2;
                    int cy = e.CellBounds.Top + (e.CellBounds.Height - size) / 2;
                    
                    var logo = CompanyLogoData.CreateLogo(company.Name, size);
                    e.Graphics.DrawImage(logo, cx, cy, size, size);
                    logo.Dispose();
                }
                return;
            }

            if ((colName == "Icon1" || colName == "Icon2" || colName == "Icon3" || colName == "Icon4") && tag != null && tag.SubPrice == null)
            {
                e.Handled = true;
                e.Graphics.FillRectangle(new SolidBrush(e.CellStyle.BackColor), e.CellBounds);
                var company = tag.Company;
                if (company == null) return;
                var rect = e.CellBounds;
                int iconY = rect.Top + (rect.Height - 14) / 2;
                Color clr = Color.Gray;
                if (company.Status == "green") clr = Color.FromArgb(76, 175, 80);
                else if (company.Status == "red") clr = Color.FromArgb(244, 67, 54);
                string sym = null;
                if (colName == "Icon1") sym = "\u263A";
                else if (colName == "Icon2") { sym = "\u2699"; if (company.Status == "green") clr = Color.FromArgb(33, 150, 243); }
                else if (colName == "Icon3") sym = "\u270E";
                else if (colName == "Icon4") { sym = "\u20BA"; clr = company.Status == "green" ? Color.FromArgb(255, 193, 7) : Color.Gray; }
                if (sym != null)
                {
                    using (var f = new Font("Segoe UI Symbol", 10F))
                    using (var b = new SolidBrush(clr))
                        e.Graphics.DrawString(sym, f, b, rect.Left + 4, iconY);
                }
            }
        }

        private void CompanyGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var colName = _companyGrid.Columns[e.ColumnIndex].Name;
            if (colName != "Icon2") return;
            var row = _companyGrid.Rows[e.RowIndex];
            var tag = row.Tag as RowTag;
            if (tag == null || tag.SubPrice != null) return;
            AdminPanelRequested?.Invoke(this, EventArgs.Empty);
        }

        public void AddCompany(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName)) return;
            var name = companyName.Trim();
            if (_companyData == null) _companyData = new List<CompanyRow>();
            if (_companyData.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))) return;

            var newCompany = new CompanyRow { Name = name, Status = "none", Percentage = 0 };
            _companyData.Add(newCompany);
            var idx = _companyGrid.Rows.Add(false, false, null, name, "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "%0", null, null, null, null, "%0");
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
                var lblPrice = new Label { Text = "Trafik: " + priceStr, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = ThemeRed, AutoSize = true, Location = new Point(8, py) };
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
                Height = 72,
                MinimumSize = new Size(0, 52),
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(8, 4, 8, 4)
            };

            var navFlow = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Location = new Point(12, 4),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                WrapContents = true,
                MaximumSize = new Size(0, 64),
                Padding = new Padding(0)
            };
            var navDefs = new[] {
                ("ANA EKRAN", Icons.Bilgiler, (EventHandler)null),
                ("ŞİRKETLER (ROBOT)", Icons.Robot, (EventHandler)((s, e) => SirketlerRobotRequested?.Invoke(this, EventArgs.Empty))),
                ("TEKLİFLER", Icons.Teklifler, (EventHandler)((s, e) => TekliflerRequested?.Invoke(this, EventArgs.Empty))),
                ("POLİÇELERİM", Icons.Policeler, (EventHandler)((s, e) => PolicelerimRequested?.Invoke(this, EventArgs.Empty))),
                ("Raporlar", Icons.Raporlar, (EventHandler)((s, e) => RaporlarRequested?.Invoke(this, EventArgs.Empty))),
                ("Destek Taleplerim", Icons.Destek, (EventHandler)((s, e) => DestekTalepleriRequested?.Invoke(this, EventArgs.Empty))),
                ("Ajanda / Yenileme", Icons.Ajanda, (EventHandler)((s, e) => AjandaYenilemeRequested?.Invoke(this, EventArgs.Empty))),
                ("Duyurular", Icons.Bilgiler, (EventHandler)((s, e) => DuyurularRequested?.Invoke(this, EventArgs.Empty)))
            };
            for (int i = 0; i < navDefs.Length; i++)
            {
                var (text, icon, handler) = navDefs[i];
                var btn = CreateNavButton(text, i == 0, icon);
                if (handler != null) btn.Click += handler;
                navFlow.Controls.Add(btn);
            }

            var lblLive = new Label
            {
                Text = "[LIVE]",
                BackColor = VegaRed,
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(52, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(6, 0, 0, 0),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };
            var btnCanliUretim = new Button
            {
                Text = "CANLI ÜRETİM",
                BackColor = VegaRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 28),
                Cursor = Cursors.Hand,
                Margin = new Padding(2, 0, 6, 0),
                Font = new Font("Segoe UI", 8.5F)
            };
            btnCanliUretim.FlatAppearance.BorderSize = 0;
            btnCanliUretim.Click += (s, e) => CanliUretimRequested?.Invoke(this, EventArgs.Empty);
            var btnCanliDestek = new Button
            {
                Text = "CANLI DESTEK > Sorunu Sor",
                BackColor = VegaOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(160, 28),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 0),
                Font = new Font("Segoe UI", 8.5F)
            };
            btnCanliDestek.FlatAppearance.BorderSize = 0;
            btnCanliDestek.Click += (s, e) => CanliDestekRequested?.Invoke(this, EventArgs.Empty);

            var rightPanel = new Panel { Dock = DockStyle.Right, Width = 280, Padding = new Padding(6, 4, 8, 4), MinimumSize = new Size(260, 0) };
            var rightFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Padding = new Padding(0),
                WrapContents = false
            };
            rightFlow.Controls.Add(btnCanliDestek);
            rightFlow.Controls.Add(btnCanliUretim);
            rightFlow.Controls.Add(lblLive);
            rightPanel.Controls.Add(rightFlow);
            topHeader.Controls.Add(navFlow);
            topHeader.Controls.Add(rightPanel);
            Controls.Add(topHeader);
        }

        private Button CreateNavButton(string text, bool active, Image icon = null)
        {
            int width = 90;
            if (text.Length > 15) width = 145;
            else if (text.Length > 10) width = 120;
            else if (text.Length > 6) width = 100;
            
            var btn = new Button
            {
                Text = text,
                BackColor = active ? ThemeRed : Color.FromArgb(240, 240, 240),
                ForeColor = active ? Color.White : Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(width, 28),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 2, 0),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(0, 0, 0, 0)
            };
            if (icon != null)
            {
                var resized = new Bitmap(16, 16);
                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(icon, 0, 0, 16, 16);
                }
                btn.Image = resized;
            }
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
            var chkDigerFiyat = new CheckBox { Text = "Diğer Fiyatları Göster", AutoSize = true, Margin = new Padding(0, 8, 12, 0) };
            chkDigerFiyat.CheckedChanged += (s, e) =>
            {
                _allExpanded = chkDigerFiyat.Checked;
                RefreshCompanyGrid();
                RefreshSelectedCompanyPanel();
            };
            var linkAltFiyat = new LinkLabel
            {
                Text = "Alt Fiyatları Göster",
                AutoSize = true,
                LinkColor = Color.Blue,
                Margin = new Padding(0, 10, 16, 0),
                Cursor = Cursors.Hand
            };
            linkAltFiyat.Click += (s, e) => { };
            var chkWebServis = new CheckBox { Text = "Web Servis Teklifi Çalış", AutoSize = true, Margin = new Padding(0, 8, 12, 0) };
            var chkKasko = new CheckBox { Text = "Kasko Özel Fiyat", AutoSize = true, Margin = new Padding(0, 8, 12, 0) };
            var btnPdfAktar = new Button
            {
                Text = "PDF Aktar",
                FlatStyle = FlatStyle.Flat,
                Size = new Size(90, 26),
                Margin = new Padding(0, 6, 8, 0),
                Cursor = Cursors.Hand
            };
            btnPdfAktar.Click += (s, e) => PdfExportRequested?.Invoke(this, EventArgs.Empty);
            var btnRedMinus = new Button
            {
                Text = "-",
                BackColor = VegaRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(28, 26),
                Margin = new Padding(0, 6, 4, 0),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 12F)
            };
            btnRedMinus.FlatAppearance.BorderSize = 0;
            var btnRedPlus = new Button
            {
                Text = "+",
                BackColor = VegaRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(28, 26),
                Margin = new Padding(0, 6, 4, 0),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 12F)
            };
            btnRedPlus.FlatAppearance.BorderSize = 0;
            var btnSirketEkle = new Button
            {
                Text = "+",
                BackColor = VegaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(28, 26),
                Margin = new Padding(0, 6, 0, 0),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 12F)
            };
            btnSirketEkle.FlatAppearance.BorderSize = 0;
            btnSirketEkle.Click += (s, e) => SirketEkleRequested?.Invoke(this, EventArgs.Empty);

            toolbar.Controls.Add(chkDigerFiyat);
            toolbar.Controls.Add(linkAltFiyat);
            toolbar.Controls.Add(chkWebServis);
            toolbar.Controls.Add(chkKasko);
            toolbar.Controls.Add(btnPdfAktar);
            toolbar.Controls.Add(btnRedMinus);
            toolbar.Controls.Add(btnRedPlus);
            toolbar.Controls.Add(btnSirketEkle);
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
            var teklifIdPanel = new Panel { Dock = DockStyle.Top, Height = 20, BackColor = Color.White, Padding = new Padding(8, 2, 8, 0) };
            var lblTeklifId = new Label
            {
                Text = "Teklif ID : 0",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(8, 2)
            };
            teklifIdPanel.Controls.Add(lblTeklifId);

            _companyGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = true,
                RowHeadersWidth = 24,
                ColumnHeadersVisible = true,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 26,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(4, 2, 4, 2),
                    SelectionBackColor = Color.White,
                    SelectionForeColor = Color.Black
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(4, 2, 4, 2),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    BackColor = Color.FromArgb(240, 240, 240),
                    ForeColor = Color.Black
                }
            };
            _companyGrid.RowTemplate.Height = 25;
            _companyGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            _companyGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            _companyGrid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Secim", HeaderText = "", Width = 32 });
            _companyGrid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Secim2", HeaderText = "", Width = 32 });
            _companyGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Logo", HeaderText = "", Width = 40, ReadOnly = true });
            var colSirket = new DataGridViewTextBoxColumn { Name = "Sirket", HeaderText = "Şirket", MinimumWidth = 100 };
            _companyGrid.Columns.Add(colSirket);
            _companyGrid.Columns.Add("Trafik", "Trafik");
            _companyGrid.Columns.Add("Sbm", "Sbm");
            _companyGrid.Columns.Add("Kasko", "Kasko");
            _companyGrid.Columns.Add("TssAy", "TSS Ayak.");
            _companyGrid.Columns.Add("TssYat", "TSS Yat.");
            _companyGrid.Columns.Add("Konut", "Konut");
            _companyGrid.Columns.Add("Dask", "Dask");
            _companyGrid.Columns.Add("Imm", "İMM");
            _companyGrid.Columns.Add("Yuzde", "%");
            _companyGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Icon1", HeaderText = "", Width = 28, ReadOnly = true });
            _companyGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Icon2", HeaderText = "", Width = 28, ReadOnly = true });
            _companyGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Icon3", HeaderText = "", Width = 28, ReadOnly = true });
            _companyGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Icon4", HeaderText = "", Width = 28, ReadOnly = true });
            var colUyari = new DataGridViewTextBoxColumn { Name = "Uyari", HeaderText = "Uyarı", MinimumWidth = 80 };
            _companyGrid.Columns.Add(colUyari);
            _companyGrid.Columns["Sirket"].FillWeight = 100;
            _companyGrid.Columns["Uyari"].FillWeight = 80;
            if (_companyGrid.Columns["Secim"] != null)
                _companyGrid.Columns["Secim"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (_companyGrid.Columns["Secim2"] != null)
                _companyGrid.Columns["Secim2"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _companyGrid.RowPrePaint += CompanyGrid_RowPrePaint;
            _companyGrid.EnableHeadersVisualStyles = false;
            _companyGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            _companyGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = _companyGrid.ColumnHeadersDefaultCellStyle.BackColor;
            _companyGrid.CellFormatting += CompanyGrid_CellFormatting;
            _companyGrid.CellPainting += CompanyGrid_CellPainting;
            _companyGrid.CellDoubleClick += CompanyGrid_CellDoubleClick;
            _companyGrid.CellClick += CompanyGrid_CellClick;

            _mainSplit.Panel1.Controls.Add(_companyGrid);
            _mainSplit.Panel1.Controls.Add(teklifIdPanel);
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

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.White, Padding = new Padding(8) };
            var sorguBaslatBar = new Panel { BackColor = ThemeRed, Size = new Size(280, 30), Location = new Point(4, 4) };
            var lblSearchIcon = new Label { Text = "\u2315", Font = new Font("Segoe UI", 12F), ForeColor = Color.White, Location = new Point(6, 4), AutoSize = true };
            var lblSorguBaslat = new Label
            {
                Text = "Sorguyu Başlat",
                BackColor = ThemeRed,
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(24, 5),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            sorguBaslatBar.Controls.Add(lblSearchIcon);
            sorguBaslatBar.Controls.Add(lblSorguBaslat);
            headerPanel.Controls.Add(sorguBaslatBar);
            var lblTrafik = new Label { Text = "TRAFİK", AutoSize = true, Location = new Point(8, 42), Font = new Font("Segoe UI", 8F) };
            _cmbPolicyType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 140,
                Height = 24,
                Location = new Point(8, 62),
                FlatStyle = FlatStyle.Standard,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            };
            _cmbPolicyType.Items.AddRange(new object[] { "TRAFİK", "KASKO", "DASK", "TSS", "KONUT", "İMM" });
            _cmbPolicyType.SelectedIndex = 0;
            _cmbPolicyType.SelectedIndexChanged += (s, e) =>
            {
                _companyGrid?.Invalidate();
                RefreshSelectedCompanyPanel();
            };
            var lblTarayici = new Label { Text = "Tarayıcı 1", AutoSize = true, Location = new Point(160, 42), Font = new Font("Segoe UI", 8F) };
            _cmbTarayici = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 120,
                Height = 24,
                Location = new Point(156, 62),
                BackColor = Color.FromArgb(255, 230, 230),
                FlatStyle = FlatStyle.Standard,
                Font = new Font("Segoe UI", 9F)
            };
            _cmbTarayici.Items.Add("Tarayıcı 1");
            _cmbTarayici.Items.Add("Tarayıcı 2");
            _cmbTarayici.SelectedIndex = 0;
            headerPanel.Controls.Add(lblTrafik);
            headerPanel.Controls.Add(_cmbPolicyType);
            headerPanel.Controls.Add(lblTarayici);
            headerPanel.Controls.Add(_cmbTarayici);

            var rightContent = new Panel { Dock = DockStyle.Fill };
            var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(8) };

            _rightActionPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 130,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(4)
            };
            int ay = 8;
            var btnYeniSorgu = CreateActionButton("Yeni Sorgu Kaydet", Icons.YeniSorgu, ref ay);
            btnYeniSorgu.Click += BtnYeniSorgu_Click;
            var btnKuyruk = CreateActionButton("Kuyruklama Sorgu", Icons.KuyruklamaSorgu, ref ay);
            btnKuyruk.Click += (s, e) => KuyrukSorgusuRequested?.Invoke(this, EventArgs.Empty);
            _btnDuraklat = CreateActionButton("Duraklat / Devam Et", Icons.Devam, ref ay);
            _btnDuraklat.Click += BtnDuraklat_Click;
            var btnRuhsatQr = CreateActionButton("Ruhsat QR", Icons.RuhsatQr, ref ay);
            var btnSbm = CreateActionButton("Sbm Sorgu", Icons.Sbm, ref ay);
            btnSbm.Click += (s, e) => SbmSorgusuRequested?.Invoke(this, EventArgs.Empty);
            var btnWebcam = CreateActionButton("Webcam Oku", Icons.Manuel, ref ay);
            btnWebcam.Click += (s, e) => WebcamQRRequested?.Invoke(this, EventArgs.Empty);
            var btnPdfYukle = CreateActionButton("Pdf Yükle", Icons.PdfYukle, ref ay);
            btnPdfYukle.Click += (s, e) => PdfUploadRequested?.Invoke(this, EventArgs.Empty);
            var btnTarayiciBaslat = CreateActionButton("Tarayıcıları Başlat", Icons.Robot, ref ay);
            var btnTemizle = CreateActionButton("Temizle", Icons.FormTemizleme, ref ay);
            _rightActionPanel.Controls.AddRange(new Control[] { btnYeniSorgu, btnKuyruk, _btnDuraklat, btnRuhsatQr, btnSbm, btnWebcam, btnPdfYukle, btnTarayiciBaslat, btnTemizle });
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
            AddFormRow(grpMusteri, "Plaka", "06BFT02", ref fy, true, "Varsa Getir", "", fieldKey: "Plaka");
            AddFormRow(grpMusteri, "TC / Vergi", "13627046986", ref fy, fieldKey: "TcVergi");
            AddFormRow(grpMusteri, "Belge Seri", "GL", ref fy, secondText: "152109", fieldKey: "BelgeSeri", fieldKey2: "BelgeNo");
            AddFormRowDatePicker(grpMusteri, "Doğum T.", ref fy, "DogumTarihi");
            AddFormRowDtSirketi(grpMusteri, "DT Şirketi", ref fy);
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
            AddFormRowMarkaTip(tabArac, ref ty, "Marka", "Tip");
            AddFormRow(tabArac, "Model Yılı", "2000", ref ty, fieldKey: "ModelYili");
            AddFormRow(tabArac, "Kasko Değeri", "", ref ty);
            AddFormRow(tabArac, "Tescil Tarihi", "19.01.2012", ref ty);
            AddFormRow(tabArac, "Koltuk Sayısı", "1", ref ty);
            AddFormRow(tabArac, "Motor No", "SE706025", ref ty);
            AddFormRow(tabArac, "Şasi No", "", ref ty);
            var chkLpg = new CheckBox { Text = "LPG", AutoSize = true, Location = new Point(FormInputLeft, ty + 2) };
            var txtLpg = new TextBox { Text = "0", Location = new Point(FormInputLeft + 50, ty), Width = 60 };
            tabArac.Controls.Add(chkLpg);
            tabArac.Controls.Add(txtLpg);
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

            rightContent.Controls.Add(scrollPanel);
            
            // Dock order: add Fill first, then Top - WinForms docks in reverse order
            rightPanel.SuspendLayout();
            rightPanel.Controls.Clear();
            rightPanel.Controls.Add(rightContent);  // Dock.Fill - will fill remaining space
            rightPanel.Controls.Add(headerPanel);   // Dock.Top - will dock at top first
            rightPanel.ResumeLayout(true);
        }

        private const int FormLabelWidth = 110;
        private const int FormInputLeft = 124;
        private const int FormInputGap = 6;
        private const int FormRowHeight = 28;
        private const int FormRowHeightMulti = 44;

        private void AddFormRow(Control parent, string labelText, string value, ref int y, bool withCheck = false, string checkText = "", string secondCheckText = "", string secondText = "", bool isCombo = false, bool secondCombo = false, bool multiLine = false, string fieldKey = null, string fieldKey2 = null)
        {
            int rowTop = y;
            const int labelVertOffset = 4;
            int inputTop = rowTop + 3;
            var lbl = new Label
            {
                Text = labelText + ":",
                AutoSize = false,
                Size = new Size(FormLabelWidth, 20),
                Location = new Point(8, rowTop + labelVertOffset),
                AutoEllipsis = true,
                Font = new Font(parent.Font.FontFamily, 9F)
            };
            parent.Controls.Add(lbl);
            Control input;
            int firstInputWidth = string.IsNullOrEmpty(secondText) ? 180 : 72;
            if (isCombo)
            {
                input = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = inputTop, Width = 180, Height = 22 };
                ((ComboBox)input).Items.Add("Seçiniz");
                ((ComboBox)input).SelectedIndex = 0;
            }
            else if (multiLine)
            {
                input = new TextBox { Multiline = true, Left = FormInputLeft, Top = inputTop, Width = 240, Height = 36 };
            }
            else
            {
                input = new TextBox { Left = FormInputLeft, Top = inputTop, Width = firstInputWidth, Height = 22, Text = value };
            }
            parent.Controls.Add(input);
            if (!string.IsNullOrEmpty(fieldKey)) _formFields[fieldKey] = input;
            int nx = FormInputLeft + 184;
            Control txt2 = null;
            if (withCheck)
            {
                var chk1 = new CheckBox { Text = checkText, AutoSize = true, Location = new Point(nx, rowTop + labelVertOffset), Font = new Font(parent.Font.FontFamily, 9F) };
                parent.Controls.Add(chk1);
                nx += 60;
                var chk2 = new CheckBox { Text = secondCheckText, AutoSize = true, Location = new Point(nx, rowTop + labelVertOffset), Font = new Font(parent.Font.FontFamily, 9F) };
                parent.Controls.Add(chk2);
            }
            if (!string.IsNullOrEmpty(secondText))
            {
                parent.Controls.Remove(input);
                int groupW = firstInputWidth + FormInputGap + 22 + 78;
                var groupPanel = new Panel
                {
                    Location = new Point(FormInputLeft, rowTop + 2),
                    Size = new Size(groupW, 24),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                var firstInput = input as TextBox;
                if (firstInput != null)
                {
                    firstInput.BorderStyle = BorderStyle.None;
                    firstInput.Location = new Point(2, 2);
                    firstInput.Size = new Size(firstInputWidth - 2, 20);
                    groupPanel.Controls.Add(firstInput);
                }
                var lblNo = new Label { Text = "No:", AutoSize = true, Location = new Point(firstInputWidth + FormInputGap + 2, 4), Font = new Font(parent.Font.FontFamily, 8F), ForeColor = Color.Gray };
                groupPanel.Controls.Add(lblNo);
                txt2 = new TextBox { Left = firstInputWidth + FormInputGap + 22, Top = 2, Width = 76, Height = 20, Text = secondText, BorderStyle = BorderStyle.None };
                groupPanel.Controls.Add(txt2);
                parent.Controls.Add(groupPanel);
                if (!string.IsNullOrEmpty(fieldKey2)) _formFields[fieldKey2] = txt2;
            }
            if (secondCombo)
            {
                var cmb2 = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft + 184 + FormInputGap, Top = inputTop, Width = 100, Height = 22 };
                cmb2.Items.Add("Seçiniz");
                cmb2.SelectedIndex = 0;
                parent.Controls.Add(cmb2);
            }
            y += multiLine ? FormRowHeightMulti : FormRowHeight;
        }

        private void AddFormRowWithItems(Control parent, string labelText, ref int y, string[] items, string fieldKey = null)
        {
            int rowTop = y;
            var lbl = new Label { Text = labelText + ":", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lbl);
            var cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = rowTop + 3, Width = 180, Height = 22 };
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
            var lblKalan = new Label { Text = "Kalan Gün:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lblKalan);
            var txtKalan = new TextBox { Left = FormInputLeft, Top = rowTop + 3, Width = 180, Height = 22, Text = "0", ReadOnly = true };
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
            var lblBaslangic = new Label { Text = "Başlangıç T.:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lblBaslangic);
            var txtBaslangic = new TextBox { Left = FormInputLeft, Top = rowTop + 3, Width = 180, Height = 22 };
            txtBaslangic.TextChanged += (s, e) => HesaplaKalanGun(parent);
            parent.Controls.Add(txtBaslangic);
            y += FormRowHeight;

            rowTop = y;
            var lblBitis = new Label { Text = "Bitiş T.:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lblBitis);
            var txtBitis = new TextBox { Left = FormInputLeft, Top = rowTop + 3, Width = 180, Height = 22 };
            txtBitis.TextChanged += (s, e) => HesaplaKalanGun(parent);
            parent.Controls.Add(txtBitis);
            y += FormRowHeight;

            rowTop = y;
            var lblKalan = new Label { Text = "Kalan Gün:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lblKalan);
            var txtKalan = new TextBox { Left = FormInputLeft, Top = rowTop + 3, Width = 180, Height = 22, Text = "0", ReadOnly = true };
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
            var lblMarka = new Label { Text = "Marka:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lblMarka);
            var cmbMarka = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = rowTop + 3, Width = 180, Height = 22 };
            cmbMarka.Items.AddRange(VehicleBrandsAndTypes.GetBrandDisplays());
            if (cmbMarka.Items.Count > 0) cmbMarka.SelectedIndex = 0;
            parent.Controls.Add(cmbMarka);
            if (!string.IsNullOrEmpty(fieldKeyMarka)) _formFields[fieldKeyMarka] = cmbMarka;
            y += FormRowHeight;

            rowTop = y;
            var lblTip = new Label { Text = "Tip:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lblTip);
            var cmbTip = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = rowTop + 3, Width = 180, Height = 22 };
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
            var lbl = new Label { Text = "İl / İlçe:", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lbl);
            _cmbIl = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = rowTop + 3, Width = 120, Height = 22 };
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
            _cmbIlce = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft + 120 + FormInputGap, Top = rowTop + 3, Width = 100, Height = 22 };
            if (_cmbIl.SelectedItem != null)
            {
                var districts = TurkeyLocations.GetDistrictsByCity(_cmbIl.SelectedItem.ToString());
                foreach (var d in districts) _cmbIlce.Items.Add(d);
                if (_cmbIlce.Items.Count > 0) _cmbIlce.SelectedIndex = 0;
            }
            parent.Controls.Add(_cmbIlce);
            y += FormRowHeight;
        }

        private void AddFormRowDatePicker(Control parent, string labelText, ref int y, string fieldKey)
        {
            int rowTop = y;
            var lbl = new Label { Text = labelText + ":", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lbl);
            
            var dtp = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Left = FormInputLeft,
                Top = rowTop + 3,
                Width = 100,
                Height = 22
            };
            parent.Controls.Add(dtp);
            if (!string.IsNullOrEmpty(fieldKey)) _formFields[fieldKey] = dtp;
            
            var lblIcon = new Label { Text = "\uD83D\uDCC5", Font = new Font("Segoe UI", 10F), Location = new Point(FormInputLeft + 104, rowTop + 3), AutoSize = true, ForeColor = Color.Gray };
            parent.Controls.Add(lblIcon);
            
            y += FormRowHeight;
        }

        private void AddFormRowDtSirketi(Control parent, string labelText, ref int y)
        {
            int rowTop = y;
            var lbl = new Label { Text = labelText + ":", AutoSize = false, Size = new Size(FormLabelWidth, 20), Location = new Point(8, rowTop + 4), AutoEllipsis = true, Font = new Font(parent.Font.FontFamily, 9F) };
            parent.Controls.Add(lbl);
            
            var cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = FormInputLeft, Top = rowTop + 3, Width = 140, Height = 22 };
            cmb.Items.Add("Seçiniz");
            cmb.SelectedIndex = 0;
            parent.Controls.Add(cmb);
            
            var lblFolder = new Label { Text = "\uD83D\uDCC1", Font = new Font("Segoe UI", 12F), Location = new Point(FormInputLeft + 144, rowTop + 2), AutoSize = true, ForeColor = Color.FromArgb(0, 150, 57) };
            parent.Controls.Add(lblFolder);
            
            y += FormRowHeight;
        }

        private Button CreateActionButton(string text, Image icon, ref int y)
        {
            var btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(122, 32),
                Location = new Point(4, y),
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(2, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            if (icon != null)
            {
                var resizedIcon = new Bitmap(18, 18);
                using (var g = Graphics.FromImage(resizedIcon))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(icon, 0, 0, 18, 18);
                }
                btn.Image = resizedIcon;
            }
            btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            y += 36;
            return btn;
        }

        private void BuildStatusBar()
        {
            const int StatusItemGap = 16;
            const int FooterTopMargin = 20;
            var statusWrapper = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32 + FooterTopMargin,
                BackColor = Color.Transparent,
                Padding = new Padding(0, FooterTopMargin, 0, 0)
            };
            var statusBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(12, 6, 12, 6)
            };
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(0) };
            var statusFont = new Font("Segoe UI", 8.5F);

            AddStatusItem(flow, Icons.Tramer, "Tramer", Color.LimeGreen, statusFont, StatusItemGap);
            AddStatusItem(flow, Icons.Egm, "Egm", Color.LimeGreen, statusFont, StatusItemGap);
            AddStatusItem(flow, Icons.SanalT, "Sanal T.", Color.Orange, statusFont, StatusItemGap);
            AddStatusItem(flow, Icons.DisaAl, "Dışa Al", Color.Orange, statusFont, StatusItemGap);

            var lblRam = new Label { Text = "Ram:", AutoSize = true, Margin = new Padding(0, 4, 4, 0), Font = statusFont };
            flow.Controls.Add(lblRam);
            var ramProgress = new ProgressBar { Size = new Size(80, 14), Value = 50, Margin = new Padding(0, 5, StatusItemGap, 0) };
            flow.Controls.Add(ramProgress);

            for (int i = 0; i < 5; i++)
            {
                var circle = new Panel { Size = new Size(10, 10), Margin = new Padding(0, 5, 4, 0), BackColor = GetCircleColor(i) };
                flow.Controls.Add(circle);
            }
            statusBar.Controls.Add(flow);
            statusWrapper.Controls.Add(statusBar);
            Controls.Add(statusWrapper);
        }

        private void AddStatusItem(FlowLayoutPanel flow, Image icon, string text, Color indicatorColor, Font font, int gap)
        {
            if (icon != null)
            {
                var pb = new PictureBox { Size = new Size(16, 16), Margin = new Padding(0, 4, 4, 0), SizeMode = PictureBoxSizeMode.StretchImage, Image = icon };
                flow.Controls.Add(pb);
            }
            else
            {
                var p = new Panel { Size = new Size(10, 10), BackColor = indicatorColor, Margin = new Padding(0, 5, 4, 0) };
                flow.Controls.Add(p);
            }
            var lbl = new Label { Text = text, AutoSize = true, Margin = new Padding(0, 4, gap, 0), Font = font };
            flow.Controls.Add(lbl);
        }

        private static Color GetCircleColor(int index)
        {
            switch (index)
            {
                case 0: return Color.LimeGreen;
                case 1: return Color.DeepSkyBlue;
                case 2: return Color.Orange;
                case 3: return Color.Red;
                default: return Color.Gray;
            }
        }
    }
}
