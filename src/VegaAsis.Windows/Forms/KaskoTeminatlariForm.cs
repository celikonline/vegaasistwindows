using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Windows;

namespace VegaAsis.Windows.Forms
{
    public class KaskoTeminatlariForm : Form
    {
        private GroupBox _grpAracBilgileri;
        private GroupBox _grpTemelTeminatlar;
        private GroupBox _grpEkTeminatlar;
        private Panel _bottomPanel;
        
        private TextBox _txtPlaka;
        private TextBox _txtMarka;
        private TextBox _txtModel;
        private TextBox _txtModelYili;
        private TextBox _txtKullanimTarzi;
        
        private DataGridView _dgvTemelTeminatlar;
        
        private FlowLayoutPanel _flpEkTeminatlar;
        private CheckBox _chkKoltukFerdiKaza;
        private CheckBox _chkYardim;
        private CheckBox _chkAnahtarKayip;
        private CheckBox _chkIkameArac;
        private CheckBox _chkLastik;
        private CheckBox _chkMaddiManevi;
        private CheckBox _chkHkAvukat;
        private CheckBox _chkMiniOnarim;
        
        private Label _lblToplamPrim;
        private Label _lblToplamPrimDeger;
        private Button _btnKapat;

        public KaskoTeminatlariForm()
        {
            InitializeComponent();
            Shown += KaskoTeminatlariForm_Shown;
        }

        private async void KaskoTeminatlariForm_Shown(object sender, EventArgs e)
        {
            await LoadTemelTeminatlarAsync().ConfigureAwait(true);
        }

