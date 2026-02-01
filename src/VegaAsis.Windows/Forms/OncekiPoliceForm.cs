using System;
using System.Drawing;
using System.Windows.Forms;
using VegaAsis.Windows.UserControls;

namespace VegaAsis.Windows.Forms
{
    public class OncekiPoliceForm : Form
    {
        private TabControl _tabControl;
        private AracOncekiPolControl _aracControl;
        private DaskOncekiPolControl _daskControl;
        private Button _btnKaydet;
        private Button _btnIptal;

        public string PolTipi { get; private set; }
        public string Sirket { get; private set; }
        public string PoliceNo { get; private set; }
        public string BitisTarihi { get; private set; }
        public string Kademe { get; private set; }

        public OncekiPoliceForm(string polTipi = "TRAFİK")
        {
            InitializeComponent(polTipi);
        }

        private void InitializeComponent(string polTipi)
        {
            Text = "Önceki Poliçe Bilgisi";
            Size = new Size(420, 380);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            _tabControl = new TabControl { Dock = DockStyle.Fill, Padding = new Point(8, 8) };

            var tabArac = new TabPage("Araç Önceki Poliçe");
            _aracControl = new AracOncekiPolControl(polTipi) { Dock = DockStyle.Fill };
            tabArac.Controls.Add(_aracControl);
            _tabControl.TabPages.Add(tabArac);

            var tabDask = new TabPage("DASK Önceki Poliçe");
            _daskControl = new DaskOncekiPolControl() { Dock = DockStyle.Fill };
            tabDask.Controls.Add(_daskControl);
            _tabControl.TabPages.Add(tabDask);

            Controls.Add(_tabControl);

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _btnKaydet = new Button { Text = "Kaydet", Size = new Size(90, 30), Location = new Point(200, 10) };
            _btnKaydet.Click += BtnKaydet_Click;
            _btnIptal = new Button { Text = "İptal", DialogResult = DialogResult.Cancel, Size = new Size(90, 30), Location = new Point(300, 10) };
            pnlBottom.Controls.Add(_btnKaydet);
            pnlBottom.Controls.Add(_btnIptal);
            Controls.Add(pnlBottom);

            AcceptButton = _btnKaydet;
            CancelButton = _btnIptal;
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (_tabControl.SelectedIndex == 0)
            {
                PolTipi = _aracControl.PolTipi;
                Sirket = _aracControl.Sirket;
                PoliceNo = _aracControl.PoliceNo;
                BitisTarihi = _aracControl.BitisTarihi;
                Kademe = _aracControl.Kademe;
            }
            else
            {
                Sirket = _daskControl.Sirket;
                PoliceNo = _daskControl.PoliceNo;
                BitisTarihi = _daskControl.BitisTarihi;
            }

            if (string.IsNullOrEmpty(Sirket) || Sirket == "Seçiniz")
            {
                MessageBox.Show("Lütfen sigorta şirketi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
