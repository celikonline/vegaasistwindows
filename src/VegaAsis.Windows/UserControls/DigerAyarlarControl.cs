using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;

namespace VegaAsis.Windows.UserControls
{
    public class DigerAyarlarControl : UserControl
    {
        private readonly IAppSettingsService _appSettingsService;
        private SplitContainer _splitContainer;
        private bool _splitterInitialized;
        private GroupBox _grpGorselAyarlar;
        private PictureBox _picLogo;
        private Button _btnLogoSec;
        private Button _btnKarsilamaGorseliSec;
        private GroupBox _grpSmsAyarlari;
        private TextBox _txtSmsApiKey;
        private TextBox _txtSmsGonderenAdi;
        private CheckBox _chkSmsAktif;
        private GroupBox _grpPdfAyarlari;
        private TextBox _txtPdfHeader;
        private TextBox _txtPdfFooter;
        private Button _btnKaydet;

        public DigerAyarlarControl(IAppSettingsService appSettingsService = null)
        {
            _appSettingsService = appSettingsService;
            SuspendLayout();
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            MinimumSize = new Size(800, 500);
            Padding = new Padding(8);

            BuildSplitContainer();
            BuildKaydetButton();

            ResumeLayout(true);
            Load += DigerAyarlarControl_Load;
        }

        private void BuildKaydetButton()
        {
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(8) };
            _btnKaydet = new Button
            {
                Text = "Kaydet",
                Size = new Size(100, 32),
                Location = new Point(8, 9),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnKaydet.FlatAppearance.BorderSize = 0;
            _btnKaydet.Click += BtnKaydet_Click;
            bottomPanel.Controls.Add(_btnKaydet);
            Controls.Add(bottomPanel);
        }

        private async void DigerAyarlarControl_Load(object sender, EventArgs e)
        {
            if (_appSettingsService != null)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var settings = await _appSettingsService.GetSettingsAsync();
                if (settings != null)
                {
                    _txtSmsApiKey.Text = settings.CaptchaApiKey ?? "";
                    _txtSmsGonderenAdi.Text = settings.SmsMessageHeader ?? "";
                    _chkSmsAktif.Checked = settings.SmsLoginActive;
                    _txtPdfHeader.Text = settings.PdfHeader ?? "";
                    _txtPdfFooter.Text = settings.PdfFooter ?? "";
                }
            }
            catch { }
        }

