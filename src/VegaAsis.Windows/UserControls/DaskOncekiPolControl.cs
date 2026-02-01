using System;
using System.Drawing;
using System.Windows.Forms;
using VegaAsis.Windows.Data;

namespace VegaAsis.Windows.UserControls
{
    public class DaskOncekiPolControl : UserControl
    {
        private ComboBox _cmbSirket;
        private TextBox _txtPoliceNo;
        private TextBox _txtBitisTarihi;
        private ComboBox _cmbIl;
        private ComboBox _cmbIlce;

        public string Sirket { get { return _cmbSirket?.SelectedItem as string; } set { SetCombo(_cmbSirket, value); } }
        public string PoliceNo { get { return _txtPoliceNo?.Text ?? ""; } set { if (_txtPoliceNo != null) _txtPoliceNo.Text = value ?? ""; } }
        public string BitisTarihi { get { return _txtBitisTarihi?.Text ?? ""; } set { if (_txtBitisTarihi != null) _txtBitisTarihi.Text = value ?? ""; } }
        public string Il { get { return _cmbIl?.SelectedItem as string; } set { SetCombo(_cmbIl, value); } }
        public string Ilce { get { return _cmbIlce?.SelectedItem as string; } set { SetCombo(_cmbIlce, value); } }

        public DaskOncekiPolControl()
        {
            Dock = DockStyle.Fill;
            Padding = new Padding(8);
            BuildUI();
        }

        private void BuildUI()
        {
            int y = 12;

            AddLabel(this, "Sigorta Şirketi:", 12, y);
            _cmbSirket = new ComboBox { Left = 120, Top = y - 2, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbSirket.Items.AddRange(SigortaSirketleri.List);
            if (_cmbSirket.Items.Count > 0) _cmbSirket.SelectedIndex = 0;
            Controls.Add(_cmbSirket);
            y += 30;

            AddLabel(this, "Poliçe No:", 12, y);
            _txtPoliceNo = new TextBox { Left = 120, Top = y - 2, Width = 200 };
            Controls.Add(_txtPoliceNo);
            y += 30;

            AddLabel(this, "Bitiş Tarihi:", 12, y);
            _txtBitisTarihi = new TextBox { Left = 120, Top = y - 2, Width = 200 };
            Controls.Add(_txtBitisTarihi);
            y += 30;

            AddLabel(this, "İl / İlçe:", 12, y);
            _cmbIl = new ComboBox { Left = 120, Top = y - 2, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbIl.Items.AddRange(TurkeyLocations.GetCityNames());
            if (_cmbIl.Items.Count > 0) _cmbIl.SelectedIndex = 0;
            _cmbIl.SelectedIndexChanged += (s, e) =>
            {
                var city = _cmbIl.SelectedItem as string;
                if (string.IsNullOrEmpty(city)) return;
                _cmbIlce.Items.Clear();
                _cmbIlce.Items.AddRange(TurkeyLocations.GetDistrictsByCity(city));
                if (_cmbIlce.Items.Count > 0) _cmbIlce.SelectedIndex = 0;
            };
            _cmbIlce = new ComboBox { Left = 225, Top = y - 2, Width = 95, DropDownStyle = ComboBoxStyle.DropDownList };
            if (_cmbIl.Items.Count > 0)
            {
                var city = _cmbIl.SelectedItem as string;
                if (!string.IsNullOrEmpty(city))
                {
                    _cmbIlce.Items.AddRange(TurkeyLocations.GetDistrictsByCity(city));
                    if (_cmbIlce.Items.Count > 0) _cmbIlce.SelectedIndex = 0;
                }
            }
            Controls.Add(_cmbIl);
            Controls.Add(_cmbIlce);
        }

        private void AddLabel(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label { Text = text, Left = x, Top = y, Width = 100 });
        }

        private void SetCombo(ComboBox cmb, string value)
        {
            if (cmb == null) return;
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (string.Equals(cmb.Items[i].ToString(), value ?? "", StringComparison.OrdinalIgnoreCase))
                {
                    cmb.SelectedIndex = i;
                    return;
                }
            }
        }
    }
}
