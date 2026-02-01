using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Windows;
using VegaAsis.Windows.Robot;

namespace VegaAsis.Windows.Forms
{
    public class SirketlerRobotForm : Form
    {
        private static readonly Color VegaGreen = Color.FromArgb(46, 125, 50);
        private static readonly Color VegaRed = Color.FromArgb(198, 40, 40);
        private static readonly Color VegaOrange = Color.FromArgb(239, 108, 0);

        private readonly List<CompanyInfo> _companies = new List<CompanyInfo>
        {
            new CompanyInfo { Id = "anadolu", Name = "ANADOLU", Url = "https://www.anadolusigorta.com.tr", Color = Color.FromArgb(30, 64, 175) },
            new CompanyInfo { Id = "ana", Name = "ANA", Url = "https://www.anasigorta.com.tr", Color = Color.FromArgb(3, 105, 161) },
            new CompanyInfo { Id = "allianz", Name = "ALLIANZ", Url = "https://www.allianz.com.tr", Color = Color.FromArgb(0, 55, 129) },
            new CompanyInfo { Id = "ak", Name = "AK", Url = "https://sat2.aksigorta.com.tr/auth/log", Color = Color.FromArgb(196, 30, 58) }
        };

        private TabControl _tabControl;
        private Panel _toolbarPanel;
        private TextBox _txtUrl;
        private Panel _contentPanel;
        private Panel _statusPanel;
        private Label _lblDurum;
        private string _activeCompanyId = "ak";
        private bool _isRunning;
        private bool _isPaused;
        private IBrowserDriver _browserDriver;

        private class CompanyInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public Color Color { get; set; }
        }

        public SirketlerRobotForm()
        {
            Text = "Şirketler / Robot";
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 500);

