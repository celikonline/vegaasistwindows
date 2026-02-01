using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class ImmTeminatlariForm : Form
    {
        private GroupBox _grpTeminatLimitleri;
        private GroupBox _grpEkBilgiler;
        private GroupBox _grpPrimBilgisi;
        private Panel _bottomPanel;

        private Label _lblKisiBasiLimit;
        private ComboBox _cmbKisiBasiLimit;
        private Label _lblKazaBasiLimit;
        private ComboBox _cmbKazaBasiLimit;
        private Label _lblAracBasiLimit;
        private ComboBox _cmbAracBasiLimit;

        private Label _lblAracPlakasi;
        private TextBox _txtAracPlakasi;
        private Label _lblRuhsatSahibi;
        private TextBox _txtRuhsatSahibi;
        private CheckBox _chkSurucuFerdiKaza;

        private Label _lblYillikPrim;
        private Label _lblYillikPrimDeger;

        private Button _btnKapat;

        public ImmTeminatlariForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "İMM Teminatları";
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Teminat Limitleri GroupBox
            _grpTeminatLimitleri = new GroupBox
            {
                Text = "Teminat Limitleri",
                Location = new Point(10, 10),
                Size = new Size(480, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            CreateTeminatLimitleriGroupBox();
            Controls.Add(_grpTeminatLimitleri);

            // Ek Bilgiler GroupBox
            _grpEkBilgiler = new GroupBox
            {
                Text = "Ek Bilgiler",
                Location = new Point(10, 140),
                Size = new Size(480, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            CreateEkBilgilerGroupBox();
            Controls.Add(_grpEkBilgiler);

            // Prim Bilgisi GroupBox
            _grpPrimBilgisi = new GroupBox
            {
                Text = "Prim Bilgisi",
                Location = new Point(10, 270),
                Size = new Size(480, 80),
                Height = 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            CreatePrimBilgisiGroupBox();
            Controls.Add(_grpPrimBilgisi);

            // Bottom Panel
            _bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };
            CreateBottomPanel();
            Controls.Add(_bottomPanel);
        }

        private void CreateTeminatLimitleriGroupBox()
        {
            int y = 20;
            const int labelWidth = 120;
            const int comboBoxWidth = 150;
            const int spacing = 35;
            const int leftCol = 15;

            // Kişi Başı Limit
            _lblKisiBasiLimit = new Label
            {
                Text = "Kişi Başı Limit:",
                AutoSize = false,
                Left = leftCol,
                Top = y,
                Width = labelWidth,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _grpTeminatLimitleri.Controls.Add(_lblKisiBasiLimit);

            _cmbKisiBasiLimit = new ComboBox
            {
                Left = leftCol + labelWidth + 5,
                Top = y - 2,
                Width = comboBoxWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbKisiBasiLimit.Items.AddRange(new object[] { "100.000 TL", "250.000 TL", "500.000 TL", "1.000.000 TL" });
            _grpTeminatLimitleri.Controls.Add(_cmbKisiBasiLimit);

            y += spacing;

            // Kaza Başı Limit
            _lblKazaBasiLimit = new Label
            {
                Text = "Kaza Başı Limit:",
                AutoSize = false,
                Left = leftCol,
                Top = y,
                Width = labelWidth,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _grpTeminatLimitleri.Controls.Add(_lblKazaBasiLimit);

            _cmbKazaBasiLimit = new ComboBox
            {
                Left = leftCol + labelWidth + 5,
                Top = y - 2,
                Width = comboBoxWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbKazaBasiLimit.Items.AddRange(new object[] { "500.000 TL", "1.000.000 TL", "2.000.000 TL", "5.000.000 TL" });
            _grpTeminatLimitleri.Controls.Add(_cmbKazaBasiLimit);

            y += spacing;

            // Araç Başı Limit
            _lblAracBasiLimit = new Label
            {
                Text = "Araç Başı Limit:",
                AutoSize = false,
                Left = leftCol,
                Top = y,
                Width = labelWidth,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _grpTeminatLimitleri.Controls.Add(_lblAracBasiLimit);

            _cmbAracBasiLimit = new ComboBox
            {
                Left = leftCol + labelWidth + 5,
                Top = y - 2,
                Width = comboBoxWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbAracBasiLimit.Items.AddRange(new object[] { "50.000 TL", "100.000 TL", "250.000 TL" });
            _grpTeminatLimitleri.Controls.Add(_cmbAracBasiLimit);
        }

        private void CreateEkBilgilerGroupBox()
        {
            int y = 20;
            const int labelWidth = 100;
            const int textBoxWidth = 200;
            const int spacing = 35;
            const int leftCol = 15;

            // Araç Plakası
            _lblAracPlakasi = new Label
            {
                Text = "Araç Plakası:",
                AutoSize = false,
                Left = leftCol,
                Top = y,
                Width = labelWidth,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _grpEkBilgiler.Controls.Add(_lblAracPlakasi);

            _txtAracPlakasi = new TextBox
            {
                Left = leftCol + labelWidth + 5,
                Top = y - 2,
                Width = textBoxWidth
            };
            _grpEkBilgiler.Controls.Add(_txtAracPlakasi);

            y += spacing;

            // Ruhsat Sahibi
            _lblRuhsatSahibi = new Label
            {
                Text = "Ruhsat Sahibi:",
                AutoSize = false,
                Left = leftCol,
                Top = y,
                Width = labelWidth,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _grpEkBilgiler.Controls.Add(_lblRuhsatSahibi);

            _txtRuhsatSahibi = new TextBox
            {
                Left = leftCol + labelWidth + 5,
                Top = y - 2,
                Width = textBoxWidth
            };
            _grpEkBilgiler.Controls.Add(_txtRuhsatSahibi);

            y += spacing;

            // Sürücü Ferdi Kaza dahil
            _chkSurucuFerdiKaza = new CheckBox
            {
                Text = "Sürücü Ferdi Kaza dahil",
                Left = leftCol,
                Top = y,
                AutoSize = true
            };
            _grpEkBilgiler.Controls.Add(_chkSurucuFerdiKaza);
        }

        private void CreatePrimBilgisiGroupBox()
        {
            _lblYillikPrim = new Label
            {
                Text = "Yıllık Prim:",
                AutoSize = true,
                Location = new Point(15, 30),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular)
            };
            _grpPrimBilgisi.Controls.Add(_lblYillikPrim);

            _lblYillikPrimDeger = new Label
            {
                Text = "0,00 TL",
                AutoSize = true,
                Location = new Point(100, 25),
                Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            _grpPrimBilgisi.Controls.Add(_lblYillikPrimDeger);
        }

        private void CreateBottomPanel()
        {
            _btnKapat = new Button
            {
                Text = "Kapat",
                DialogResult = DialogResult.Cancel,
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(390, 10)
            };
            _btnKapat.Click += (s, e) => Close();

            _bottomPanel.Controls.Add(_btnKapat);
        }
    }
}
