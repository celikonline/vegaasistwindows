using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class TssDetaylariForm : Form
    {
        // Sigortalı Bilgileri
        private TextBox _txtAdSoyad;
        private TextBox _txtTcKimlikNo;
        private DateTimePicker _dtpDogumTarihi;
        private ComboBox _cmbCinsiyet;

        // Plan Bilgileri
        private ComboBox _cmbPlanTipi;
        private ComboBox _cmbHastaneAgi;

        // Teminat Limitleri
        private DataGridView _dgvTeminatLimitleri;

        // Alt Panel
        private Label _lblAylikPrim;
        private Button _btnKapat;
        private Panel _bottomPanel;

        public TssDetaylariForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "TSS Detayları";
            Size = new Size(600, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Sigortalı Bilgileri GroupBox
            var grpSigortaliBilgileri = new GroupBox
            {
                Text = "Sigortalı Bilgileri",
                Location = new Point(12, 12),
                Size = new Size(570, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            CreateSigortaliBilgileriGroup(grpSigortaliBilgileri);
            Controls.Add(grpSigortaliBilgileri);

            // Plan Bilgileri GroupBox
            var grpPlanBilgileri = new GroupBox
            {
                Text = "Plan Bilgileri",
                Location = new Point(12, 140),
                Size = new Size(570, 90),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            CreatePlanBilgileriGroup(grpPlanBilgileri);
            Controls.Add(grpPlanBilgileri);

            // Teminat Limitleri GroupBox
            var grpTeminatLimitleri = new GroupBox
            {
                Text = "Teminat Limitleri",
                Location = new Point(12, 238),
                Size = new Size(570, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            CreateTeminatLimitleriGroup(grpTeminatLimitleri);
            Controls.Add(grpTeminatLimitleri);

            // Bottom Panel
            _bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };
            Controls.Add(_bottomPanel);

            // Aylık Prim Label
            var lblAylikPrimLabel = new Label
            {
                Text = "Aylık Prim:",
                Location = new Point(10, 15),
                AutoSize = true
            };
            _bottomPanel.Controls.Add(lblAylikPrimLabel);

            _lblAylikPrim = new Label
            {
                Text = "0,00 TL",
                Location = new Point(lblAylikPrimLabel.Right + 5, 15),
                AutoSize = true,
                Font = new Font(DefaultFont, FontStyle.Bold)
            };
            _bottomPanel.Controls.Add(_lblAylikPrim);

            // Kapat Button
            _btnKapat = new Button
            {
                Text = "Kapat",
                DialogResult = DialogResult.Cancel,
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            _btnKapat.Location = new Point(_bottomPanel.Width - _btnKapat.Width - 10, 10);
            _btnKapat.Click += (s, e) => Close();
            _bottomPanel.Controls.Add(_btnKapat);
        }

        private void CreateSigortaliBilgileriGroup(GroupBox groupBox)
        {
            var y = 25;
            const int labelWidth = 120;
            const int controlWidth = 200;
            const int spacing = 30;
            const int leftOffset = 15;
            const int controlLeft = 140;

            // Ad Soyad
            AddLabel(groupBox, "Ad Soyad:", leftOffset, y, labelWidth);
            _txtAdSoyad = AddTextBox(groupBox, controlLeft, y - 2, controlWidth);
            y += spacing;

            // TC Kimlik No
            AddLabel(groupBox, "TC Kimlik No:", leftOffset, y, labelWidth);
            _txtTcKimlikNo = AddTextBox(groupBox, controlLeft, y - 2, controlWidth);
            y += spacing;

            // Doğum Tarihi
            AddLabel(groupBox, "Doğum Tarihi:", leftOffset, y, labelWidth);
            _dtpDogumTarihi = new DateTimePicker
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                Format = DateTimePickerFormat.Short
            };
            groupBox.Controls.Add(_dtpDogumTarihi);
            y += spacing;

            // Cinsiyet
            AddLabel(groupBox, "Cinsiyet:", leftOffset, y, labelWidth);
            _cmbCinsiyet = new ComboBox
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbCinsiyet.Items.AddRange(new object[] { "Erkek", "Kadın" });
            groupBox.Controls.Add(_cmbCinsiyet);
        }

        private void CreatePlanBilgileriGroup(GroupBox groupBox)
        {
            var y = 25;
            const int labelWidth = 120;
            const int controlWidth = 200;
            const int spacing = 30;
            const int leftOffset = 15;
            const int controlLeft = 140;

            // Plan Tipi
            AddLabel(groupBox, "Plan Tipi:", leftOffset, y, labelWidth);
            _cmbPlanTipi = new ComboBox
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbPlanTipi.Items.AddRange(new object[] { "Giriş", "Standart", "Premium", "VIP" });
            groupBox.Controls.Add(_cmbPlanTipi);
            y += spacing;

            // Hastane Ağı
            AddLabel(groupBox, "Hastane Ağı:", leftOffset, y, labelWidth);
            _cmbHastaneAgi = new ComboBox
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbHastaneAgi.Items.AddRange(new object[] { "Tüm Hastaneler", "Anlaşmalı", "Özel Seçim" });
            groupBox.Controls.Add(_cmbHastaneAgi);
        }

        private void CreateTeminatLimitleriGroup(GroupBox groupBox)
        {
            _dgvTeminatLimitleri = new DataGridView
            {
                Location = new Point(15, 25),
                Size = new Size(540, 165),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // Kolonları ekle
            _dgvTeminatLimitleri.Columns.Add("Teminat", "Teminat");
            _dgvTeminatLimitleri.Columns.Add("Limit", "Limit");
            _dgvTeminatLimitleri.Columns.Add("Muafiyet", "Muafiyet");

            // Örnek verileri ekle
            _dgvTeminatLimitleri.Rows.Add("Yatarak Tedavi", "500.000 TL", "10%");
            _dgvTeminatLimitleri.Rows.Add("Ayakta Tedavi", "50.000 TL", "5%");
            _dgvTeminatLimitleri.Rows.Add("Ameliyat", "200.000 TL", "15%");
            _dgvTeminatLimitleri.Rows.Add("Check-up", "5.000 TL", "0%");
            _dgvTeminatLimitleri.Rows.Add("Diş Tedavisi", "30.000 TL", "10%");

            groupBox.Controls.Add(_dgvTeminatLimitleri);
        }

        private Label AddLabel(GroupBox groupBox, string text, int x, int y, int width)
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
            groupBox.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(GroupBox groupBox, int x, int y, int width)
        {
            var txt = new TextBox
            {
                Left = x,
                Top = y,
                Width = width
            };
            groupBox.Controls.Add(txt);
            return txt;
        }
    }
}
