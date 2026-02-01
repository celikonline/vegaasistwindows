using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Windows.UserControls;

namespace VegaAsis.Windows.Forms
{
    public class YetkilendirmeForm : Form
    {
        private readonly ICompanySettingsService _companySettingsService;
        private readonly IUserManagementService _userManagementService;
        private readonly IAuthService _authService;
        private TabControl _tabControl;
        private SirketAyarlariPanel _sirketAyarlariPanel;

        private static readonly string[] AllCompanies = new[]
        {
            "ACIBADEM", "ACNTURK", "AK", "ALLIANZ", "ANA", "ANADOLU", "ANKARA", "ANKARASFS",
            "AREX", "ATLAS", "AVEON", "AXA", "BEREKET", "BEREKETNARLINE", "CORPUS",
            "DEMIROFFICE", "DOGA", "ETHICA", "EUREKO", "FIBA", "GENERALI", "GENERALI_PORTAL",
            "GRI", "GROUPAMA", "GROUPAMA_ANKA", "GULF", "GULFSFS", "HDI", "HDIWEB",
            "HDI_AS400", "HEPIYI", "KORU", "MAGDEBURGER", "MAGDEBURGERP", "MAPFRE",
            "MAPFREMAP", "NEOVA", "ORIENT", "QUICK", "QUICK_PORTAL", "RAY", "RAYSFS",
            "SOMPO", "SOMPOSFS", "TRAMER", "TURKNIPPON", "UNICO", "ZURICH"
        };

        public YetkilendirmeForm(
            ICompanySettingsService companySettingsService,
            IUserManagementService userManagementService,
            IAuthService authService)
        {
            _companySettingsService = companySettingsService ?? throw new ArgumentNullException(nameof(companySettingsService));
            _userManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            Text = "Yetkilendirme";
            Size = new Size(1100, 700);
            MinimumSize = new Size(900, 550);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            BackColor = SystemColors.Control;
            Padding = new Padding(0);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(34, 139, 34),
                Padding = new Padding(8, 0, 0, 0)
            };

            var lblTitle = new Label
            {
                Text = "▲ Yetkilendirme",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            headerPanel.Controls.Add(lblTitle);

            _tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                Padding = new Point(4, 2)
            };

            _sirketAyarlariPanel = new SirketAyarlariPanel(_companySettingsService, AllCompanies);
            var sirketTab = new TabPage("Şirket Ayarları");
            sirketTab.Controls.Add(_sirketAyarlariPanel);

            var kullaniciTab = new TabPage("Kullanıcı Ayarları");
            var kullaniciControl = new KullaniciAyarlariUserControl(_userManagementService, _companySettingsService, _authService);
            kullaniciControl.Dock = DockStyle.Fill;
            kullaniciTab.Controls.Add(kullaniciControl);

            _tabControl.TabPages.Add(sirketTab);
            _tabControl.TabPages.Add(kullaniciTab);
            _tabControl.TabPages.Add(CreatePlaceholderTab("Paylaşılan Şirketler", "Paylaşılan şirketler sekmesi (yakında)"));
            _tabControl.TabPages.Add(CreatePlaceholderTab("WEB Ekranları (hızlıteklif)", "WEB ekranları sekmesi (yakında)"));
            _tabControl.TabPages.Add(CreatePlaceholderTab("Diğer Ayarlar", "Diğer ayarlar sekmesi (yakında)"));
            _tabControl.TabPages.Add(CreatePlaceholderTab("Teklifler", "Teklifler sekmesi (yakında)"));
            _tabControl.TabPages.Add(CreatePlaceholderTab("Gruplar", "Gruplar sekmesi (yakında)"));
            _tabControl.TabPages.Add(CreatePlaceholderTab("Benchmark", "Benchmark sekmesi (yakında)"));
            _tabControl.TabPages.Add(CreatePlaceholderTab("Kota Ayarları", "Kota ayarları sekmesi (yakında)"));

            Controls.Add(_tabControl);
            Controls.Add(headerPanel);

            Load += async (s, e) =>
            {
                await _companySettingsService.LoadSettingsAsync().ConfigureAwait(false);
                await _sirketAyarlariPanel.RefreshAsync().ConfigureAwait(false);
            };
        }

