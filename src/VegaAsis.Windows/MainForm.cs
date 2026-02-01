using System;
using System.Drawing;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Windows.Forms;
using VegaAsis.Windows.Models;

namespace VegaAsis.Windows
{
    public class MainForm : Form
    {
        private readonly IAuthService _authService;
        private readonly IOfferService _offerService;
        private readonly IPolicyService _policyService;
        private readonly IAppointmentService _appointmentService;
        private readonly ICompanySettingsService _companySettingsService;
        private readonly IUserManagementService _userManagementService;
        private Panel _contentPanel;

        public MainForm(
            IAuthService authService,
            IOfferService offerService,
            IPolicyService policyService,
            IAppointmentService appointmentService,
            ICompanySettingsService companySettingsService,
            IUserManagementService userManagementService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _offerService = offerService ?? throw new ArgumentNullException(nameof(offerService));
            _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _companySettingsService = companySettingsService ?? throw new ArgumentNullException(nameof(companySettingsService));
            _userManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
            Text = "VegaAsis";
            Size = new Size(1024, 768);
            MinimumSize = new Size(800, 600);

            var menuStrip = new MenuStrip();
            var mnuDosya = new ToolStripMenuItem("Dosya");
            var mnuCikis = new ToolStripMenuItem("Çıkış");
            mnuCikis.Click += (s, e) => { _authService.Logout(); Close(); };
            mnuDosya.DropDownItems.Add(mnuCikis);

            var mnuYonetim = new ToolStripMenuItem("Yönetim");
            var mnuAdminPanel = new ToolStripMenuItem("Yönetim Paneli");
            mnuAdminPanel.ShortcutKeys = Keys.Control | Keys.P;
            mnuAdminPanel.Click += (s, e) => OpenForm(new AdminPanelForm(_companySettingsService, _userManagementService, _authService));
            var mnuGenelAyarlar = new ToolStripMenuItem("Genel Ayarlar");
            mnuGenelAyarlar.Click += (s, e) => OpenForm(new ConfigForm());
            mnuYonetim.DropDownItems.Add(mnuAdminPanel);
            mnuYonetim.DropDownItems.Add(mnuGenelAyarlar);

            var mnuSayfalar = new ToolStripMenuItem("Sayfalar");
            var mnuAnaSayfa = new ToolStripMenuItem("Ana Sayfa");
            var mnuTeklifler = new ToolStripMenuItem("Teklifler");
            var mnuPolicelerim = new ToolStripMenuItem("Policelerim");
            var mnuAjanda = new ToolStripMenuItem("Ajanda");
            var mnuSirketlerRobot = new ToolStripMenuItem("Şirketler / Robot");
            var mnuRaporlar = new ToolStripMenuItem("Raporlar");
            var mnuDestek = new ToolStripMenuItem("Destek Talepleri");
            var mnuDuyurular = new ToolStripMenuItem("Duyurular");
            var mnuCanliUretim = new ToolStripMenuItem("Canlı Üretim");
            var mnuCanliDestek = new ToolStripMenuItem("Canlı Destek");
            var mnuPoliceNoGit = new ToolStripMenuItem("Poliçe No ile Git");
            var mnuTeklifNoGit = new ToolStripMenuItem("Teklif No ile Git");
            var mnuExcelOku = new ToolStripMenuItem("Excel Oku");
            var mnuOncekiPolice = new ToolStripMenuItem("Önceki Poliçe");
            var mnuCokluFiyat = new ToolStripMenuItem("Çoklu Fiyat Karşılaştırma");
            var mnuManuelUavt = new ToolStripMenuItem("Manuel UAVT Sorgusu");
            var mnuTramerSorgu = new ToolStripMenuItem("Tramer Sorgusu");
            var mnuBildirimler = new ToolStripMenuItem("Bildirimler");
            var mnuSablonDuzenle = new ToolStripMenuItem("Şablon Düzenle");
            var mnuWSTeklifSorgula = new ToolStripMenuItem("WS Tekliflerini Sorgula");

            mnuAnaSayfa.ShortcutKeys = Keys.Control | Keys.H;
            mnuAnaSayfa.Click += (s, e) => ShowIndexView();
            mnuTeklifler.ShortcutKeys = Keys.Control | Keys.T;
            mnuTeklifler.Click += (s, e) => OpenForm(new TekliflerForm(_authService, _offerService));
            mnuPolicelerim.ShortcutKeys = Keys.Control | Keys.L;
            mnuPolicelerim.Click += (s, e) => OpenForm(new PolicelerimForm(_policyService));
            mnuAjanda.ShortcutKeys = Keys.Control | Keys.J;
            mnuAjanda.Click += (s, e) => OpenForm(new AjandaYenilemeForm(_appointmentService));
            mnuSirketlerRobot.Click += (s, e) => OpenForm(new SirketlerRobotForm());
            mnuRaporlar.ShortcutKeys = Keys.Control | Keys.R;
            mnuRaporlar.Click += (s, e) => OpenForm(new RaporlarForm());
            mnuDestek.Click += (s, e) => OpenForm(new DestekTalepleriForm());
            mnuDuyurular.Click += (s, e) => OpenForm(new DuyurularForm());
            mnuCanliUretim.Click += (s, e) => OpenForm(new CanliUretimForm());
            mnuCanliDestek.Click += (s, e) => OpenForm(new CanliDestekForm());
            mnuPoliceNoGit.Click += (s, e) => OpenPoliceNoGit();
            mnuTeklifNoGit.Click += (s, e) => OpenTeklifNoGit();
            mnuExcelOku.Click += (s, e) => OpenForm(new ExcelOkuForm());
            mnuOncekiPolice.Click += (s, e) => OpenForm(new OncekiPoliceForm());
            mnuCokluFiyat.Click += (s, e) =>
            {
                var idx = _contentPanel.Controls.Count > 0 ? _contentPanel.Controls[0] as UserControls.IndexViewControl : null;
                var satirlar = idx?.GetFiyatSatirlari();
                OpenForm(new CokluFiyatForm(satirlar));
            };
            mnuManuelUavt.Click += (s, e) => OpenForm(new ManuelUavtSorguForm());
            mnuTramerSorgu.Click += (s, e) => OpenForm(new TramerSorguForm());
            mnuBildirimler.Click += (s, e) => OpenForm(new BildirimEkraniForm());
            mnuSablonDuzenle.Click += (s, e) => OpenForm(new SablonDuzenleForm());
            mnuWSTeklifSorgula.Click += (s, e) => OpenForm(new WSTeklifleriniSorgulaForm());

            mnuSayfalar.DropDownItems.AddRange(new ToolStripItem[] {
                mnuAnaSayfa, mnuTeklifler, mnuPolicelerim, mnuAjanda,
                mnuSirketlerRobot, mnuRaporlar, mnuDestek, mnuDuyurular,
                mnuCanliUretim, mnuCanliDestek, new ToolStripSeparator(),
                mnuExcelOku, mnuOncekiPolice, mnuCokluFiyat, mnuManuelUavt, mnuTramerSorgu, mnuBildirimler, mnuSablonDuzenle, mnuWSTeklifSorgula, mnuPoliceNoGit, mnuTeklifNoGit
            });

            menuStrip.Items.Add(mnuDosya);
            menuStrip.Items.Add(mnuSayfalar);
            menuStrip.Items.Add(mnuYonetim);
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            var toolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                ImageScalingSize = new Size(20, 20),
                Padding = new Padding(4, 2, 4, 2)
            };
            toolStrip.Items.Add(new ToolStripButton("Ana Sayfa", null, (s, e) => ShowIndexView()) { DisplayStyle = ToolStripItemDisplayStyle.Text });
            toolStrip.Items.Add(new ToolStripButton("Teklifler", null, (s, e) => OpenForm(new TekliflerForm(_authService, _offerService))) { DisplayStyle = ToolStripItemDisplayStyle.Text });
            toolStrip.Items.Add(new ToolStripButton("Policelerim", null, (s, e) => OpenForm(new PolicelerimForm(_policyService))) { DisplayStyle = ToolStripItemDisplayStyle.Text });
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("Yönetim Paneli", null, (s, e) => OpenForm(new AdminPanelForm(_companySettingsService, _userManagementService, _authService))) { DisplayStyle = ToolStripItemDisplayStyle.Text });
            toolStrip.Items.Add(new ToolStripButton("Şirket Seç", null, (s, e) => OpenSirketSecim()) { DisplayStyle = ToolStripItemDisplayStyle.Text });
            toolStrip.Items.Add(new ToolStripButton("Excel Oku", null, (s, e) => OpenForm(new ExcelOkuForm())) { DisplayStyle = ToolStripItemDisplayStyle.Text });
            Controls.Add(toolStrip);

            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };
            Controls.Add(_contentPanel);

            ShowIndexView();
        }

        private void ShowIndexView()
        {
            _contentPanel.Controls.Clear();
            var indexControl = new UserControls.IndexViewControl();
            indexControl.Dock = DockStyle.Fill;
            indexControl.SbmSorgusuRequested += (s, e) => OpenForm(new SbmSorgusuForm());
            indexControl.KuyrukSorgusuRequested += (s, e) => OpenForm(new KuyrukSorgusuForm());
            indexControl.WebcamQRRequested += (s, e) => OpenForm(new WebcamQRForm());
            indexControl.SirketEkleRequested += (s, e) => OpenSirketSecim();
            indexControl.AdminPanelRequested += (s, e) => OpenForm(new AdminPanelForm(_companySettingsService, _userManagementService, _authService));
            indexControl.TekliflerRequested += (s, e) => OpenForm(new TekliflerForm(_authService, _offerService));
            indexControl.PolicelerimRequested += (s, e) => OpenForm(new PolicelerimForm(_policyService));
            indexControl.SirketlerRobotRequested += (s, e) => OpenForm(new SirketlerRobotForm());
            indexControl.RaporlarRequested += (s, e) => OpenForm(new RaporlarForm());
            indexControl.DestekTalepleriRequested += (s, e) => OpenForm(new DestekTalepleriForm());
            indexControl.AjandaYenilemeRequested += (s, e) => OpenForm(new AjandaYenilemeForm(_appointmentService));
            indexControl.DuyurularRequested += (s, e) => OpenForm(new DuyurularForm());
            indexControl.CanliUretimRequested += (s, e) => OpenForm(new CanliUretimForm());
            indexControl.CanliDestekRequested += (s, e) => OpenForm(new CanliDestekForm());
            indexControl.BranchFormRequested += (s, branch) =>
            {
                if (branch == "TRAFİK") OpenForm(new TrafikTeklifiForm());
                else if (branch == "KASKO") OpenForm(new KaskoTeminatlariForm());
                else if (branch == "TSS") OpenForm(new TssDetaylariForm());
                else if (branch == "DASK" || branch == "KONUT") OpenForm(new DaskDetaylariForm());
                else if (branch == "İMM") OpenForm(new ImmTeminatlariForm());
            };
            indexControl.BranchCellClickRequested += (s, e) =>
            {
                var branch = e.Branch;
                var company = e.CompanyName;
                Form form = null;
                if (branch == "TRAFİK") form = new TrafikTeklifiForm();
                else if (branch == "KASKO") form = new KaskoTeminatlariForm();
                else if (branch == "SBM") form = new SbmSorgusuForm();
                else if (branch == "TSS") form = new TssDetaylariForm();
                else if (branch == "DASK" || branch == "KONUT") form = new DaskDetaylariForm();
                else if (branch == "İMM") form = new ImmTeminatlariForm();
                if (form != null)
                {
                    form.Text = string.IsNullOrEmpty(company) ? form.Text : form.Text + " - " + company;
                    OpenForm(form);
                }
            };
            indexControl.PdfExportRequested += (s, e) => OpenForm(new PDFExportForm());
            indexControl.PdfUploadRequested += (s, e) => OpenPdfUpload();
            indexControl.SorguBaslatRequested += (s, ev) =>
            {
                var idx = s as UserControls.IndexViewControl;
                if (idx == null) return;
                var session = idx.Session;
                var userId = _authService.GetCurrentUserId;
                if (!userId.HasValue) { MessageBox.Show("Giriş yapmalısınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                try
                {
                    var dto = new OfferDto
                    {
                        Plaka = session.Plaka,
                        TcVergi = session.TcVergi,
                        BelgeSeri = session.BelgeSeri,
                        Musteri = session.MusteriAdi,
                        Meslek = session.Meslek,
                        DogumTarihi = session.DogumTarihi,
                        PoliceTipi = session.AktifBrans ?? "TRAFİK",
                        Trf = (session.AktifBrans ?? "").IndexOf("TRAFİK", StringComparison.OrdinalIgnoreCase) >= 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _offerService.CreateAsync(dto, userId.Value).ConfigureAwait(true).GetAwaiter().GetResult();
                    MessageBox.Show("Teklif kaydı oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            indexControl.DuraklatRequested += (s, ev) => { };
            indexControl.YeniSorguKaydetRequested += (s, ev) => MessageBox.Show("Yeni sorgu hazır. Form alanları temizlendi.", "Yeni Sorgu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _contentPanel.Controls.Add(indexControl);
        }

        private void OpenForm(Form form)
        {
            form.ShowDialog(this);
        }

        private void OpenPoliceNoGit()
        {
            using (var form = new PoliceNoGitForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(form.PoliceNo))
                {
                    OpenForm(new PolicelerimForm(_policyService));
                    MessageBox.Show("Poliçe No: " + form.PoliceNo + "\n(Filtre entegrasyonu bekleniyor)", "Poliçe No ile Git", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void OpenTeklifNoGit()
        {
            using (var form = new TeklifNoGitForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(form.TeklifNo))
                {
                    OpenForm(new TekliflerForm(_authService, _offerService));
                    MessageBox.Show("Teklif No: " + form.TeklifNo + "\n(Filtre entegrasyonu bekleniyor)", "Teklif No ile Git", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void OpenPdfUpload()
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "PDF Dosyaları (*.pdf)|*.pdf|Tüm Dosyalar (*.*)|*.*";
                dlg.Title = "PDF Yükle";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ShowInfo("Seçilen dosya: " + dlg.FileName, "PDF Yükle");
                }
            }
        }

        private void OpenSirketSecim()
        {
            using (var form = new SirketSecimForm(_companySettingsService))
            {
                if (form.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(form.SelectedCompany))
                {
                    var indexControl = _contentPanel.Controls.Count > 0 ? _contentPanel.Controls[0] as UserControls.IndexViewControl : null;
                    if (indexControl != null)
                    {
                        indexControl.AddCompany(form.SelectedCompany);
                    }
                    else
                    {
                        ShowInfo("Seçilen şirket: " + form.SelectedCompany, "Şirket Seçimi");
                    }
                }
            }
        }

        private void ShowInfo(string message, string title)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
