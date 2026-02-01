using System;
using System.Drawing;
using System.Windows.Forms;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.Forms
{
    public class PoliceDetayForm : Form
    {
        private PolicyDto _policy;
        private TabControl _tabControl;
        private Panel _bottomPanel;
        
        // Genel Bilgiler Tab
        private TextBox _txtPoliceNo;
        private TextBox _txtZeyilNo;
        private TextBox _txtYenilemeNo;
        private TextBox _txtSirket;
        private TextBox _txtPoliceTuru;
        private DateTimePicker _dtpBaslangicTarihi;
        private DateTimePicker _dtpBitisTarihi;
        private TextBox _txtPrim;
        private ComboBox _cmbDovizTipi;
        private TextBox _txtAciklama;
        
        // Müşteri Bilgileri Tab
        private TextBox _txtMusteri;
        private TextBox _txtTcVergi;
        private TextBox _txtKullaniciAdi;
        
        // Araç Bilgileri Tab
        private TextBox _txtPlaka;
        
        // Acente Bilgileri Tab
        private TextBox _txtAnaAcente;
        private TextBox _txtKesenAcente;
        
        private Button _btnKaydet;
        private Button _btnIptal;

        public PoliceDetayForm(PolicyDto policy)
        {
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
            
            InitializeComponent();
            LoadPolicyData();
        }

        public PolicyDto EditedPolicy { get; private set; }

        private void InitializeComponent()
        {
            Text = "Poliçe Detayı";
            Size = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // TabControl
            _tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Point(10, 5)
            };
            Controls.Add(_tabControl);

            // Genel Bilgiler Tab
            var tabGenel = new TabPage("Genel Bilgiler");
            _tabControl.TabPages.Add(tabGenel);
            CreateGenelBilgilerTab(tabGenel);

            // Müşteri Bilgileri Tab
            var tabMusteri = new TabPage("Müşteri Bilgileri");
            _tabControl.TabPages.Add(tabMusteri);
            CreateMusteriBilgileriTab(tabMusteri);

            // Araç Bilgileri Tab
            var tabArac = new TabPage("Araç Bilgileri");
            _tabControl.TabPages.Add(tabArac);
            CreateAracBilgileriTab(tabArac);

            // Acente Bilgileri Tab
            var tabAcente = new TabPage("Acente Bilgileri");
            _tabControl.TabPages.Add(tabAcente);
            CreateAcenteBilgileriTab(tabAcente);

            // Bottom Panel
            _bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };
            Controls.Add(_bottomPanel);

            // Buttons
            _btnIptal = new Button
            {
                Text = "İptal",
                DialogResult = DialogResult.Cancel,
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            _btnIptal.Location = new Point(_bottomPanel.Width - _btnIptal.Width - 10, 10);
            _btnIptal.Click += (s, e) => Close();

            _btnKaydet = new Button
            {
                Text = "Kaydet",
                DialogResult = DialogResult.OK,
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            _btnKaydet.Location = new Point(_btnIptal.Left - _btnKaydet.Width - 10, 10);
            _btnKaydet.Click += BtnKaydet_Click;

            _bottomPanel.Controls.Add(_btnKaydet);
            _bottomPanel.Controls.Add(_btnIptal);
        }

        private void CreateGenelBilgilerTab(TabPage tab)
        {
            var y = 20;
            const int labelWidth = 120;
            const int controlWidth = 300;
            const int spacing = 35;

            // Poliçe No
            AddLabel(tab, "Poliçe No:", 20, y, labelWidth);
            _txtPoliceNo = AddTextBox(tab, 150, y, controlWidth);
            y += spacing;

            // Zeyil No
            AddLabel(tab, "Zeyil No:", 20, y, labelWidth);
            _txtZeyilNo = AddTextBox(tab, 150, y, controlWidth);
            y += spacing;

            // Yenileme No
            AddLabel(tab, "Yenileme No:", 20, y, labelWidth);
            _txtYenilemeNo = AddTextBox(tab, 150, y, controlWidth);
            y += spacing;

            // Şirket
            AddLabel(tab, "Şirket:", 20, y, labelWidth);
            _txtSirket = AddTextBox(tab, 150, y, controlWidth);
            y += spacing;

            // Poliçe Türü
            AddLabel(tab, "Poliçe Türü:", 20, y, labelWidth);
            _txtPoliceTuru = AddTextBox(tab, 150, y, controlWidth);
            y += spacing;

            // Başlangıç Tarihi
            AddLabel(tab, "Başlangıç Tarihi:", 20, y, labelWidth);
            _dtpBaslangicTarihi = new DateTimePicker
            {
                Left = 150,
                Top = y,
                Width = controlWidth,
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true,
                Checked = false
            };
            tab.Controls.Add(_dtpBaslangicTarihi);
            y += spacing;

            // Bitiş Tarihi
            AddLabel(tab, "Bitiş Tarihi:", 20, y, labelWidth);
            _dtpBitisTarihi = new DateTimePicker
            {
                Left = 150,
                Top = y,
                Width = controlWidth,
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true,
                Checked = false
            };
            tab.Controls.Add(_dtpBitisTarihi);
            y += spacing;

            // Prim
            AddLabel(tab, "Prim:", 20, y, labelWidth);
            _txtPrim = AddTextBox(tab, 150, y, controlWidth);
            y += spacing;

            // Döviz Tipi
            AddLabel(tab, "Döviz Tipi:", 20, y, labelWidth);
            _cmbDovizTipi = new ComboBox
            {
                Left = 150,
                Top = y,
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbDovizTipi.Items.AddRange(new object[] { "TL", "USD", "EUR" });
            tab.Controls.Add(_cmbDovizTipi);
            y += spacing;

            // Açıklama
            AddLabel(tab, "Açıklama:", 20, y, labelWidth);
            _txtAciklama = new TextBox
            {
                Left = 150,
                Top = y,
                Width = controlWidth,
                Height = 80,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            tab.Controls.Add(_txtAciklama);
        }

        private void CreateMusteriBilgileriTab(TabPage tab)
        {
            var y = 20;
            const int labelWidth = 120;
            const int controlWidth = 300;
            const int spacing = 35;

            // Müşteri Adı
            AddLabel(tab, "Müşteri Adı:", 20, y, labelWidth);
            _txtMusteri = AddTextBox(tab, 150, y, controlWidth);
            y += spacing;

            // TC/Vergi No
            AddLabel(tab, "TC/Vergi No:", 20, y, labelWidth);
            _txtTcVergi = AddTextBox(tab, 150, y, controlWidth);
            y += spacing;

            // Kullanıcı Adı
            AddLabel(tab, "Kullanıcı Adı:", 20, y, labelWidth);
            _txtKullaniciAdi = AddTextBox(tab, 150, y, controlWidth);
        }

        private void CreateAracBilgileriTab(TabPage tab)
        {
            var y = 20;
            const int labelWidth = 120;
            const int controlWidth = 300;

            // Plaka
            AddLabel(tab, "Plaka:", 20, y, labelWidth);
            _txtPlaka = AddTextBox(tab, 150, y, controlWidth);
        }

        private void CreateAcenteBilgileriTab(TabPage tab)
        {
            var y = 20;
            const int labelWidth = 120;
            const int controlWidth = 300;
            const int spacing = 35;

            // Ana Acente
            AddLabel(tab, "Ana Acente:", 20, y, labelWidth);
            _txtAnaAcente = AddTextBox(tab, 150, y, controlWidth);
            y += spacing;

            // Kesen Acente
            AddLabel(tab, "Kesen Acente:", 20, y, labelWidth);
            _txtKesenAcente = AddTextBox(tab, 150, y, controlWidth);
        }

        private Label AddLabel(TabPage tab, string text, int x, int y, int width)
        {
            var lbl = new Label
            {
                Text = text,
                AutoSize = false,
                Left = x,
                Top = y,
                Width = width,
                TextAlign = ContentAlignment.MiddleLeft
            };
            tab.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(TabPage tab, int x, int y, int width)
        {
            var txt = new TextBox
            {
                Left = x,
                Top = y,
                Width = width
            };
            tab.Controls.Add(txt);
            return txt;
        }

        private void LoadPolicyData()
        {
            if (_policy == null) return;

            _txtPoliceNo.Text = _policy.PoliceNo ?? string.Empty;
            _txtZeyilNo.Text = _policy.ZeyilNo ?? string.Empty;
            _txtYenilemeNo.Text = _policy.YenilemeNo ?? string.Empty;
            _txtSirket.Text = _policy.Sirket ?? string.Empty;
            _txtPoliceTuru.Text = _policy.PoliceTuru ?? string.Empty;
            
            if (_policy.BaslangicTarihi.HasValue)
            {
                _dtpBaslangicTarihi.Value = _policy.BaslangicTarihi.Value;
                _dtpBaslangicTarihi.Checked = true;
            }
            
            if (_policy.BitisTarihi.HasValue)
            {
                _dtpBitisTarihi.Value = _policy.BitisTarihi.Value;
                _dtpBitisTarihi.Checked = true;
            }
            
            _txtPrim.Text = _policy.Prim?.ToString() ?? string.Empty;
            
            if (!string.IsNullOrEmpty(_policy.DovizTipi))
            {
                var idx = _cmbDovizTipi.Items.IndexOf(_policy.DovizTipi);
                if (idx >= 0) _cmbDovizTipi.SelectedIndex = idx;
            }
            else
            {
                _cmbDovizTipi.SelectedIndex = 0; // Default to TL
            }
            
            _txtAciklama.Text = _policy.Aciklama ?? string.Empty;
            _txtMusteri.Text = _policy.Musteri ?? string.Empty;
            _txtTcVergi.Text = _policy.TcVergi ?? string.Empty;
            _txtKullaniciAdi.Text = _policy.KullaniciAdi ?? string.Empty;
            _txtPlaka.Text = _policy.Plaka ?? string.Empty;
            _txtAnaAcente.Text = _policy.AnaAcente ?? string.Empty;
            _txtKesenAcente.Text = _policy.KesenAcente ?? string.Empty;
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            EditedPolicy = new PolicyDto
            {
                Id = _policy.Id,
                UserId = _policy.UserId,
                PoliceNo = _txtPoliceNo.Text?.Trim(),
                ZeyilNo = _txtZeyilNo.Text?.Trim(),
                YenilemeNo = _txtYenilemeNo.Text?.Trim(),
                Sirket = _txtSirket.Text?.Trim(),
                PoliceTuru = _txtPoliceTuru.Text?.Trim(),
                BaslangicTarihi = _dtpBaslangicTarihi.Checked ? (DateTime?)_dtpBaslangicTarihi.Value : null,
                BitisTarihi = _dtpBitisTarihi.Checked ? (DateTime?)_dtpBitisTarihi.Value : null,
                Prim = decimal.TryParse(_txtPrim.Text?.Trim(), out var prim) ? (decimal?)prim : null,
                DovizTipi = _cmbDovizTipi.SelectedItem?.ToString(),
                Aciklama = _txtAciklama.Text?.Trim(),
                Musteri = _txtMusteri.Text?.Trim(),
                TcVergi = _txtTcVergi.Text?.Trim(),
                KullaniciAdi = _txtKullaniciAdi.Text?.Trim(),
                Plaka = _txtPlaka.Text?.Trim(),
                AnaAcente = _txtAnaAcente.Text?.Trim(),
                KesenAcente = _txtKesenAcente.Text?.Trim(),
                KayitTarihi = _policy.KayitTarihi,
                KayitTipi = _policy.KayitTipi,
                Personel = _policy.Personel,
                AcentemAlinmaDurumu = _policy.AcentemAlinmaDurumu,
                CreatedAt = _policy.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
