using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.UserControls
{
    public class KullaniciAyarlariUserControl : UserControl
    {
        private readonly IUserManagementService _userManagement;
        private readonly ICompanySettingsService _companySettings;
        private readonly IAuthService _authService;

        private Label _lblKullanicilar;
        private TextBox _txtAra;
        private ListBox _lstKullanicilar;
        private Label _lblKullaniciSayisi;
        private Panel _pnlSol;

        private Label _lblKullaniciBilgileri;
        private TextBox _txtAdSoyad;
        private TextBox _txtKullanici;
        private TextBox _txtSifre;
        private TextBox _txtGsm;
        private TextBox _txtAciklama;
        private ComboBox _cmbGrup;
        private CheckBox _chkYonetici;
        private CheckBox _chkOfisCalisani;
        private CheckBox _chkWebServisTeklifleri;
        private CheckBox _chkSessionKaydet;
        private CheckBox _chkSirketBilgileriGizle;
        private Panel _pnlOrta;

        private Label _lblYasakliSirketler;
        private FlowLayoutPanel _flpYasakliSirketler;
        private Panel _pnlSagUst;

        private Label _lblKisitlamalar;
        private TextBox _txtIpAdresi;
        private Button _btnIpEkle;
        private CheckedListBox _lstKisitlamalar;
        private Panel _pnlSagAlt;

        private Button _btnYeni;
        private Button _btnKaydet;
        private Label _lblYoneticiUyari;

        private Guid? _selectedUserId;
        private string _selectedUserCompany;
        private List<string> _selectedCompanies = new List<string>();
        private Dictionary<string, CheckBox> _companyCheckBoxes = new Dictionary<string, CheckBox>();
        private UserFormData _formData;
        private bool _isSaving;

        private static readonly KeyValuePair<string, string>[] YasaklarList = new[]
        {
            new KeyValuePair<string, string>("acik_police_kesilmesin", "Açık Poliçe Kesilmesin"),
            new KeyValuePair<string, string>("yk_arac_yasaklama", "YK Araç Yasaklama"),
            new KeyValuePair<string, string>("zeyil_yasaklama", "Zeyil Yasaklama"),
            new KeyValuePair<string, string>("acik_police_teklif_aramali", "Açık Poliçe > Teklif Aramalı"),
            new KeyValuePair<string, string>("trafik_fk_zorunlu", "Trafik FK-20.000 Zorunlu"),
            new KeyValuePair<string, string>("trafik_yasaklama", "Trafik Yasaklama"),
            new KeyValuePair<string, string>("kasko_yasaklama", "Kasko Yasaklama"),
            new KeyValuePair<string, string>("tahsilat_uretim_muhasebe_rapor", "Tahsilat-Üretim Rapor Yasaklama"),
            new KeyValuePair<string, string>("ana_menu_tahsilat_islemleri", "Ana Menü > Tahsilat İşlemleri"),
            new KeyValuePair<string, string>("dask_yasaklama", "Dask Yasaklama")
        };

        private class UserFormData
        {
            public string AdSoyad;
            public string KullaniciAdi;
            public string Sifre;
            public string Gsm;
            public string Aciklama;
            public string GrupAdi;
            public bool Yonetici;
            public bool OfisCalisan;
            public bool WebServisTeklifleri;
            public bool SessionKaydet;
            public bool SirketBilgileriGizle;
            public List<string> IzinliIpList;
            public List<string> BannedCompanies;
            public Dictionary<string, Dictionary<string, bool>> CompanyRestrictions;
        }

        public KullaniciAyarlariUserControl(
            IUserManagementService userManagement,
            ICompanySettingsService companySettings,
            IAuthService authService)
        {
            _userManagement = userManagement ?? throw new ArgumentNullException(nameof(userManagement));
            _companySettings = companySettings ?? throw new ArgumentNullException(nameof(companySettings));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _formData = NewEmptyFormData();
            MinimumSize = new Size(700, 450);
            Dock = DockStyle.Fill;
            Padding = new Padding(8);
            BackColor = SystemColors.Control;
            InitializeComponents();
        }

        private static UserFormData NewEmptyFormData()
        {
            return new UserFormData
            {
                AdSoyad = "",
                KullaniciAdi = "",
                Sifre = "",
                Gsm = "",
                Aciklama = "",
                GrupAdi = "",
                Yonetici = false,
                OfisCalisan = false,
                WebServisTeklifleri = false,
                SessionKaydet = false,
                SirketBilgileriGizle = false,
                IzinliIpList = new List<string>(),
                BannedCompanies = new List<string>(),
                CompanyRestrictions = new Dictionary<string, Dictionary<string, bool>>()
            };
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await _userManagement.CheckAdminStatusAsync().ConfigureAwait(false);
            await _userManagement.FetchUsersAsync().ConfigureAwait(false);
            await _companySettings.LoadSettingsAsync().ConfigureAwait(false);
            _selectedCompanies = await _companySettings.GetSelectedCompaniesAsync().ConfigureAwait(false);

            if (InvokeRequired)
            {
                Invoke(new Action(RefreshUserList));
                Invoke(new Action(RefreshYasakliSirketler));
                Invoke(new Action(UpdateKullaniciSayisi));
            }
            else
            {
                RefreshUserList();
                RefreshYasakliSirketler();
                UpdateKullaniciSayisi();
            }
        }

        private void InitializeComponents()
        {
            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(4)
            };
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            _pnlSol = BuildSolPanel();
            _pnlOrta = BuildOrtaPanel();
            var rightSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 140,
                Panel1 = { MinimumSize = new Size(0, 80) },
                Panel2 = { MinimumSize = new Size(0, 120) }
            };
            _pnlSagUst = BuildYasakliSirketlerPanel();
            _pnlSagAlt = BuildKisitlamalarPanel();
            rightSplit.Panel1.Controls.Add(_pnlSagUst);
            rightSplit.Panel2.Controls.Add(_pnlSagAlt);

            mainTable.Controls.Add(_pnlSol, 0, 0);
            mainTable.Controls.Add(_pnlOrta, 1, 0);
            mainTable.Controls.Add(rightSplit, 2, 0);

            var footer = new Panel { Dock = DockStyle.Fill, Height = 40 };
            _lblYoneticiUyari = new Label
            {
                Text = "Yönetici yetkisi gerekli",
                AutoSize = true,
                Location = new Point(8, 12),
                ForeColor = SystemColors.GrayText
            };
            _btnYeni = new Button
            {
                Text = "Yeni",
                Size = new Size(70, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnYeni.Location = new Point(footer.Width - 160, 8);
            _btnKaydet = new Button
            {
                Text = "Kaydet",
                Size = new Size(70, 24),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnKaydet.Location = new Point(footer.Width - 84, 8);
            footer.Controls.Add(_lblYoneticiUyari);
            footer.Controls.Add(_btnYeni);
            footer.Controls.Add(_btnKaydet);
            mainTable.Controls.Add(footer, 0, 1);
            mainTable.SetColumnSpan(footer, 3);

            _btnYeni.Click += (s, ev) => HandleNewUser();
            _btnKaydet.Click += async (s, ev) => await HandleSaveUserAsync();

            Controls.Add(mainTable);

            rightSplit.Panel1.Controls.Clear();
            rightSplit.Panel1.Controls.Add(_pnlSagUst);
            rightSplit.Panel2.Controls.Clear();
            rightSplit.Panel2.Controls.Add(_pnlSagAlt);
        }

        private Panel BuildSolPanel()
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Padding = new Padding(4)
            };
            _lblKullanicilar = new Label
            {
                Text = "Kullanıcılar",
                AutoSize = true,
                Location = new Point(6, 6),
                Font = new Font(Font.FontFamily, 9, FontStyle.Bold)
            };
            _txtAra = new TextBox
            {
                Bounds = new Rectangle(6, 26, 166, 20),
                Font = new Font(Font.FontFamily, 9)
            };
            _txtAra.TextChanged += (s, e) => FilterUserList();
            _lstKullanicilar = new ListBox
            {
                Bounds = new Rectangle(6, 50, 166, 320),
                Font = new Font(Font.FontFamily, 9),
                DisplayMember = "DisplayName"
            };
            _lstKullanicilar.SelectedIndexChanged += LstKullanicilar_SelectedIndexChanged;
            _lblKullaniciSayisi = new Label
            {
                Text = "0 kullanıcı",
                AutoSize = true,
                Location = new Point(6, 374),
                ForeColor = SystemColors.GrayText,
                Font = new Font(Font.FontFamily, 8)
            };
            pnl.Controls.Add(_lblKullanicilar);
            pnl.Controls.Add(_txtAra);
            pnl.Controls.Add(_lstKullanicilar);
            pnl.Controls.Add(_lblKullaniciSayisi);
            return pnl;
        }

        private Panel BuildOrtaPanel()
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Padding = new Padding(6),
                AutoScroll = true
            };
            var y = 6;
            _lblKullaniciBilgileri = new Label
            {
                Text = "Kullanıcı Bilgileri",
                Location = new Point(6, y),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 9, FontStyle.Bold)
            };
            pnl.Controls.Add(_lblKullaniciBilgileri);
            y += 22;

            y = AddLabelAndTextBox(pnl, "Ad Soyad", y, out _txtAdSoyad);
            y = AddLabelAndTextBox(pnl, "Kullanıcı", y, out _txtKullanici);
            y = AddLabelAndTextBox(pnl, "Şifre", y, out _txtSifre, true);
            y = AddLabelAndTextBox(pnl, "GSM", y, out _txtGsm);
            y = AddLabelAndTextBox(pnl, "Açıklama", y, out _txtAciklama);

            var lblGrup = new Label { Text = "Grup", Location = new Point(6, y), AutoSize = true };
            _cmbGrup = new ComboBox
            {
                Bounds = new Rectangle(70, y - 2, 120, 21),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font(Font.FontFamily, 9)
            };
            _cmbGrup.Items.AddRange(new object[] { "-", "Grup 1", "Grup 2" });
            _cmbGrup.SelectedIndex = 0;
            pnl.Controls.Add(lblGrup);
            pnl.Controls.Add(_cmbGrup);
            y += 26;

            _chkYonetici = AddCheckBox(pnl, "Yönetici", y); y += 22;
            _chkOfisCalisani = AddCheckBox(pnl, "Ofis Çalışanı", y); y += 22;
            _chkWebServisTeklifleri = AddCheckBox(pnl, "Web Servis Teklifleri", y); y += 22;
            _chkSessionKaydet = AddCheckBox(pnl, "Session Kaydet", y); y += 22;
            _chkSirketBilgileriGizle = AddCheckBox(pnl, "Şirket Bilgileri Gizle", y);

            _txtAdSoyad.TextChanged += (s, e) => _formData.AdSoyad = _txtAdSoyad.Text;
            _txtKullanici.TextChanged += (s, e) => _formData.KullaniciAdi = _txtKullanici.Text;
            _txtSifre.TextChanged += (s, e) => _formData.Sifre = _txtSifre.Text;
            _txtGsm.TextChanged += (s, e) => _formData.Gsm = _txtGsm.Text;
            _txtAciklama.TextChanged += (s, e) => _formData.Aciklama = _txtAciklama.Text;
            _cmbGrup.SelectedIndexChanged += (s, e) => _formData.GrupAdi = _cmbGrup.SelectedIndex <= 0 ? "" : _cmbGrup.SelectedItem?.ToString() ?? "";
            _chkYonetici.CheckedChanged += (s, e) => _formData.Yonetici = _chkYonetici.Checked;
            _chkOfisCalisani.CheckedChanged += (s, e) => _formData.OfisCalisan = _chkOfisCalisani.Checked;
            _chkWebServisTeklifleri.CheckedChanged += (s, e) => _formData.WebServisTeklifleri = _chkWebServisTeklifleri.Checked;
            _chkSessionKaydet.CheckedChanged += (s, e) => _formData.SessionKaydet = _chkSessionKaydet.Checked;
            _chkSirketBilgileriGizle.CheckedChanged += (s, e) => _formData.SirketBilgileriGizle = _chkSirketBilgileriGizle.Checked;

            return pnl;
        }

        private int AddLabelAndTextBox(Panel pnl, string labelText, int y, out TextBox txt, bool isPassword = false)
        {
            var lbl = new Label { Text = labelText, Location = new Point(6, y), AutoSize = true };
            txt = new TextBox
            {
                Bounds = new Rectangle(70, y - 2, 120, 21),
                Font = new Font(Font.FontFamily, 9),
                UseSystemPasswordChar = isPassword
            };
            pnl.Controls.Add(lbl);
            pnl.Controls.Add(txt);
            return y + 26;
        }

        private CheckBox AddCheckBox(Panel pnl, string text, int y)
        {
            var chk = new CheckBox
            {
                Text = text,
                Location = new Point(6, y),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 8)
            };
            pnl.Controls.Add(chk);
            return chk;
        }

        private Panel BuildYasakliSirketlerPanel()
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Padding = new Padding(4),
                AutoScroll = true
            };
            _lblYasakliSirketler = new Label
            {
                Text = "Yasaklı Şirketler",
                Location = new Point(6, 6),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 9, FontStyle.Bold)
            };
            _flpYasakliSirketler = new FlowLayoutPanel
            {
                Location = new Point(6, 28),
                Size = new Size(140, 100),
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown
            };
            pnl.Controls.Add(_lblYasakliSirketler);
            pnl.Controls.Add(_flpYasakliSirketler);
            return pnl;
        }

        private Panel BuildKisitlamalarPanel()
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Padding = new Padding(4),
                AutoScroll = true
            };
            _lblKisitlamalar = new Label
            {
                Text = "Kısıtlamalar",
                Location = new Point(6, 6),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 9, FontStyle.Bold)
            };
            var lblIp = new Label
            {
                Text = "İzinli IP Adresleri",
                Location = new Point(6, 28),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 8)
            };
            _txtIpAdresi = new TextBox
            {
                Bounds = new Rectangle(6, 46, 180, 20),
                Font = new Font(Font.FontFamily, 9)
            };
            _btnIpEkle = new Button
            {
                Text = "Ekle",
                Bounds = new Rectangle(192, 44, 50, 24),
                FlatStyle = FlatStyle.Flat
            };
            _btnIpEkle.Click += BtnIpEkle_Click;
            _lstKisitlamalar = new CheckedListBox
            {
                Bounds = new Rectangle(6, 74, 240, 180),
                Font = new Font(Font.FontFamily, 9),
                CheckOnClick = true
            };
            _lstKisitlamalar.ItemCheck += LstKisitlamalar_ItemCheck;
            pnl.Controls.Add(_lblKisitlamalar);
            pnl.Controls.Add(lblIp);
            pnl.Controls.Add(_txtIpAdresi);
            pnl.Controls.Add(_btnIpEkle);
            pnl.Controls.Add(_lstKisitlamalar);

            foreach (var item in YasaklarList)
            {
                _lstKisitlamalar.Items.Add(item.Value, false);
            }

            return pnl;
        }

        private void LstKisitlamalar_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_selectedUserId == null || string.IsNullOrEmpty(_selectedUserCompany) || e.Index < 0 || e.Index >= YasaklarList.Length)
            {
                return;
            }

            var item = YasaklarList[e.Index];
            if (!_formData.CompanyRestrictions.ContainsKey(_selectedUserCompany))
            {
                _formData.CompanyRestrictions[_selectedUserCompany] = new Dictionary<string, bool>();
            }
            _formData.CompanyRestrictions[_selectedUserCompany][item.Key] = e.NewValue == CheckState.Checked;
        }

        private void BtnIpEkle_Click(object sender, EventArgs e)
        {
            var ip = _txtIpAdresi.Text?.Trim();
            if (string.IsNullOrEmpty(ip))
            {
                return;
            }

            if (_formData.IzinliIpList == null)
            {
                _formData.IzinliIpList = new List<string>();
            }

            if (!_formData.IzinliIpList.Contains(ip))
            {
                _formData.IzinliIpList.Add(ip);
                _txtIpAdresi.Clear();
            }
        }

        private void RefreshRestrictionCheckStates()
        {
            if (_lstKisitlamalar == null || string.IsNullOrEmpty(_selectedUserCompany) || _formData?.CompanyRestrictions == null)
            {
                return;
            }

            var restrictions = _formData.CompanyRestrictions.ContainsKey(_selectedUserCompany)
                ? _formData.CompanyRestrictions[_selectedUserCompany]
                : null;
            for (var i = 0; i < YasaklarList.Length && i < _lstKisitlamalar.Items.Count; i++)
            {
                var key = YasaklarList[i].Key;
                var @checked = restrictions != null && restrictions.ContainsKey(key) && restrictions[key];
                _lstKisitlamalar.SetItemChecked(i, @checked);
            }
        }

        private void RefreshUserList()
        {
            var query = _txtAra?.Text?.Trim().ToLowerInvariant() ?? "";
            var filtered = _userManagement.Users
                .Where(u =>
                    (u.FullName ?? "").ToLowerInvariant().Contains(query) ||
                    (u.Email ?? "").ToLowerInvariant().Contains(query))
                .ToList();

            _lstKullanicilar.Items.Clear();
            foreach (var u in filtered)
            {
                _lstKullanicilar.Items.Add(new UserDisplayItem(u));
            }

            UpdateKullaniciSayisi();
        }

        private void FilterUserList()
        {
            RefreshUserList();
        }

        private void UpdateKullaniciSayisi()
        {
            var count = _lstKullanicilar?.Items?.Count ?? 0;
            if (_lblKullaniciSayisi != null)
            {
                _lblKullaniciSayisi.Text = count + " kullanıcı";
            }
        }

        private void LstKullanicilar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lstKullanicilar.SelectedItem is UserDisplayItem item)
            {
                _selectedUserId = item.Dto.UserId;
                LoadUserFormData(item.Dto);
            }
        }

        private void LoadUserFormData(UserDataDto user)
        {
            var settings = _userManagement.GetUserSettings(user.UserId);
            _formData = new UserFormData
            {
                AdSoyad = user.FullName ?? "",
                KullaniciAdi = user.Email ?? "",
                Sifre = "",
                Gsm = settings?.Gsm ?? "",
                Aciklama = settings?.Description ?? "",
                GrupAdi = settings?.GroupName ?? "",
                Yonetici = false,
                OfisCalisan = settings?.IsOfficeWorker ?? false,
                WebServisTeklifleri = settings?.CanViewWebService ?? false,
                SessionKaydet = settings?.CanSaveSession ?? false,
                SirketBilgileriGizle = settings?.HideCompanyDetails ?? false,
                IzinliIpList = settings?.AllowedIps != null ? new List<string>(settings.AllowedIps) : new List<string>(),
                BannedCompanies = settings?.BannedCompanies != null ? new List<string>(settings.BannedCompanies) : new List<string>(),
                CompanyRestrictions = settings?.CompanyRestrictions != null
                    ? new Dictionary<string, Dictionary<string, bool>>(settings.CompanyRestrictions.ToDictionary(k => k.Key, v => new Dictionary<string, bool>(v.Value)))
                    : new Dictionary<string, Dictionary<string, bool>>()
            };

            _txtAdSoyad.Text = _formData.AdSoyad;
            _txtKullanici.Text = _formData.KullaniciAdi;
            _txtSifre.Text = _formData.Sifre;
            _txtGsm.Text = _formData.Gsm;
            _txtAciklama.Text = _formData.Aciklama;
            _cmbGrup.SelectedIndex = string.IsNullOrEmpty(_formData.GrupAdi) ? 0 : (_cmbGrup.Items.IndexOf(_formData.GrupAdi) >= 0 ? _cmbGrup.Items.IndexOf(_formData.GrupAdi) : 0);
            _chkYonetici.Checked = _formData.Yonetici;
            _chkOfisCalisani.Checked = _formData.OfisCalisan;
            _chkWebServisTeklifleri.Checked = _formData.WebServisTeklifleri;
            _chkSessionKaydet.Checked = _formData.SessionKaydet;
            _chkSirketBilgileriGizle.Checked = _formData.SirketBilgileriGizle;

            RefreshYasakliSirketler();
        }

        private void RefreshYasakliSirketler()
        {
            _flpYasakliSirketler.Controls.Clear();
            _companyCheckBoxes.Clear();
            foreach (var company in _selectedCompanies)
            {
                var chk = new CheckBox
                {
                    Text = company,
                    AutoSize = true,
                    Font = new Font(Font.FontFamily, 9),
                    Checked = _formData?.BannedCompanies != null && _formData.BannedCompanies.Contains(company)
                };
                chk.CheckedChanged += (s, e) =>
                {
                    if (_formData == null) return;
                    if (chk.Checked && !_formData.BannedCompanies.Contains(company))
                    {
                        _formData.BannedCompanies.Add(company);
                    }
                    else if (!chk.Checked && _formData.BannedCompanies.Contains(company))
                    {
                        _formData.BannedCompanies.Remove(company);
                    }
                };
                chk.Click += (s, e) =>
                {
                    _selectedUserCompany = company;
                    if (_lblKisitlamalar != null)
                    {
                        _lblKisitlamalar.Text = "Kısıtlamalar (" + company + ")";
                    }
                    RefreshRestrictionCheckStates();
                };
                _flpYasakliSirketler.Controls.Add(chk);
                _companyCheckBoxes[company] = chk;
            }
        }

        private void HandleNewUser()
        {
            _selectedUserId = null;
            _selectedUserCompany = null;
            _formData = NewEmptyFormData();
            _txtAdSoyad.Text = "";
            _txtKullanici.Text = "";
            _txtSifre.Text = "";
            _txtGsm.Text = "";
            _txtAciklama.Text = "";
            _cmbGrup.SelectedIndex = 0;
            _chkYonetici.Checked = false;
            _chkOfisCalisani.Checked = false;
            _chkWebServisTeklifleri.Checked = false;
            _chkSessionKaydet.Checked = false;
            _chkSirketBilgileriGizle.Checked = false;
            _lstKullanicilar.ClearSelected();
            RefreshYasakliSirketler();
        }

        private async Task HandleSaveUserAsync()
        {
            if (!_selectedUserId.HasValue)
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_userManagement.IsAdmin)
            {
                MessageBox.Show("Bu işlem için yönetici yetkisi gereklidir.", "Yetkisiz", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isSaving = true;
            _btnKaydet.Enabled = false;
            _btnKaydet.Text = "Kaydediliyor...";
            try
            {
                var okProfile = await _userManagement.UpdateUserProfileAsync(_selectedUserId.Value, _formData.AdSoyad, _formData.Gsm).ConfigureAwait(false);
                var dto = new UserSettingsDto
                {
                    UserId = _selectedUserId.Value,
                    Gsm = _formData.Gsm,
                    Description = _formData.Aciklama,
                    GroupName = _formData.GrupAdi,
                    IsOfficeWorker = _formData.OfisCalisan,
                    CanViewWebService = _formData.WebServisTeklifleri,
                    CanSaveSession = _formData.SessionKaydet,
                    HideCompanyDetails = _formData.SirketBilgileriGizle,
                    AllowedIps = _formData.IzinliIpList ?? new List<string>(),
                    BannedCompanies = _formData.BannedCompanies ?? new List<string>(),
                    CompanyRestrictions = _formData.CompanyRestrictions ?? new Dictionary<string, Dictionary<string, bool>>()
                };
                var okSettings = await _userManagement.SaveUserSettingsAsync(dto).ConfigureAwait(false);
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        _isSaving = false;
                        _btnKaydet.Enabled = true;
                        _btnKaydet.Text = "Kaydet";
                        MessageBox.Show(okProfile && okSettings ? "Kullanıcı ayarları kaydedildi." : "Kayıt sırasında hata oluştu.", okProfile && okSettings ? "Bilgi" : "Hata", MessageBoxButtons.OK, okProfile && okSettings ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                    }));
                }
                else
                {
                    _isSaving = false;
                    _btnKaydet.Enabled = true;
                    _btnKaydet.Text = "Kaydet";
                    MessageBox.Show(okProfile && okSettings ? "Kullanıcı ayarları kaydedildi." : "Kayıt sırasında hata oluştu.", okProfile && okSettings ? "Bilgi" : "Hata", MessageBoxButtons.OK, okProfile && okSettings ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                }
            }
            finally
            {
                if (_isSaving && _btnKaydet != null)
                {
                    _isSaving = false;
                    _btnKaydet.Enabled = true;
                    _btnKaydet.Text = "Kaydet";
                }
            }
        }

        private sealed class UserDisplayItem
        {
            public UserDataDto Dto { get; }
            public string DisplayName => Dto?.FullName ?? Dto?.Email ?? "-";
            public UserDisplayItem(UserDataDto dto) { Dto = dto; }
            public override string ToString() => DisplayName;
        }
    }
}
