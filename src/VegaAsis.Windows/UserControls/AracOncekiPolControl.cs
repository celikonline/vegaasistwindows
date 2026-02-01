using System;
using System.Drawing;
using System.Windows.Forms;
using VegaAsis.Windows.Data;

namespace VegaAsis.Windows.UserControls
{
    public class AracOncekiPolControl : UserControl
    {
        private ComboBox _cmbPolTipi;
        private ComboBox _cmbSirket;
        private TextBox _txtPoliceNo;
        private TextBox _txtBitisTarihi;
        private TextBox _txtKademe;
        private TextBox _txtYenilemeNo;

        public string PolTipi { get { return _cmbPolTipi?.SelectedItem as string; } set { SetCombo(_cmbPolTipi, value); } }
        public string Sirket { get { return _cmbSirket?.SelectedItem as string; } set { SetCombo(_cmbSirket, value); } }
        public string PoliceNo { get { return _txtPoliceNo?.Text ?? ""; } set { if (_txtPoliceNo != null) _txtPoliceNo.Text = value ?? ""; } }
        public string BitisTarihi { get { return _txtBitisTarihi?.Text ?? ""; } set { if (_txtBitisTarihi != null) _txtBitisTarihi.Text = value ?? ""; } }
        public string Kademe { get { return _txtKademe?.Text ?? ""; } set { if (_txtKademe != null) _txtKademe.Text = value ?? ""; } }
        public string YenilemeNo { get { return _txtYenilemeNo?.Text ?? ""; } set { if (_txtYenilemeNo != null) _txtYenilemeNo.Text = value ?? ""; } }

        public AracOncekiPolControl(string polTipi = "TRAFİK")
        {
            Dock = DockStyle.Fill;
            Padding = new Padding(8);
            BuildUI(polTipi);
        }

        private void BuildUI(string polTipi)
        {
            int y = 12;

            AddLabel(this, "Poliçe Tipi:", 12, y);
            _cmbPolTipi = new ComboBox { Left = 120, Top = y - 2, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbPolTipi.Items.AddRange(new[] { "TRAFİK", "KASKO" });
            _cmbPolTipi.SelectedIndex = polTipi == "KASKO" ? 1 : 0;
            Controls.Add(_cmbPolTipi);
            y += 30;

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

            AddLabel(this, "Kademe:", 12, y);
            _txtKademe = new TextBox { Left = 120, Top = y - 2, Width = 200 };
            Controls.Add(_txtKademe);
            y += 30;

            AddLabel(this, "Yenileme No:", 12, y);
            _txtYenilemeNo = new TextBox { Left = 120, Top = y - 2, Width = 200 };
            Controls.Add(_txtYenilemeNo);
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