            BuildCompanyBar();
            BuildTabBar();
            BuildToolbar();
            BuildContentPanel();
            BuildStatusBar();
            FormClosing += SirketlerRobotForm_FormClosing;
        }

        private void SirketlerRobotForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_browserDriver != null)
            {
                try { _browserDriver.Dispose(); } catch { }
                _browserDriver = null;
            }
        }

        private void BuildCompanyBar()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 36,
                Padding = new Padding(6, 4, 6, 4),
                BackColor = Color.FromArgb(245, 245, 245)
            };
            foreach (var c in _companies)
            {
                var btn = new Button
                {
                    Text = " " + c.Name,
                    Tag = c,
                    Width = 95,
                    Height = 26,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = panel.BackColor;
                btn.Click += (s, e) => SelectCompany((CompanyInfo)((Button)s).Tag);
                panel.Controls.Add(btn);
            }
            Controls.Add(panel);
        }

        private void BuildTabBar()
        {
            _tabControl = new TabControl
            {
                Dock = DockStyle.Top,
                Height = 32,
                Appearance = TabAppearance.FlatButtons,
                SizeMode = TabSizeMode.Fixed,
                ItemSize = new Size(120, 28),
                Padding = Point.Empty
            };
            AddNewTab("ak", "AK");
            _tabControl.SelectedIndexChanged += (s, e) => OnTabChanged();
            Controls.Add(_tabControl);
        }

        private void AddNewTab(string companyId, string companyName)
        {
            var tab = new TabPage(companyName) { Tag = companyId };
            _tabControl.TabPages.Add(tab);
            _tabControl.SelectedTab = tab;
        }

        private void OnTabChanged()
        {
            if (_tabControl.SelectedTab?.Tag is string id)
            {
                _activeCompanyId = id;
                var c = GetCompany(id);
                if (c != null)
                {
                    _txtUrl.Text = c.Url;
                    UpdateStatusLabel(c.Name);
                    var browser = _contentPanel?.Controls.Count > 0 ? _contentPanel.Controls[0] as WebBrowser : null;
                    if (browser != null)
                    {
                        try { browser.Navigate(c.Url); }
                        catch { }
                    }
                }
            }
        }

        private void UpdateStatusLabel(string companyName)
        {
            if (_lblDurum != null)
            {
                var durum = _isRunning ? (_isPaused ? "Duraklatıldı" : "Çalışıyor") : "Bekliyor";
                _lblDurum.Text = "Aktif Şirket: " + companyName + " | Durum: " + durum;
            }
        }

        private CompanyInfo GetCompany(string id)
        {
            foreach (var c in _companies)
            {
                if (c.Id == id) return c;
            }
            return null;
        }

        private void SelectCompany(CompanyInfo company)
        {
            for (int i = 0; i < _tabControl.TabPages.Count; i++)
            {
                if (_tabControl.TabPages[i].Tag as string == company.Id)
                {
                    _tabControl.SelectedIndex = i;
                    return;
                }
            }
            AddNewTab(company.Id, company.Name);
        }

        private void BuildToolbar()
        {
            _toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(6, 4, 6, 4),
                BackColor = Color.FromArgb(250, 250, 250)
            };

            int x = 8;
            var btnGeri = CreateToolButton("◀", ref x, 28);
            var btnIleri = CreateToolButton("▶", ref x, 28);
            var btnYenile = CreateToolButton("↻", ref x, 28);
            var btnHome = CreateToolButton("⌂", ref x, 28);
            x += 8;
            _txtUrl = new TextBox
            {
                Left = x,
                Top = 8,
                Width = 350,
                Height = 24,
                ReadOnly = true,
                BackColor = Color.White
            };
            x += 358;

            var btnGiris = new Button { Text = "Giriş", Left = x, Top = 6, Width = 60, Height = 26, FlatStyle = FlatStyle.Flat };
            x += 66;
            var btnBaslat = new Button { Text = "▶ Başlat", Left = x, Top = 6, Width = 85, Height = 26, FlatStyle = FlatStyle.Flat, ForeColor = VegaGreen };
            btnBaslat.Click += (s, e) => ShowBaslatMenu(btnBaslat);
            x += 91;
            var btnDuraklat = new Button { Text = "⏸ Duraklat", Left = x, Top = 6, Width = 85, Height = 26, FlatStyle = FlatStyle.Flat, ForeColor = VegaOrange };
            btnDuraklat.Click += (s, e) => { _isPaused = true; MessageBox.Show("Robot duraklatıldı.", "Bilgi", MessageBoxButtons.OK); };
            x += 91;
            var btnDurdur = new Button { Text = "■ Durdur", Left = x, Top = 6, Width = 75, Height = 26, FlatStyle = FlatStyle.Flat, ForeColor = VegaRed };
            btnDurdur.Click += (s, e) => DurdurBrowser();
            x += 81;
            var btnYeni = new Button { Text = "+ Yeni", Left = x, Top = 6, Width = 55, Height = 26, FlatStyle = FlatStyle.Flat };
            btnYeni.Click += (s, e) =>
            {
                var c = GetCompany(_activeCompanyId);
                if (c != null) AddNewTab(c.Id, c.Name);
            };
            x += 61;

            var btnPoliceKaydet = new Button { Text = "Poliçe Kaydet", Left = x, Top = 6, Width = 95, Height = 26, FlatStyle = FlatStyle.Flat };
            btnPoliceKaydet.Click += (s, e) => AcPoliceKaydetDialog();
            x += 101;
            var btnHataBildir = new Button { Text = "Hata Bildir", Left = x, Top = 6, Width = 85, Height = 26, FlatStyle = FlatStyle.Flat, ForeColor = VegaOrange };
            btnHataBildir.Click += (s, e) => AcHataBildirDialog();

            _toolbarPanel.Controls.AddRange(new Control[] { btnGeri, btnIleri, btnYenile, btnHome, _txtUrl, btnGiris, btnBaslat, btnDuraklat, btnDurdur, btnYeni, btnPoliceKaydet, btnHataBildir });
            Controls.Add(_toolbarPanel);

            var c0 = GetCompany(_activeCompanyId);
            if (c0 != null) _txtUrl.Text = c0.Url;
        }

        private Button CreateToolButton(string text, ref int x, int w)
        {
            var btn = new Button { Text = text, Left = x, Top = 6, Width = w, Height = 26, FlatStyle = FlatStyle.Flat };
            x += w + 2;
            return btn;
        }

        private void ShowBaslatMenu(Control owner)
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Chrome ile Aç", null, (s, e) => BaslatChrome());
            menu.Items.Add("Tümüne Giriş (sıralı)", null, (s, e) => TumuneGiris());
            menu.Items.Add("Tümünden Teklif Al", null, (s, e) => TumundenTeklifAl());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Trafik Sorgu", null, (s, e) => { _isRunning = true; _isPaused = false; MessageBox.Show("Trafik sorgu başlatıldı.", "Bilgi"); });
            menu.Items.Add("Kasko Sorgu", null, (s, e) => { _isRunning = true; _isPaused = false; MessageBox.Show("Kasko sorgu başlatıldı.", "Bilgi"); });
            menu.Items.Add("Tss Sorgu", null, (s, e) => { _isRunning = true; _isPaused = false; MessageBox.Show("Tss sorgu başlatıldı.", "Bilgi"); });
            menu.Show(owner, new Point(0, owner.Height));
        }

        private void DurdurBrowser()
        {
            _isRunning = false;
            _isPaused = false;
            if (_browserDriver != null)
            {
                try { _browserDriver.Dispose(); }
                catch { }
                _browserDriver = null;
            }
            UpdateStatusLabel(GetCompany(_activeCompanyId)?.Name ?? "?");
            MessageBox.Show("Robot durduruldu.", "Bilgi", MessageBoxButtons.OK);
        }

        private async void TumuneGiris()
        {
            if (_browserDriver == null)
            {
                try { _browserDriver = new ChromeBrowserDriver(headless: false); }
                catch (Exception ex)
                {
                    MessageBox.Show("Chrome başlatılamadı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            _isRunning = true;
            List<string> companyIds = null;
            try
            {
                if (ServiceLocator.IsInitialized)
                {
                    var settings = ServiceLocator.Resolve<ICompanySettingsService>();
                    if (settings != null) companyIds = await settings.GetSelectedCompaniesAsync().ConfigureAwait(true);
                }
            }
            catch { /* seçili şirket servisi yoksa _companies kullanılır */ }
            if (companyIds == null || companyIds.Count == 0)
                companyIds = _companies.Select(c => c.Id).ToList();
            var results = await AllLoginsRunner.RunAsync(_browserDriver, companyIds).ConfigureAwait(true);
            _isRunning = false;
            var msg = results.Count > 0 ? string.Join("\r\n", results.Select(r => r.CompanyName + ": " + (r.Success ? "OK" : r.Message))) : "Şirket listesi boş.";
            MessageBox.Show("Tümüne Giriş özeti:\r\n" + msg, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void TumundenTeklifAl()
        {
            if (_browserDriver == null)
            {
                try { _browserDriver = new ChromeBrowserDriver(headless: false); }
                catch (Exception ex)
                {
                    MessageBox.Show("Chrome başlatılamadı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            List<string> companyIds = null;
            try
            {
                if (ServiceLocator.IsInitialized)
                {
                    var settings = ServiceLocator.Resolve<ICompanySettingsService>();
                    if (settings != null) companyIds = await settings.GetSelectedCompaniesAsync().ConfigureAwait(true);
                }
            }
            catch { }
            if (companyIds == null || companyIds.Count == 0)
                companyIds = _companies.Select(c => c.Id).ToList();
            var offerParams = new { Plaka = "34ABC123", Tckn = "12345678901" };
            _isRunning = true;
            var results = await AllOffersRunner.RunAsync(_browserDriver, companyIds, offerParams).ConfigureAwait(true);
            _isRunning = false;
            var msg = results.Count > 0 ? string.Join("\r\n", results.Select(r => r.CompanyName + ": " + (r.LoginSuccess ? r.OfferResult : r.OfferResult))) : "Şirket listesi boş.";
            MessageBox.Show("Tümünden Teklif Al özeti:\r\n" + msg, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BaslatChrome()
        {
            var c = GetCompany(_activeCompanyId);
            if (c == null || string.IsNullOrWhiteSpace(c.Url))
            {
                MessageBox.Show("Aktif şirket veya URL bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_browserDriver != null)
            {
                try { _browserDriver.Dispose(); } catch { }
                _browserDriver = null;
            }
            try
            {
                _browserDriver = new ChromeBrowserDriver(headless: false);
                var robot = CompanyRobotRegistry.GetRobot(c.Id);
                if (robot != null)
                {
                    var ok = await robot.LoginAsync(_browserDriver).ConfigureAwait(true);
                    _isRunning = true;
                    UpdateStatusLabel(c.Name);
                    MessageBox.Show(ok ? "Chrome açıldı: " + c.Name : "Chrome açıldı; giriş adımı tamamlandı veya beklemede.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _browserDriver.Navigate(c.Url);
                    _isRunning = true;
                    UpdateStatusLabel(c.Name);
                    MessageBox.Show("Chrome açıldı: " + c.Name + "\nURL: " + c.Url, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chrome başlatılamadı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AcPoliceKaydetDialog()
        {
            using (var f = new Form
            {
                Text = "Poliçe Kaydet",
                Size = new Size(400, 380),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            })
            {
                int y = 20;
                AddFormRow(f, "Poliçe Türü:", new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList }, ref y);
                ((ComboBox)f.Controls[f.Controls.Count - 1]).Items.AddRange(new[] { "TRAFİK", "KASKO", "TSS", "DASK" });
                ((ComboBox)f.Controls[f.Controls.Count - 1]).SelectedIndex = 0;
                AddFormRow(f, "Sigorta Şirketi:", new TextBox { Width = 200, Text = "AK" }, ref y);
                AddFormRow(f, "Poliçe No:", new TextBox { Width = 200 }, ref y);
                AddFormRow(f, "TCKN/Vergi No:", new TextBox { Width = 200 }, ref y);
                AddFormRow(f, "Plaka:", new TextBox { Width = 200 }, ref y);
                AddFormRow(f, "Brüt Prim:", new TextBox { Width = 200, Text = "0.00" }, ref y);
                AddFormRow(f, "Açıklama:", new TextBox { Width = 200, Height = 50, Multiline = true }, ref y);
                var btnKaydet = new Button { Text = "Kaydet", Width = 80, Top = y + 10, Left = 120 };
                btnKaydet.Click += (s, ev) => { MessageBox.Show("Poliçe kaydedildi.", "Bilgi"); f.DialogResult = DialogResult.OK; f.Close(); };
                f.Controls.Add(btnKaydet);
                f.ShowDialog(this);
            }
        }

        private void AcHataBildirDialog()
        {
            using (var f = new Form
            {
                Text = "Hata Bildir",
                Size = new Size(420, 320),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            })
            {
                int y = 20;
                AddFormRow(f, "Teklif ID:", new TextBox { Width = 200, Text = "0" }, ref y);
                AddFormRow(f, "Plaka:", new TextBox { Width = 200 }, ref y);
                AddFormRow(f, "Açıklama:", new TextBox { Width = 200, Height = 60, Multiline = true }, ref y);
                AddFormRow(f, "Telefon:", new TextBox { Width = 200 }, ref y);
                var btnGonder = new Button { Text = "Gönder", Width = 80, Top = y + 10, Left = 140 };
                btnGonder.Click += (s, ev) => { MessageBox.Show("Hata bildirimi gönderildi.", "Bilgi"); f.DialogResult = DialogResult.OK; f.Close(); };
                f.Controls.Add(btnGonder);
                f.ShowDialog(this);
            }
        }

        private void AddFormRow(Form parent, string label, Control control, ref int y)
        {
            var lbl = new Label { Text = label, Left = 20, Top = y, Width = 120 };
            control.Left = 150;
            control.Top = y - 2;
            parent.Controls.Add(lbl);
            parent.Controls.Add(control);
            y += Math.Max(control.Height, 24) + 10;
        }

        private void BuildContentPanel()
        {
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0)
            };
            try
            {
                var browser = new WebBrowser
                {
                    Dock = DockStyle.Fill,
                    ScriptErrorsSuppressed = true
                };
                var c = GetCompany(_activeCompanyId);
                if (c != null) browser.Navigate(c.Url);
                _contentPanel.Controls.Add(browser);
            }
            catch
            {
                var lbl = new Label
                {
                    Text = "Tarayıcı yüklenemedi. WebBrowser kontrolü kullanılamıyor olabilir.",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.Gray
                };
                _contentPanel.Controls.Add(lbl);
            }
            Controls.Add(_contentPanel);
        }

        private void BuildStatusBar()
        {
            _statusPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 26,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(8, 4, 8, 4)
            };
            _lblDurum = new Label
            {
                Text = "Aktif Şirket: AK | Durum: Bekliyor",
                AutoSize = true,
                Left = 8,
                Top = 5
            };
            _statusPanel.Controls.Add(_lblDurum);
            Controls.Add(_statusPanel);
        }
    }
}
