using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Windows.Data;
using VegaAsis.Windows;

namespace VegaAsis.Windows.Forms
{
    public class DaskDetaylariForm : Form
    {
        // Bina Bilgileri
        private ComboBox _cmbYapiTarzi;
        private NumericUpDown _nudKatSayisi;
        private NumericUpDown _nudBulunduguKat;
        private NumericUpDown _nudBrutYuzolcumu;
        private NumericUpDown _nudInsaatYili;

        // Adres Bilgileri
        private ComboBox _cmbIl;
        private ComboBox _cmbIlce;
        private TextBox _txtMahalle;
        private TextBox _txtAcikAdres;

        // Teminat Bilgileri
        private DataGridView _dgvTeminatlar;
        private Label _lblTeminatTutari;
        private Label _lblPrimTutari;

        private Button _btnKapat;
        private Panel _bottomPanel;

        public DaskDetaylariForm()
        {
            InitializeComponent();
            Shown += DaskDetaylariForm_Shown;
        }

        private async void DaskDetaylariForm_Shown(object sender, EventArgs e)
        {
            await LoadTeminatlarAsync().ConfigureAwait(true);
        }

        private void InitializeComponent()
        {
            Text = "DASK Detayları";
            Size = new Size(600, 580);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Bina Bilgileri GroupBox
            var grpBinaBilgileri = new GroupBox
            {
                Text = "Bina Bilgileri",
                Location = new Point(12, 12),
                Size = new Size(570, 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            CreateBinaBilgileriGroup(grpBinaBilgileri);
            Controls.Add(grpBinaBilgileri);

            // Adres Bilgileri GroupBox
            var grpAdresBilgileri = new GroupBox
            {
                Text = "Adres Bilgileri",
                Location = new Point(12, 170),
                Size = new Size(570, 180),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            CreateAdresBilgileriGroup(grpAdresBilgileri);
            Controls.Add(grpAdresBilgileri);

            // Teminat Bilgileri GroupBox
            var grpTeminatBilgileri = new GroupBox
            {
                Text = "Teminat Bilgileri",
                Location = new Point(12, 358),
                Size = new Size(570, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            CreateTeminatBilgileriGroup(grpTeminatBilgileri);
            Controls.Add(grpTeminatBilgileri);

            // Bottom Panel
            _bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };
            Controls.Add(_bottomPanel);

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

        private void CreateBinaBilgileriGroup(GroupBox groupBox)
        {
            var y = 25;
            const int labelWidth = 120;
            const int controlWidth = 200;
            const int spacing = 30;
            const int leftOffset = 15;
            const int controlLeft = 140;

            // Yapı Tarzı
            AddLabel(groupBox, "Yapı Tarzı:", leftOffset, y, labelWidth);
            _cmbYapiTarzi = new ComboBox
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbYapiTarzi.Items.AddRange(YapiTarziOptions.List);
            groupBox.Controls.Add(_cmbYapiTarzi);
            y += spacing;

            // Kat Sayısı
            AddLabel(groupBox, "Kat Sayısı:", leftOffset, y, labelWidth);
            _nudKatSayisi = new NumericUpDown
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                Minimum = 0,
                Maximum = 50,
                DecimalPlaces = 0
            };
            groupBox.Controls.Add(_nudKatSayisi);
            y += spacing;

            // Bulunduğu Kat
            AddLabel(groupBox, "Bulunduğu Kat:", leftOffset, y, labelWidth);
            _nudBulunduguKat = new NumericUpDown
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                Minimum = 0,
                Maximum = 50,
                DecimalPlaces = 0
            };
            groupBox.Controls.Add(_nudBulunduguKat);
            y += spacing;

            // Brüt Yüzölçümü
            AddLabel(groupBox, "Brüt Yüzölçümü (m²):", leftOffset, y, labelWidth);
            _nudBrutYuzolcumu = new NumericUpDown
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 2
            };
            groupBox.Controls.Add(_nudBrutYuzolcumu);

            // İnşaat Yılı (sağ tarafta)
            AddLabel(groupBox, "İnşaat Yılı:", 360, 25, labelWidth);
            _nudInsaatYili = new NumericUpDown
            {
                Left = 485,
                Top = 23,
                Width = controlWidth,
                Minimum = 1900,
                Maximum = DateTime.Now.Year,
                DecimalPlaces = 0
            };
            groupBox.Controls.Add(_nudInsaatYili);
        }

        private void CreateAdresBilgileriGroup(GroupBox groupBox)
        {
            var y = 25;
            const int labelWidth = 120;
            const int controlWidth = 200;
            const int spacing = 30;
            const int leftOffset = 15;
            const int controlLeft = 140;

            // İl
            AddLabel(groupBox, "İl:", leftOffset, y, labelWidth);
            _cmbIl = new ComboBox
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbIl.Items.AddRange(TurkeyLocations.GetCityNames());
            if (_cmbIl.Items.Count > 0) _cmbIl.SelectedIndex = 0;
            _cmbIl.SelectedIndexChanged += CmbIl_SelectedIndexChanged;
            groupBox.Controls.Add(_cmbIl);
            y += spacing;

            // İlçe (cascade İl'e göre)
            AddLabel(groupBox, "İlçe:", leftOffset, y, labelWidth);
            _cmbIlce = new ComboBox
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            CmbIl_SelectedIndexChanged(_cmbIl, EventArgs.Empty);
            _cmbIlce.SelectedIndexChanged += (s, e) => LoadTeminatlarAsync().ConfigureAwait(true);
            groupBox.Controls.Add(_cmbIlce);
            y += spacing;

            // Mahalle
            AddLabel(groupBox, "Mahalle:", leftOffset, y, labelWidth);
            _txtMahalle = AddTextBox(groupBox, controlLeft, y - 2, controlWidth);
            y += spacing;

            // Açık Adres
            AddLabel(groupBox, "Açık Adres:", leftOffset, y, labelWidth);
            _txtAcikAdres = new TextBox
            {
                Left = controlLeft,
                Top = y - 2,
                Width = controlWidth,
                Height = 60,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            groupBox.Controls.Add(_txtAcikAdres);
        }

        private void CreateTeminatBilgileriGroup(GroupBox groupBox)
        {
            _dgvTeminatlar = new DataGridView
            {
                Location = new Point(15, 25),
                Size = new Size(480, 90),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvTeminatlar.Columns.Add("Kod", "Kod");
            _dgvTeminatlar.Columns.Add("Ad", "Teminat Adı");
            _dgvTeminatlar.Columns.Add("Prim", "Prim");
            _dgvTeminatlar.Columns.Add("Secili", "Seçili");
            groupBox.Controls.Add(_dgvTeminatlar);

            var btnYenile = new Button
            {
                Text = "Yenile",
                Location = new Point(505, 25),
                Size = new Size(60, 26)
            };
            btnYenile.Click += (s, e) => LoadTeminatlarAsync().ConfigureAwait(true);
            groupBox.Controls.Add(btnYenile);

            var y = 120;
            const int leftOffset = 15;
            const int spacing = 30;

            AddLabel(groupBox, "Teminat Tutarı:", leftOffset, y, 120);
            _lblTeminatTutari = new Label
            {
                Text = "0,00 TL",
                Left = 140,
                Top = y,
                Width = 200,
                TextAlign = ContentAlignment.MiddleLeft
            };
            groupBox.Controls.Add(_lblTeminatTutari);
            y += spacing;

            AddLabel(groupBox, "Prim Tutarı:", leftOffset, y, 120);
            _lblPrimTutari = new Label
            {
                Text = "0,00 TL",
                Left = 140,
                Top = y,
                Width = 200,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(DefaultFont, FontStyle.Bold)
            };
            groupBox.Controls.Add(_lblPrimTutari);
        }

        private async Task LoadTeminatlarAsync()
        {
            if (_dgvTeminatlar == null) return;
            _dgvTeminatlar.Rows.Clear();
            if (!ServiceLocator.IsInitialized) return;
            try
            {
                var service = ServiceLocator.Resolve<VegaAsis.Core.Contracts.IDaskService>();
                if (service == null) return;
                var il = _cmbIl?.SelectedItem?.ToString();
                var ilce = _cmbIlce?.SelectedItem?.ToString();
                var list = await service.GetTeminatlarAsync(il, ilce).ConfigureAwait(true);
                if (list == null) return;
                foreach (var t in list)
                {
                    _dgvTeminatlar.Rows.Add(t.Kod ?? "", t.Ad ?? "", t.Prim.HasValue ? t.Prim.Value.ToString("N2") : "", t.Secili ? "Evet" : "Hayır");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DASK teminatlar yüklenirken hata: " + ex.Message);
            }
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

        private void CmbIl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var city = _cmbIl.SelectedItem as string;
            if (string.IsNullOrEmpty(city)) return;
            _cmbIlce.Items.Clear();
            foreach (var d in TurkeyLocations.GetDistrictsByCity(city))
                _cmbIlce.Items.Add(d);
            if (_cmbIlce.Items.Count > 0) _cmbIlce.SelectedIndex = 0;
            LoadTeminatlarAsync().ConfigureAwait(true);
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