        private void InitializeComponent()
        {
            Text = "Kasko Teminatları";
            Size = new Size(700, 550);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Araç Bilgileri GroupBox
            _grpAracBilgileri = new GroupBox
            {
                Text = "Araç Bilgileri",
                Location = new Point(10, 10),
                Size = new Size(680, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            CreateAracBilgileriGroupBox();
            Controls.Add(_grpAracBilgileri);

            // Temel Teminatlar GroupBox
            _grpTemelTeminatlar = new GroupBox
            {
                Text = "Temel Teminatlar",
                Location = new Point(10, 120),
                Size = new Size(680, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            CreateTemelTeminatlarGroupBox();
            Controls.Add(_grpTemelTeminatlar);

            // Ek Teminatlar GroupBox
            _grpEkTeminatlar = new GroupBox
            {
                Text = "Ek Teminatlar",
                Location = new Point(10, 330),
                Size = new Size(680, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            CreateEkTeminatlarGroupBox();
            Controls.Add(_grpEkTeminatlar);

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

        private void CreateAracBilgileriGroupBox()
        {
            var y = 20;
            const int labelWidth = 80;
            const int textBoxWidth = 120;
            const int spacing = 25;
            const int leftCol1 = 15;
            const int leftCol2 = 150;
            const int leftCol3 = 285;
            const int leftCol4 = 420;
            const int leftCol5 = 555;

            // Plaka
            AddLabel(_grpAracBilgileri, "Plaka:", leftCol1, y, labelWidth);
            _txtPlaka = AddTextBox(_grpAracBilgileri, leftCol1 + labelWidth, y - 2, textBoxWidth);
            y += spacing;

            // Marka
            AddLabel(_grpAracBilgileri, "Marka:", leftCol2, y - spacing, labelWidth);
            _txtMarka = AddTextBox(_grpAracBilgileri, leftCol2 + labelWidth, y - spacing - 2, textBoxWidth);

            // Model
            AddLabel(_grpAracBilgileri, "Model:", leftCol3, y - spacing, labelWidth);
            _txtModel = AddTextBox(_grpAracBilgileri, leftCol3 + labelWidth, y - spacing - 2, textBoxWidth);

            // Model Yılı
            AddLabel(_grpAracBilgileri, "Model Yılı:", leftCol4, y - spacing, labelWidth);
            _txtModelYili = AddTextBox(_grpAracBilgileri, leftCol4 + labelWidth, y - spacing - 2, textBoxWidth);

            // Kullanım Tarzı
            AddLabel(_grpAracBilgileri, "Kullanım Tarzı:", leftCol5, y - spacing, labelWidth);
            _txtKullanimTarzi = AddTextBox(_grpAracBilgileri, leftCol5 + labelWidth, y - spacing - 2, textBoxWidth);
        }

        private void CreateTemelTeminatlarGroupBox()
        {
            _dgvTemelTeminatlar = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false
            };

            // Kolonlar
            _dgvTemelTeminatlar.Columns.Add("TeminatAdi", "Teminat Adı");
            _dgvTemelTeminatlar.Columns.Add("Limit", "Limit");
            _dgvTemelTeminatlar.Columns.Add("Muafiyet", "Muafiyet");

            _grpTemelTeminatlar.Controls.Add(_dgvTemelTeminatlar);
        }

        private void CreateEkTeminatlarGroupBox()
        {
            _flpEkTeminatlar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            _chkKoltukFerdiKaza = new CheckBox { Text = "Koltuk Ferdi Kaza", Width = 150, Margin = new Padding(5) };
            _chkYardim = new CheckBox { Text = "Yardım", Width = 150, Margin = new Padding(5) };
            _chkAnahtarKayip = new CheckBox { Text = "Anahtar Kayıp", Width = 150, Margin = new Padding(5) };
            _chkIkameArac = new CheckBox { Text = "İkame Araç", Width = 150, Margin = new Padding(5) };
            _chkLastik = new CheckBox { Text = "Lastik", Width = 150, Margin = new Padding(5) };
            _chkMaddiManevi = new CheckBox { Text = "Maddi Manevi", Width = 150, Margin = new Padding(5) };
            _chkHkAvukat = new CheckBox { Text = "HK Avukat", Width = 150, Margin = new Padding(5) };
            _chkMiniOnarim = new CheckBox { Text = "Mini Onarım", Width = 150, Margin = new Padding(5) };

            _flpEkTeminatlar.Controls.Add(_chkKoltukFerdiKaza);
            _flpEkTeminatlar.Controls.Add(_chkYardim);
            _flpEkTeminatlar.Controls.Add(_chkAnahtarKayip);
            _flpEkTeminatlar.Controls.Add(_chkIkameArac);
            _flpEkTeminatlar.Controls.Add(_chkLastik);
            _flpEkTeminatlar.Controls.Add(_chkMaddiManevi);
            _flpEkTeminatlar.Controls.Add(_chkHkAvukat);
            _flpEkTeminatlar.Controls.Add(_chkMiniOnarim);

            _grpEkTeminatlar.Controls.Add(_flpEkTeminatlar);
        }

        private void CreateBottomPanel()
        {
            _lblToplamPrim = new Label
            {
                Text = "Toplam Prim:",
                AutoSize = true,
                Location = new Point(10, 15),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular)
            };

            _lblToplamPrimDeger = new Label
            {
                Text = "0,00 TL",
                AutoSize = true,
                Location = new Point(100, 15),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _btnKapat = new Button
            {
                Text = "Kapat",
                DialogResult = DialogResult.Cancel,
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            _btnKapat.Location = new Point(_bottomPanel.Width - _btnKapat.Width - 10, 10);
            _btnKapat.Click += (s, e) => Close();

            _bottomPanel.Controls.Add(_lblToplamPrim);
            _bottomPanel.Controls.Add(_lblToplamPrimDeger);
            _bottomPanel.Controls.Add(_btnKapat);
        }

        private Label AddLabel(Control parent, string text, int x, int y, int width)
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
            parent.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(Control parent, int x, int y, int width)
        {
            var txt = new TextBox
            {
                Left = x,
                Top = y,
                Width = width
            };
            parent.Controls.Add(txt);
            return txt;
        }

        private async Task LoadTemelTeminatlarAsync()
        {
            _dgvTemelTeminatlar.Rows.Clear();
            if (!ServiceLocator.IsInitialized) return;
            try
            {
                var service = ServiceLocator.Resolve<IKaskoService>();
                if (service == null) return;
                var list = await service.GetTeminatlarAsync().ConfigureAwait(true);
                if (list == null) return;
                foreach (var t in list)
                {
                    _dgvTemelTeminatlar.Rows.Add(
                        t.Ad ?? t.Kod ?? "",
                        t.Prim.HasValue ? t.Prim.Value.ToString("N2") : "",
                        t.Aciklama ?? "");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Kasko teminatlar yüklenirken hata: " + ex.Message);
            }
        }
    }
}