        private static TabPage CreatePlaceholderTab(string title, string text)
        {
            var tab = new TabPage(title);
            var lbl = new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(16, 16),
                Font = new Font("Segoe UI", 10F)
            };
            tab.Controls.Add(lbl);
            return tab;
        }
    }

    internal class SirketAyarlariPanel : UserControl
    {
        private readonly ICompanySettingsService _service;
        private readonly string[] _allCompanies;
        private ListBox _lstAvailable;
        private ListBox _lstSelected;
        private string _editingCompany;
        private readonly List<YasakItem> _yasaklar = new List<YasakItem>();
        private FlowLayoutPanel _yasaklarPanel;

        private TextBox _txtSirketAdi;
        private TextBox _txtProxy;
        private TextBox _txtProxyUser;
        private TextBox _txtProxyPass;
        private TextBox _txtKullanici;
        private TextBox _txtSifre;
        private TextBox _txtGoogleKey;
        private TextBox _txtKaskoOran;
        private TextBox _txtIp;
        private CheckBox _chkTrafikTeklifi;
        private CheckBox _chkKaskoTeklifi;
        private CheckBox _chkOtoSession;
        private CheckBox _chkIkameArac;
        private CheckBox _chkKesilenPaket;
        private Button _btnKaydet;
        private bool _isSaving;

        private static readonly (string Id, string Label)[] YasaklarData = new[]
        {
            ("acik_police_aznet_nakit", "Açık Poliçe - Aznet Nakit"),
            ("acik_police_aznet_mail", "Açık Poliçe - Aznet Mail"),
            ("acik_police_pmc_mail", "Açık Poliçe - PMC Mail"),
            ("acik_police_pmc_nakit", "Açık Poliçe - PMC Nakit"),
            ("yk_arac_trafik_yasaklama", "YK Araç Trafik Yasaklama"),
            ("yk_arac_kasko_yasaklama", "YK Araç Kasko Yasaklama"),
            ("bonus_indirimi_kasko", "Bonus İndirimi - Kasko"),
            ("bonus_indirimi_trafik", "Bonus İndirimi - Trafik"),
            ("trafik_yasaklama", "Trafik Yasaklama"),
            ("kasko_yasaklama", "Kasko Yasaklama"),
            ("zeyl_yasaklama", "Zeyl Yasaklama"),
            ("dask_yasaklama", "Dask Yasaklama")
        };

        public SirketAyarlariPanel(ICompanySettingsService service, string[] allCompanies)
        {
            _service = service;
            _allCompanies = allCompanies ?? new string[0];
            Dock = DockStyle.Fill;
            Padding = new Padding(8);
            BackColor = SystemColors.Control;

            BuildUi();
        }

        private void BuildUi()
        {
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                Padding = new Padding(0)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var leftPanel = CreateCompaniesListPanel("Tüm Şirketler", false, out _lstAvailable);
            var transferPanel = CreateTransferPanel();
            var selectedPanel = CreateCompaniesListPanel("Seçili", true, out _lstSelected);
            var formPanel = CreateFormPanel();
            var yasaklarPanel = CreateYasaklarPanel();

            table.Controls.Add(leftPanel, 0, 0);
            table.Controls.Add(transferPanel, 1, 0);
            table.Controls.Add(selectedPanel, 2, 0);
            table.Controls.Add(formPanel, 3, 0);
            table.Controls.Add(yasaklarPanel, 4, 0);

            Controls.Add(table);

            _lstSelected.DoubleClick += (s, e) =>
            {
                if (_lstSelected.SelectedItem != null)
                {
                    _editingCompany = _lstSelected.SelectedItem.ToString();
                    LoadCompanyFormData(_editingCompany);
                }
            };
        }

        private Panel CreateCompaniesListPanel(string title, bool isSelected, out ListBox listBox)
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Dock = DockStyle.Fill,
                MinimumSize = new Size(100, 400)
            };

            var lblHeader = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(8, 6),
                AutoSize = true,
                ForeColor = Color.FromArgb(34, 139, 34)
            };
            panel.Controls.Add(lblHeader);

            listBox = new ListBox
            {
                Location = new Point(0, 28),
                Width = panel.Width - 4,
                Height = 420,
                IntegralHeight = false,
                Font = new Font("Segoe UI", 9F),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            var theListBox = listBox;
            panel.Resize += (s, e) =>
            {
                theListBox.Width = Math.Max(80, panel.Width - 4);
                theListBox.Height = Math.Max(100, panel.Height - 36);
            };
            panel.Controls.Add(listBox);

            return panel;
        }

        private Panel CreateTransferPanel()
        {
            var panel = new Panel
            {
                Width = 36,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(4)
            };

            var btnAdd = new Button
            {
                Text = ">",
                Size = new Size(28, 24),
                Location = new Point(4, 60),
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.Click += async (s, e) => await AddCompanyAsync().ConfigureAwait(true);

            var btnRemove = new Button
            {
                Text = "<",
                Size = new Size(28, 24),
                Location = new Point(4, 90),
                FlatStyle = FlatStyle.Flat
            };
            btnRemove.Click += async (s, e) => await RemoveCompanyAsync().ConfigureAwait(true);

            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnRemove);
            return panel;
        }

        private Panel CreateFormPanel()
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Dock = DockStyle.Fill,
                MinimumSize = new Size(220, 450),
                Padding = new Padding(8)
            };

            var y = 8;
            var lblGenel = new Label { Text = "GENEL", Location = new Point(8, y), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            panel.Controls.Add(lblGenel);
            y += 24;

            AddFormRow(panel, ref y, "Şirket Adı", out _txtSirketAdi, 55);
            AddFormRow(panel, ref y, "Proxy", out _txtProxy, 55);

            var rowUser = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Location = new Point(8, y), AutoSize = true };
            var lblUser = new Label { Text = "User", Width = 35, AutoSize = false };
            _txtProxyUser = new TextBox { Width = 85 };
            rowUser.Controls.Add(lblUser);
            rowUser.Controls.Add(_txtProxyUser);
            panel.Controls.Add(rowUser);
            y += 28;

            var rowPass = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Location = new Point(8, y), AutoSize = true };
            var lblPass = new Label { Text = "Pass", Width = 35 };
            _txtProxyPass = new TextBox { Width = 85, UseSystemPasswordChar = true };
            rowPass.Controls.Add(lblPass);
            rowPass.Controls.Add(_txtProxyPass);
            panel.Controls.Add(rowPass);
            y += 28;

            AddFormRow(panel, ref y, "Kullanıcı", out _txtKullanici, 55);
            AddFormRow(panel, ref y, "Şifre", out _txtSifre, 55);
            _txtSifre.UseSystemPasswordChar = true;

            var rowGoogle = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Location = new Point(8, y), AutoSize = true };
            var lblGoogle = new Label { Text = "Google Key", Width = 65 };
            _txtGoogleKey = new TextBox { Width = 130 };
            var btnQr = new Button { Text = "QR", Width = 28, Height = 24 };
            rowGoogle.Controls.Add(lblGoogle);
            rowGoogle.Controls.Add(_txtGoogleKey);
            rowGoogle.Controls.Add(btnQr);
            panel.Controls.Add(rowGoogle);
            y += 28;

            var rowKaskoIp = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Location = new Point(8, y), AutoSize = true };
            var lblKasko = new Label { Text = "Kasko %", Width = 45 };
            _txtKaskoOran = new TextBox { Width = 45, Text = "0" };
            var lblIp = new Label { Text = "IP", Width = 25, Location = new Point(0, 0) };
            _txtIp = new TextBox { Width = 90 };
            rowKaskoIp.Controls.Add(lblKasko);
            rowKaskoIp.Controls.Add(_txtKaskoOran);
            rowKaskoIp.Controls.Add(lblIp);
            rowKaskoIp.Controls.Add(_txtIp);
            panel.Controls.Add(rowKaskoIp);
            y += 32;

            _chkTrafikTeklifi = AddCheckbox(panel, ref y, "Trafik Teklifi Kaydet");
            _chkKaskoTeklifi = AddCheckbox(panel, ref y, "Kasko Teklifi Kaydet");
            _chkOtoSession = AddCheckbox(panel, ref y, "Oto Session Kaydet");
            _chkIkameArac = AddCheckbox(panel, ref y, "İkame Araç Hizmeti");
            _chkKesilenPaket = AddCheckbox(panel, ref y, "Kesilen Paket Getir");
            y += 8;

            _btnKaydet = new Button
            {
                Text = "Kaydet",
                Size = new Size(200, 28),
                Location = new Point(16, y),
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnKaydet.Click += async (s, e) => await SaveCompanySettingsAsync().ConfigureAwait(true);
            panel.Controls.Add(_btnKaydet);

            return panel;
        }

        private void AddFormRow(Control parent, ref int y, string labelText, out TextBox textBox, int labelWidth)
        {
            var lbl = new Label { Text = labelText, Location = new Point(8, y), Width = labelWidth, AutoSize = false };
            textBox = new TextBox { Location = new Point(labelWidth + 12, y - 2), Width = 180 };
            parent.Controls.Add(lbl);
            parent.Controls.Add(textBox);
            y += 28;
        }

        private CheckBox AddCheckbox(Control parent, ref int y, string text)
        {
            var chk = new CheckBox
            {
                Text = text,
                Location = new Point(8, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };
            parent.Controls.Add(chk);
            y += 24;
            return chk;
        }

        private Panel CreateYasaklarPanel()
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Dock = DockStyle.Fill,
                MinimumSize = new Size(260, 200)
            };

            var lblHeader = new Label
            {
                Text = "Yasakları Seçiniz",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(8, 6),
                AutoSize = true,
                ForeColor = Color.FromArgb(34, 139, 34)
            };
            panel.Controls.Add(lblHeader);

            _yasaklarPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = true,
                Location = new Point(8, 32),
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            panel.Resize += (s, e) =>
            {
                _yasaklarPanel.Size = new Size(Math.Max(200, panel.Width - 20), Math.Max(100, panel.Height - 44));
            };

            foreach (var (id, label) in YasaklarData)
            {
                var item = new YasakItem(id, label);
                item.CheckedChanged += (s, e) => { };
                _yasaklar.Add(item);
                var p = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
                p.Controls.Add(item.CheckBox);
                p.Controls.Add(new Label { Text = label, AutoSize = true });
                _yasaklarPanel.Controls.Add(p);
            }

            panel.Controls.Add(_yasaklarPanel);
            return panel;
        }

        public async Task RefreshAsync()
        {
            var selected = await _service.GetSelectedCompaniesAsync().ConfigureAwait(false);
            if (InvokeRequired)
            {
                Invoke(new Action(() => RefreshLists(selected)));
            }
            else
            {
                RefreshLists(selected);
            }
        }

        private void RefreshLists(List<string> selectedCompanies)
        {
            _lstAvailable.Items.Clear();
            _lstSelected.Items.Clear();

            var selectedSet = new HashSet<string>(selectedCompanies, StringComparer.OrdinalIgnoreCase);
            foreach (var c in _allCompanies.OrderBy(x => x))
            {
                if (selectedSet.Contains(c))
                {
                    _lstSelected.Items.Add(c);
                }
                else
                {
                    _lstAvailable.Items.Add(c);
                }
            }

            if (selectedCompanies.Count > 0 && string.IsNullOrEmpty(_editingCompany))
            {
                _editingCompany = selectedCompanies[0];
                LoadCompanyFormData(_editingCompany);
            }
        }

        private void LoadCompanyFormData(string companyName)
        {
            var setting = _service.GetCompanySetting(companyName);
            if (setting != null)
            {
                _txtSirketAdi.Text = setting.CompanyName ?? string.Empty;
                _txtProxy.Text = setting.ProxyAddress ?? string.Empty;
                _txtProxyUser.Text = setting.ProxyUser ?? string.Empty;
                _txtProxyPass.Text = setting.ProxyPassword ?? string.Empty;
                _txtKullanici.Text = setting.CompanyUsername ?? string.Empty;
                _txtSifre.Text = setting.CompanyPassword ?? string.Empty;
                _txtGoogleKey.Text = setting.GoogleSecretKey ?? string.Empty;
                _txtKaskoOran.Text = (setting.KaskoSpecialDiscount).ToString();
                _txtIp.Text = setting.IpAddresses ?? string.Empty;
                _chkTrafikTeklifi.Checked = setting.TrafikTeklifiKaydet;
                _chkKaskoTeklifi.Checked = setting.KaskoTeklifiKaydet;
                _chkOtoSession.Checked = setting.OtoSessionKaydet;
                _chkIkameArac.Checked = setting.IkameAracHizmeti;
                _chkKesilenPaket.Checked = setting.KesilenPaketGetir;

                var bans = setting.CompanyBans ?? new Dictionary<string, bool>();
                foreach (var item in _yasaklar)
                {
                    item.CheckBox.Checked = bans.TryGetValue(item.Id, out var v) && v;
                }
            }
            else
            {
                _txtSirketAdi.Text = companyName ?? string.Empty;
                _txtProxy.Text = string.Empty;
                _txtProxyUser.Text = string.Empty;
                _txtProxyPass.Text = string.Empty;
                _txtKullanici.Text = string.Empty;
                _txtSifre.Text = string.Empty;
                _txtGoogleKey.Text = string.Empty;
                _txtKaskoOran.Text = "0";
                _txtIp.Text = string.Empty;
                _chkTrafikTeklifi.Checked = false;
                _chkKaskoTeklifi.Checked = false;
                _chkOtoSession.Checked = false;
                _chkIkameArac.Checked = false;
                _chkKesilenPaket.Checked = false;
                foreach (var item in _yasaklar)
                {
                    item.CheckBox.Checked = false;
                }
            }
        }

        private async Task AddCompanyAsync()
        {
            if (_lstAvailable.SelectedItem == null) return;

            var company = _lstAvailable.SelectedItem.ToString();
            var dto = new CompanySettingsDto { CompanyName = company };
            var ok = await _service.SaveCompanySettingAsync(dto).ConfigureAwait(false);
            if (ok)
            {
                await RefreshAsync().ConfigureAwait(false);
                _editingCompany = company;
                if (InvokeRequired)
                    Invoke(new Action(() => LoadCompanyFormData(company)));
                else
                    LoadCompanyFormData(company);
                MessageBox.Show(company + " eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Şirket eklenirken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RemoveCompanyAsync()
        {
            if (_lstSelected.SelectedItem == null) return;

            var company = _lstSelected.SelectedItem.ToString();
            var ok = await _service.DeleteCompanySettingAsync(company).ConfigureAwait(false);
            if (ok)
            {
                if (_editingCompany == company)
                {
                    var selected = await _service.GetSelectedCompaniesAsync().ConfigureAwait(false);
                    _editingCompany = selected.FirstOrDefault();
                }
                await RefreshAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(_editingCompany))
                {
                    if (InvokeRequired)
                        Invoke(new Action(() => LoadCompanyFormData(_editingCompany)));
                    else
                        LoadCompanyFormData(_editingCompany);
                }
                MessageBox.Show(company + " silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Şirket silinirken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SaveCompanySettingsAsync()
        {
            if (string.IsNullOrEmpty(_editingCompany))
            {
                MessageBox.Show("Lütfen bir şirket seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isSaving) return;
            _isSaving = true;
            _btnKaydet.Enabled = false;

            try
            {
                var bans = new Dictionary<string, bool>();
                foreach (var item in _yasaklar)
                {
                    bans[item.Id] = item.CheckBox.Checked;
                }

                decimal kaskoOran = 0;
                decimal.TryParse(_txtKaskoOran.Text, out kaskoOran);

                var dto = new CompanySettingsDto
                {
                    CompanyName = _txtSirketAdi.Text.Trim(),
                    ProxyAddress = _txtProxy.Text,
                    ProxyUser = _txtProxyUser.Text,
                    ProxyPassword = _txtProxyPass.Text,
                    CompanyUsername = _txtKullanici.Text,
                    CompanyPassword = _txtSifre.Text,
                    GoogleSecretKey = _txtGoogleKey.Text,
                    KaskoSpecialDiscount = kaskoOran,
                    IpAddresses = _txtIp.Text,
                    TrafikTeklifiKaydet = _chkTrafikTeklifi.Checked,
                    KaskoTeklifiKaydet = _chkKaskoTeklifi.Checked,
                    OtoSessionKaydet = _chkOtoSession.Checked,
                    IkameAracHizmeti = _chkIkameArac.Checked,
                    KesilenPaketGetir = _chkKesilenPaket.Checked,
                    CompanyBans = bans
                };

                var ok = await _service.SaveCompanySettingAsync(dto).ConfigureAwait(false);
                if (ok)
                {
                    MessageBox.Show(_editingCompany + " ayarları güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ayarlar kaydedilirken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                _isSaving = false;
                if (_btnKaydet.InvokeRequired)
                    _btnKaydet.Invoke(new Action(() => _btnKaydet.Enabled = true));
                else
                    _btnKaydet.Enabled = true;
            }
        }

        private class YasakItem
        {
            public string Id { get; }
            public CheckBox CheckBox { get; }
            public event EventHandler CheckedChanged;

            public YasakItem(string id, string label)
            {
                Id = id;
                CheckBox = new CheckBox { Text = label, AutoSize = true };
                CheckBox.CheckedChanged += (s, e) => CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
