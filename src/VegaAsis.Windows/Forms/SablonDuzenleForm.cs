using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class SablonDuzenleForm : Form
    {
        private ListBox _lstSablonlar;
        private TextBox _txtSablonAdi;
        private ComboBox _cmbBrans;
        private CheckedListBox _chkSirketler;
        private Button _btnKaydet;
        private Button _btnSil;
        private Button _btnYeni;
        private Button _btnKapat;

        public SablonDuzenleForm()
        {
            InitializeComponent();
            OrnekYukle();
        }

        private void InitializeComponent()
        {
            Text = "Otomatik Sorgu - Şablon Düzenle";
            Size = new Size(550, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(500, 350);

            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 160 };
            _lstSablonlar = new ListBox { Dock = DockStyle.Fill };
            _lstSablonlar.SelectedIndexChanged += (s, e) => SablonSecildi();
            split.Panel1.Controls.Add(_lstSablonlar);

            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            int y = 12;
            rightPanel.Controls.Add(new Label { Text = "Şablon Adı:", Left = 8, Top = y });
            _txtSablonAdi = new TextBox { Left = 100, Top = y - 2, Width = 250 };
            rightPanel.Controls.Add(_txtSablonAdi);
            y += 32;
            rightPanel.Controls.Add(new Label { Text = "Branş:", Left = 8, Top = y });
            _cmbBrans = new ComboBox { Left = 100, Top = y - 2, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbBrans.Items.AddRange(new[] { "TRAFİK", "KASKO", "DASK", "TSS", "KONUT", "İMM" });
            if (_cmbBrans.Items.Count > 0) _cmbBrans.SelectedIndex = 0;
            rightPanel.Controls.Add(_cmbBrans);
            y += 32;
            rightPanel.Controls.Add(new Label { Text = "Şirketler:", Left = 8, Top = y });
            _chkSirketler = new CheckedListBox { Left = 100, Top = y, Width = 250, Height = 140 };
            _chkSirketler.Items.AddRange(new[] { "ANADOLU", "AK SİGORTA", "AXA", "HDI", "ATLAS", "SOMPO" });
            rightPanel.Controls.Add(_chkSirketler);
            y += 150;
            _btnYeni = new Button { Text = "Yeni", Size = new Size(70, 28), Location = new Point(8, y) };
            _btnKaydet = new Button { Text = "Kaydet", Size = new Size(70, 28), Location = new Point(85, y) };
            _btnSil = new Button { Text = "Sil", Size = new Size(70, 28), Location = new Point(162, y) };
            _btnYeni.Click += (s, e) => { _lstSablonlar.Items.Add("Yeni Şablon"); _txtSablonAdi.Clear(); _txtSablonAdi.Text = "Yeni Şablon"; };
            _btnKaydet.Click += (s, e) => MessageBox.Show("Şablon kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _btnSil.Click += (s, e) => { if (_lstSablonlar.SelectedIndex >= 0) _lstSablonlar.Items.RemoveAt(_lstSablonlar.SelectedIndex); };
            rightPanel.Controls.AddRange(new Control[] { _btnYeni, _btnKaydet, _btnSil });
            split.Panel2.Controls.Add(rightPanel);
            Controls.Add(split);

            var pnlAlt = new Panel { Dock = DockStyle.Bottom, Height = 45 };
            _btnKapat = new Button { Text = "Kapat", Size = new Size(80, 28), Location = new Point(458, 8) };
            _btnKapat.Click += (s, e) => Close();
            pnlAlt.Controls.Add(_btnKapat);
            Controls.Add(pnlAlt);
        }

        private void OrnekYukle()
        {
            _lstSablonlar.Items.AddRange(new[] { "Varsayılan Trafik", "Kasko Hızlı", "DASK Toplu" });
        }

        private void SablonSecildi()
        {
            if (_lstSablonlar.SelectedItem != null)
                _txtSablonAdi.Text = _lstSablonlar.SelectedItem.ToString();
        }
    }
}
