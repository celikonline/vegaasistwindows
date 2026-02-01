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
    public class SirketAyarlariControl : UserControl
    {
        private readonly ICompanySettingsService _companySettingsService;

        private ListBox _lstSirketler;
        private TextBox _txtSirketAdi;
        private TextBox _txtKullaniciAdi;
        private TextBox _txtSifre;
        private TextBox _txtProxyAdresi;
        private CheckBox _chkAktif;
        private CheckBox _chkYasakli;
        private Button _btnKaydet;

        private string _selectedCompanyName;
        private CompanySettingsDto _currentSettings;
        private SplitContainer _mainSplit;
        private bool _splitterInitialized;

        public SirketAyarlariControl(ICompanySettingsService companySettingsService)
        {
            _companySettingsService = companySettingsService ?? throw new ArgumentNullException(nameof(companySettingsService));
            MinimumSize = new Size(600, 400);
            Dock = DockStyle.Fill;
            Padding = new Padding(8);
            BackColor = SystemColors.Control;
            InitializeComponents();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await _companySettingsService.LoadSettingsAsync().ConfigureAwait(false);
            var companies = await _companySettingsService.GetSelectedCompaniesAsync().ConfigureAwait(false);

            if (InvokeRequired)
            {
                Invoke(new Action(() => RefreshCompanyList(companies)));
            }
            else
            {
                RefreshCompanyList(companies);
            }
        }

        private void RefreshCompanyList(List<string> companies)
        {
            _lstSirketler.Items.Clear();
            foreach (var company in companies.OrderBy(c => c))
            {
                _lstSirketler.Items.Add(company);
            }

            if (_lstSirketler.Items.Count > 0 && _lstSirketler.SelectedIndex < 0)
            {
                _lstSirketler.SelectedIndex = 0;
            }
        }

        private void InitializeComponents()
        {
            _mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Panel1MinSize = 0,
                Panel2MinSize = 0,
                SplitterDistance = 0,
                BackColor = SystemColors.Control
            };
            _mainSplit.Resize += SirketAyarlariSplit_Resize;

            // Sol Panel - Şirket Listesi
            var leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Padding = new Padding(4)
            };

            var lblSirketler = new Label
            {
                Text = "Şirketler",
                AutoSize = true,
                Location = new Point(6, 6),
                Font = new Font(Font.FontFamily, 9, FontStyle.Bold)
            };

            _lstSirketler = new ListBox
            {
                Bounds = new Rectangle(6, 28, 188, 350),
                Font = new Font(Font.FontFamily, 9)
            };
            _lstSirketler.SelectedIndexChanged += LstSirketler_SelectedIndexChanged;

            leftPanel.Controls.Add(lblSirketler);
            leftPanel.Controls.Add(_lstSirketler);
            _mainSplit.Panel1.Controls.Add(leftPanel);

            // Sağ Panel - Şirket Bilgileri
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Padding = new Padding(8),
                AutoScroll = true
            };

            var grpSirketBilgileri = new GroupBox
            {
                Text = "Şirket Bilgileri",
                Dock = DockStyle.Fill,
                Font = new Font(Font.FontFamily, 9, FontStyle.Bold),
                Padding = new Padding(8)
            };

            int y = 24;
            y = AddLabelAndTextBox(grpSirketBilgileri, "Şirket Adı", y, out _txtSirketAdi, true);
            y = AddLabelAndTextBox(grpSirketBilgileri, "Kullanıcı Adı", y, out _txtKullaniciAdi);
            y = AddLabelAndTextBox(grpSirketBilgileri, "Şifre", y, out _txtSifre, false, true);
            y = AddLabelAndTextBox(grpSirketBilgileri, "Proxy Adresi", y, out _txtProxyAdresi);

            _chkAktif = new CheckBox
            {
                Text = "Aktif",
                Location = new Point(8, y),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 9)
            };
            grpSirketBilgileri.Controls.Add(_chkAktif);
            y += 26;

            _chkYasakli = new CheckBox
            {
                Text = "Yasaklı",
                Location = new Point(8, y),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 9)
            };
            grpSirketBilgileri.Controls.Add(_chkYasakli);
            y += 40;

            _btnKaydet = new Button
            {
                Text = "Kaydet",
                Size = new Size(100, 32),
                Location = new Point(8, y),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnKaydet.Click += async (s, e) => await HandleSaveAsync();
            grpSirketBilgileri.Controls.Add(_btnKaydet);

            rightPanel.Controls.Add(grpSirketBilgileri);
            _mainSplit.Panel2.Controls.Add(rightPanel);

            Controls.Add(_mainSplit);
        }

        private void SirketAyarlariSplit_Resize(object sender, EventArgs e)
        {
            if (_splitterInitialized) return;
            const int panel1Min = 150;
            const int panel2Min = 300;
            int w = _mainSplit.Width;
            if (w < panel1Min + panel2Min) return;
            _splitterInitialized = true;
            _mainSplit.Resize -= SirketAyarlariSplit_Resize;
            _mainSplit.Panel1MinSize = panel1Min;
            _mainSplit.Panel2MinSize = panel2Min;
            _mainSplit.SplitterDistance = Math.Max(panel1Min, Math.Min(200, w - panel2Min));
        }

        private int AddLabelAndTextBox(Control parent, string labelText, int y, out TextBox txt, bool isReadOnly = false, bool isPassword = false)
        {
            var lbl = new Label
            {
                Text = labelText + ":",
                Location = new Point(8, y),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 9)
            };
            txt = new TextBox
            {
                Bounds = new Rectangle(120, y - 2, 250, 21),
                Font = new Font(Font.FontFamily, 9),
                ReadOnly = isReadOnly,
                UseSystemPasswordChar = isPassword
            };
            if (isPassword)
            {
                txt.PasswordChar = '*';
            }
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            return y + 28;
        }

        private void LstSirketler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lstSirketler.SelectedItem == null)
            {
                ClearForm();
                return;
            }

            _selectedCompanyName = _lstSirketler.SelectedItem.ToString();
            LoadCompanySettings(_selectedCompanyName);
        }

        private void LoadCompanySettings(string companyName)
        {
            _currentSettings = _companySettingsService.GetCompanySetting(companyName);

            if (_currentSettings == null)
            {
                _currentSettings = new CompanySettingsDto
                {
                    CompanyName = companyName
                };
            }

            _txtSirketAdi.Text = _currentSettings.CompanyName ?? "";
            _txtKullaniciAdi.Text = _currentSettings.CompanyUsername ?? "";
            _txtSifre.Text = _currentSettings.CompanyPassword ?? "";
            _txtProxyAdresi.Text = _currentSettings.ProxyAddress ?? "";

            // Aktif ve Yasaklı durumları için varsayılan değerler
            // Bu property'ler DTO'da yoksa, başka bir kaynaktan alınabilir veya varsayılan olarak true/false yapılabilir
            _chkAktif.Checked = true; // Varsayılan olarak aktif
            _chkYasakli.Checked = _currentSettings.CompanyBans != null && _currentSettings.CompanyBans.ContainsKey(companyName) && _currentSettings.CompanyBans[companyName];
        }

        private void ClearForm()
        {
            _selectedCompanyName = null;
            _currentSettings = null;
            _txtSirketAdi.Text = "";
            _txtKullaniciAdi.Text = "";
            _txtSifre.Text = "";
            _txtProxyAdresi.Text = "";
            _chkAktif.Checked = false;
            _chkYasakli.Checked = false;
        }

        private async Task HandleSaveAsync()
        {
            if (string.IsNullOrEmpty(_selectedCompanyName))
            {
                MessageBox.Show("Lütfen bir şirket seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_currentSettings == null)
            {
                _currentSettings = new CompanySettingsDto
                {
                    CompanyName = _selectedCompanyName
                };
            }

            _currentSettings.CompanyName = _txtSirketAdi.Text.Trim();
            _currentSettings.CompanyUsername = _txtKullaniciAdi.Text.Trim();
            _currentSettings.CompanyPassword = _txtSifre.Text.Trim();
            _currentSettings.ProxyAddress = _txtProxyAdresi.Text.Trim();

            // CompanyBans dictionary'sini güncelle
            if (_currentSettings.CompanyBans == null)
            {
                _currentSettings.CompanyBans = new Dictionary<string, bool>();
            }
            _currentSettings.CompanyBans[_selectedCompanyName] = _chkYasakli.Checked;

            _btnKaydet.Enabled = false;
            _btnKaydet.Text = "Kaydediliyor...";

            try
            {
                var success = await _companySettingsService.SaveCompanySettingAsync(_currentSettings).ConfigureAwait(false);

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        _btnKaydet.Enabled = true;
                        _btnKaydet.Text = "Kaydet";
                        MessageBox.Show(
                            success ? "Şirket ayarları kaydedildi." : "Kayıt sırasında hata oluştu.",
                            success ? "Bilgi" : "Hata",
                            MessageBoxButtons.OK,
                            success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                    }));
                }
                else
                {
                    _btnKaydet.Enabled = true;
                    _btnKaydet.Text = "Kaydet";
                    MessageBox.Show(
                        success ? "Şirket ayarları kaydedildi." : "Kayıt sırasında hata oluştu.",
                        success ? "Bilgi" : "Hata",
                        MessageBoxButtons.OK,
                        success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        _btnKaydet.Enabled = true;
                        _btnKaydet.Text = "Kaydet";
                        MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                else
                {
                    _btnKaydet.Enabled = true;
                    _btnKaydet.Text = "Kaydet";
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
