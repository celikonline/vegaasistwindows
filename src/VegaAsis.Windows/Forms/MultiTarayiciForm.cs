using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class MultiTarayiciForm : Form
    {
        private Panel _pnlUst;
        private Panel _pnlAlt;
        private Label _lblAktifTarayicilar;
        private Button _btnYeniEkle;
        private ListBox _lstTarayicilar;
        private Button _btnBaslat;
        private Button _btnDurdur;
        private Button _btnKaldir;
        private Button _btnKapat;

        public MultiTarayiciForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Çoklu Tarayıcı Yönetimi";
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Üst Panel
            _pnlUst = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50
            };

            _lblAktifTarayicilar = new Label
            {
                Text = "Aktif Tarayıcılar",
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                Left = 10,
                Top = 15,
                AutoSize = true
            };

            _btnYeniEkle = new Button
            {
                Text = "Yeni Tarayıcı Ekle",
                Width = 130,
                Left = 350,
                Top = 12,
                Height = 26
            };
            _btnYeniEkle.Click += BtnYeniEkle_Click;

            _pnlUst.Controls.Add(_lblAktifTarayicilar);
            _pnlUst.Controls.Add(_btnYeniEkle);

            // ListBox
            _lstTarayicilar = new ListBox
            {
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.One
            };
            _lstTarayicilar.Items.Add("Tarayıcı 1 - Chrome (Aktif)");
            _lstTarayicilar.Items.Add("Tarayıcı 2 - Firefox (Beklemede)");
            _lstTarayicilar.Items.Add("Tarayıcı 3 - Edge (Kapalı)");

            // Alt Panel
            _pnlAlt = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            _btnBaslat = new Button
            {
                Text = "Başlat",
                Width = 80,
                Left = 10,
                Top = 12,
                Height = 26
            };
            _btnBaslat.Click += BtnBaslat_Click;

            _btnDurdur = new Button
            {
                Text = "Durdur",
                Width = 80,
                Left = 100,
                Top = 12,
                Height = 26
            };
            _btnDurdur.Click += BtnDurdur_Click;

            _btnKaldir = new Button
            {
                Text = "Kaldır",
                Width = 80,
                Left = 190,
                Top = 12,
                Height = 26
            };
            _btnKaldir.Click += BtnKaldir_Click;

            _btnKapat = new Button
            {
                Text = "Kapat",
                Width = 80,
                Left = 400,
                Top = 12,
                Height = 26,
                DialogResult = DialogResult.Cancel
            };
            _btnKapat.Click += BtnKapat_Click;

            _pnlAlt.Controls.Add(_btnBaslat);
            _pnlAlt.Controls.Add(_btnDurdur);
            _pnlAlt.Controls.Add(_btnKaldir);
            _pnlAlt.Controls.Add(_btnKapat);

            Controls.Add(_lstTarayicilar);
            Controls.Add(_pnlUst);
            Controls.Add(_pnlAlt);
        }

        private void BtnYeniEkle_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Yeni tarayıcı ekleme işlemi başlatıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnBaslat_Click(object sender, EventArgs e)
        {
            if (_lstTarayicilar.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir tarayıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show($"Seçili tarayıcı başlatılıyor: {_lstTarayicilar.SelectedItem}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDurdur_Click(object sender, EventArgs e)
        {
            if (_lstTarayicilar.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir tarayıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show($"Seçili tarayıcı durduruluyor: {_lstTarayicilar.SelectedItem}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnKaldir_Click(object sender, EventArgs e)
        {
            if (_lstTarayicilar.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir tarayıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var result = MessageBox.Show($"Seçili tarayıcı kaldırılacak: {_lstTarayicilar.SelectedItem}\n\nDevam etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Tarayıcı kaldırıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnKapat_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
