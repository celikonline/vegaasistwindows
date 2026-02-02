using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private UserControls.IndexViewControl _indexControl;

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
            Text = "Open Hızlı Teklif Sistemi V3 - Ana Acente : ANGORA SİGORTA - 87945 - Yönetici : VOLKAN - Sürüm : 3.0.0.348 | Bağlandı!";
            Size = new Size(1024, 768);
            MinimumSize = new Size(800, 600);
            Icon = CreateRedCircleIcon();

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
            _indexControl = new UserControls.IndexViewControl();
            _indexControl.Dock = DockStyle.Fill;
            _indexControl.SbmSorgusuRequested += (s, e) => OpenForm(new SbmSorgusuForm());
            _indexControl.KuyrukSorgusuRequested += (s, e) => OpenForm(new KuyrukSorgusuForm());
            _indexControl.WebcamQRRequested += (s, e) => OpenForm(new WebcamQRForm());
            _indexControl.SirketEkleRequested += (s, e) => OpenSirketSecim();
            _indexControl.AdminPanelRequested += (s, e) => OpenForm(new AdminPanelForm(_companySettingsService, _userManagementService, _authService));
            _indexControl.TekliflerRequested += (s, e) => OpenForm(new TekliflerForm(_authService, _offerService));
            _indexControl.PolicelerimRequested += (s, e) => OpenForm(new PolicelerimForm(_policyService));
            _indexControl.SirketlerRobotRequested += (s, e) => OpenForm(new SirketlerRobotForm());
            _indexControl.RaporlarRequested += (s, e) => OpenForm(new RaporlarForm(_offerService, _policyService));
            _indexControl.DestekTalepleriRequested += (s, e) => OpenForm(new DestekTalepleriForm());
            _indexControl.AjandaYenilemeRequested += (s, e) => OpenForm(new AjandaYenilemeForm(_appointmentService));
            _indexControl.DuyurularRequested += (s, e) =>
            {
                OpenForm(new DuyurularForm());
                var _ = _indexControl.RefreshDuyuruBadgeAsync();
            };
            _indexControl.CanliUretimRequested += (s, e) => OpenForm(new CanliUretimForm());
            _indexControl.CanliDestekRequested += (s, e) => OpenForm(new CanliDestekForm());
            _indexControl.BranchCellClickRequested += (s, e) =>
            {
                var branch = e.Branch;
                var company = e.CompanyName;
                Form form = null;
                if (branch == "TRAFİK") form = new TrafikTeklifiForm();
                else if (branch == "KASKO") form = new KaskoTeminatlariForm();
                else if (branch == "SBM") form = new SbmSorgusuForm();
                else if (branch == "TSS") form = new TssDetaylariForm();
                else if (branch == "DASK") form = new DaskDetaylariForm();
                else if (branch == "KONUT") form = new KonutTeminatlariForm();
                else if (branch == "İMM") form = new ImmTeminatlariForm();
                if (form != null)
                {
                    form.Text = string.IsNullOrEmpty(company) ? form.Text : form.Text + " - " + company;
                    OpenForm(form);
                }
            };
            _indexControl.PdfExportRequested += (s, e) => OpenForm(new PDFExportForm());
            _indexControl.PdfUploadRequested += (s, e) => OpenPdfUpload();
            _indexControl.SorguBaslatRequested += (s, ev) =>
            {
                var idx = s as UserControls.IndexViewControl;
                if (idx == null) return;
                var session = idx.Session;
                var userId = _authService.GetCurrentUserId;
                if (!userId.HasValue) { MessageBox.Show("Giriş yapmalısınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                try
                {
                    var kullanimTarzi = session.KullanimTarzi;
                    if (string.IsNullOrWhiteSpace(kullanimTarzi) || string.Equals(kullanimTarzi, "KULLANIM TARZI SEÇİNİZ", StringComparison.OrdinalIgnoreCase))
                        kullanimTarzi = null;
                    var dto = new OfferDto
                    {
                        Plaka = session.Plaka,
                        TcVergi = session.TcVergi,
                        BelgeSeri = session.BelgeSeri,
                        Musteri = session.MusteriAdi,
                        Meslek = session.Meslek,
                        DogumTarihi = session.DogumTarihi,
                        KullanimTarzi = kullanimTarzi,
                        AracMarkasi = string.IsNullOrWhiteSpace(session.Marka) ? null : session.Marka,
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
            _indexControl.DuraklatRequested += (s, ev) => { };
            _indexControl.YeniSorguKaydetRequested += (s, ev) => MessageBox.Show("Yeni sorgu hazır. Form alanları temizlendi.", "Yeni Sorgu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _contentPanel.Controls.Add(_indexControl);
        }

        private void OpenForm(Form form)
        {
            try
            {
                form.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message ?? "Form açılırken hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                form?.Dispose();
            }
        }

        private void OpenPoliceNoGit()
        {
            using (var form = new PoliceNoGitForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(form.PoliceNo))
                {
                    OpenForm(new PolicelerimForm(_policyService, form.PoliceNo));
                }
            }
        }

        private void OpenTeklifNoGit()
        {
            using (var form = new TeklifNoGitForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(form.TeklifNo))
                {
                    OpenForm(new TekliflerForm(_authService, _offerService, form.TeklifNo));
                }
            }
        }

        private void OpenPdfUpload()
        {
            using (var input = new TeklifNoGitForm())
            {
                if (input.ShowDialog(this) != DialogResult.OK || string.IsNullOrEmpty(input.TeklifNo))
                    return;
                OpenForm(new TeklifDosyalariForm(input.TeklifNo));
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

        private static Icon CreateRedCircleIcon()
        {
            const int size = 32;
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (var brush = new SolidBrush(Color.FromArgb(211, 47, 47)))
                    g.FillEllipse(brush, 2, 2, size - 4, size - 4);
            }
            var hicon = bmp.GetHicon();
            var icon = (Icon)Icon.FromHandle(hicon).Clone();
            bmp.Dispose();
            return icon;
        }
    }
}
