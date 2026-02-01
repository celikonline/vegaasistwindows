using System;
using System.Drawing;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Windows.UserControls;

namespace VegaAsis.Windows.Forms
{
    public class AdminPanelForm : Form
    {
        private readonly ICompanySettingsService _companySettingsService;
        private readonly IUserManagementService _userManagementService;
        private readonly IAuthService _authService;

        public AdminPanelForm(
            ICompanySettingsService companySettingsService,
            IUserManagementService userManagementService,
            IAuthService authService)
        {
            _companySettingsService = companySettingsService ?? throw new ArgumentNullException(nameof(companySettingsService));
            _userManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            Text = "Yönetim Paneli";
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 500);

            var tabControl = new TabControl { Dock = DockStyle.Fill };

            // Şirket Ayarları Tab
            var tabSirketAyarlari = new TabPage("Şirket Ayarları");
            var sirketAyarlariControl = new SirketAyarlariControl(_companySettingsService) { Dock = DockStyle.Fill };
            tabSirketAyarlari.Controls.Add(sirketAyarlariControl);
            tabControl.TabPages.Add(tabSirketAyarlari);

            // Kullanıcı Ayarları Tab
            var tabKullaniciAyarlari = new TabPage("Kullanıcı Ayarları");
            var kullaniciAyarlariControl = new KullaniciAyarlariUserControl(_userManagementService, _companySettingsService, _authService) { Dock = DockStyle.Fill };
            tabKullaniciAyarlari.Controls.Add(kullaniciAyarlariControl);
            tabControl.TabPages.Add(tabKullaniciAyarlari);

            // Paylaşılan Şirketler Tab
            var sharedCompanyService = VegaAsis.Windows.ServiceLocator.IsInitialized ? VegaAsis.Windows.ServiceLocator.Resolve<ISharedCompanyService>() : null;
            var tabPaylasilanSirketler = new TabPage("Paylaşılan Şirketler");
            var paylasilanSirketlerControl = new PaylasilanSirketlerControl(sharedCompanyService, _companySettingsService, _authService) { Dock = DockStyle.Fill };
            tabPaylasilanSirketler.Controls.Add(paylasilanSirketlerControl);
            tabControl.TabPages.Add(tabPaylasilanSirketler);

            // WEB Ekranları Tab
            var webUserService = VegaAsis.Windows.ServiceLocator.IsInitialized ? VegaAsis.Windows.ServiceLocator.Resolve<IWebUserService>() : null;
            var tabWebEkranlari = new TabPage("WEB Ekranları");
            var webEkranlariControl = new WebEkranlariControl(webUserService, _authService) { Dock = DockStyle.Fill };
            tabWebEkranlari.Controls.Add(webEkranlariControl);
            tabControl.TabPages.Add(tabWebEkranlari);

            // Diğer Ayarlar Tab
            var appSettingsService = VegaAsis.Windows.ServiceLocator.IsInitialized ? VegaAsis.Windows.ServiceLocator.Resolve<IAppSettingsService>() : null;
            var tabDigerAyarlar = new TabPage("Diğer Ayarlar");
            var digerAyarlarControl = new DigerAyarlarControl(appSettingsService) { Dock = DockStyle.Fill };
            tabDigerAyarlar.Controls.Add(digerAyarlarControl);
            tabControl.TabPages.Add(tabDigerAyarlar);

            // Teklifler Tab
            var tabTeklifler = new TabPage("Teklifler");
            var tekliflerAyarControl = new TekliflerAyarControl() { Dock = DockStyle.Fill };
            tabTeklifler.Controls.Add(tekliflerAyarControl);
            tabControl.TabPages.Add(tabTeklifler);

            // Gruplar Tab
            var userGroupService = VegaAsis.Windows.ServiceLocator.IsInitialized ? VegaAsis.Windows.ServiceLocator.Resolve<IUserGroupService>() : null;
            var tabGruplar = new TabPage("Gruplar");
            var gruplarControl = new GruplarControl(userGroupService, _userManagementService, _authService) { Dock = DockStyle.Fill };
            tabGruplar.Controls.Add(gruplarControl);
            tabControl.TabPages.Add(tabGruplar);

            // Benchmark Tab
            var tabBenchmark = new TabPage("Benchmark");
            var benchmarkControl = new BenchmarkControl() { Dock = DockStyle.Fill };
            tabBenchmark.Controls.Add(benchmarkControl);
            tabControl.TabPages.Add(tabBenchmark);

            // Kota Ayarları Tab
            var quotaSettingsService = VegaAsis.Windows.ServiceLocator.IsInitialized ? VegaAsis.Windows.ServiceLocator.Resolve<IQuotaSettingsService>() : null;
            var tabKotaAyarlari = new TabPage("Kota Ayarları");
            var kotaAyarlariControl = new KotaAyarlariControl(quotaSettingsService, _userManagementService) { Dock = DockStyle.Fill };
            tabKotaAyarlari.Controls.Add(kotaAyarlariControl);
            tabControl.TabPages.Add(tabKotaAyarlari);

            Controls.Add(tabControl);
        }

        private static TabPage CreatePlaceholderTab(string title, string content)
        {
            var tab = new TabPage(title);
            var lbl = new Label
            {
                Text = content,
                AutoSize = true,
                Left = 16,
                Top = 16
            };
            tab.Controls.Add(lbl);
            return tab;
        }
    }
}