        private async void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (_appSettingsService == null)
            {
                MessageBox.Show("Ayarlar servisi mevcut değil.", "Uyarı");
                return;
            }
            try
            {
                var settings = await _appSettingsService.GetSettingsAsync();
                if (settings == null) settings = new VegaAsis.Core.DTOs.AppSettingsDto { Id = Guid.NewGuid(), UserId = Guid.Empty };
                settings.CaptchaApiKey = _txtSmsApiKey.Text;
                settings.SmsMessageHeader = _txtSmsGonderenAdi.Text;
                settings.SmsLoginActive = _chkSmsAktif.Checked;
                settings.PdfHeader = _txtPdfHeader.Text;
                settings.PdfFooter = _txtPdfFooter.Text;
                await _appSettingsService.UpdateSettingsAsync(settings);
                MessageBox.Show("Ayarlar kaydedildi.", "Bilgi");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydetme hatası: " + ex.Message, "Hata");
            }
        }

        private void BuildSplitContainer()
        {
            _splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                Panel1MinSize = 0,
                Panel2MinSize = 0,
                SplitterDistance = 0
            };
            _splitContainer.Resize += SplitContainer_Resize;

            BuildPanel1(_splitContainer.Panel1);
            BuildPanel2(_splitContainer.Panel2);

            Controls.Add(_splitContainer);
        }

        private void SplitContainer_Resize(object sender, EventArgs e)
        {
            if (_splitterInitialized) return;
            const int panel1Min = 150;
            const int panel2Min = 200;
            int h = _splitContainer.Height;
            if (h < panel1Min + panel2Min) return;
            _splitterInitialized = true;
            _splitContainer.Resize -= SplitContainer_Resize;
            _splitContainer.Panel1MinSize = panel1Min;
            _splitContainer.Panel2MinSize = panel2Min;
            _splitContainer.SplitterDistance = Math.Max(panel1Min, Math.Min(200, h - panel2Min));
        }

        private void BuildPanel1(Panel panel)
        {
            panel.Padding = new Padding(4);
            panel.BackColor = Color.White;

            _grpGorselAyarlar = new GroupBox
            {
                Text = "Görsel Ayarlar",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Padding = new Padding(8)
            };

            var logoPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                Padding = new Padding(4)
            };

            _picLogo = new PictureBox
            {
                Size = new Size(100, 60),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Location = new Point(8, 8)
            };

            _btnLogoSec = new Button
            {
                Text = "Logo Seç",
                Size = new Size(100, 28),
                Location = new Point(116, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _btnLogoSec.FlatAppearance.BorderSize = 0;

            _btnKarsilamaGorseliSec = new Button
            {
                Text = "Karşılama Görseli Seç",
                Size = new Size(150, 28),
                Location = new Point(116, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _btnKarsilamaGorseliSec.FlatAppearance.BorderSize = 0;

            logoPanel.Controls.Add(_picLogo);
            logoPanel.Controls.Add(_btnLogoSec);
            logoPanel.Controls.Add(_btnKarsilamaGorseliSec);

            _grpGorselAyarlar.Controls.Add(logoPanel);
            panel.Controls.Add(_grpGorselAyarlar);
        }

        private void BuildPanel2(Panel panel)
        {
            panel.Padding = new Padding(4);
            panel.BackColor = Color.White;

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(4)
            };

            var lblAdminInfo = new Label
            {
                Text = "Bu ayarlar yönetici yetkisi gerektirir.",
                Location = new Point(8, 8),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 9F),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            scrollPanel.Controls.Add(lblAdminInfo);

            int y = 32;

            // SMS Ayarları GroupBox
            _grpSmsAyarlari = new GroupBox
            {
                Text = "SMS / Captcha Ayarları",
                Location = new Point(8, y),
                Size = new Size(400, 120),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblApiKey = new Label
            {
                Text = "API Key:",
                Location = new Point(8, 24),
                AutoSize = true
            };
            _txtSmsApiKey = new TextBox
            {
                Location = new Point(80, 22),
                Size = new Size(300, 21),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblGonderenAdi = new Label
            {
                Text = "Gönderen Adı:",
                Location = new Point(8, 52),
                AutoSize = true
            };
            _txtSmsGonderenAdi = new TextBox
            {
                Location = new Point(80, 50),
                Size = new Size(300, 21),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _chkSmsAktif = new CheckBox
            {
                Text = "SMS Aktif",
                Location = new Point(8, 78),
                AutoSize = true
            };

            _grpSmsAyarlari.Controls.Add(lblApiKey);
            _grpSmsAyarlari.Controls.Add(_txtSmsApiKey);
            _grpSmsAyarlari.Controls.Add(lblGonderenAdi);
            _grpSmsAyarlari.Controls.Add(_txtSmsGonderenAdi);
            _grpSmsAyarlari.Controls.Add(_chkSmsAktif);

            scrollPanel.Controls.Add(_grpSmsAyarlari);
            y += 128;

            // PDF Ayarları GroupBox
            _grpPdfAyarlari = new GroupBox
            {
                Text = "PDF Ayarları",
                Location = new Point(8, y),
                Size = new Size(400, 150),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblHeader = new Label
            {
                Text = "Header Metni:",
                Location = new Point(8, 24),
                AutoSize = true
            };
            _txtPdfHeader = new TextBox
            {
                Multiline = true,
                Location = new Point(8, 42),
                Size = new Size(380, 50),
                ScrollBars = ScrollBars.Vertical,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var lblFooter = new Label
            {
                Text = "Footer Metni:",
                Location = new Point(8, 100),
                AutoSize = true
            };
            _txtPdfFooter = new TextBox
            {
                Multiline = true,
                Location = new Point(8, 118),
                Size = new Size(380, 50),
                ScrollBars = ScrollBars.Vertical,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _grpPdfAyarlari.Controls.Add(lblHeader);
            _grpPdfAyarlari.Controls.Add(_txtPdfHeader);
            _grpPdfAyarlari.Controls.Add(lblFooter);
            _grpPdfAyarlari.Controls.Add(_txtPdfFooter);

            scrollPanel.Controls.Add(_grpPdfAyarlari);

            panel.Controls.Add(scrollPanel);
        }
    }
}
