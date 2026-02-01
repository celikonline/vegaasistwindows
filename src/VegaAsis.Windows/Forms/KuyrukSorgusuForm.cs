using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class KuyrukSorgusuForm : Form
    {
        private Panel _topPanel;
        private Panel _bottomPanel;
        private Label _lblAktifSorgular;
        private Button _btnYenile;
        private DataGridView _dgvKuyruk;
        private Label _lblToplam;
        private Button _btnIptal;
        private Button _btnKapat;

        public KuyrukSorgusuForm()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void InitializeComponent()
        {
            Text = "Kuyruk Durumu";
            Size = new Size(700, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Üst Panel
            _topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            _lblAktifSorgular = new Label
            {
                Text = "Aktif Sorgular:",
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold, GraphicsUnit.Point),
                Location = new Point(10, 20),
                AutoSize = true
            };

            _btnYenile = new Button
            {
                Text = "Yenile",
                Width = 80,
                Height = 28,
                Location = new Point(600, 15)
            };
            _btnYenile.Click += BtnYenile_Click;

            _topPanel.Controls.Add(_lblAktifSorgular);
            _topPanel.Controls.Add(_btnYenile);

            // DataGridView
            _dgvKuyruk = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // Alt Panel
            _bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };

            _lblToplam = new Label
            {
                Text = "Toplam: 0 sorgu",
                Location = new Point(10, 15),
                AutoSize = true
            };

            _btnIptal = new Button
            {
                Text = "Seçili Sorguyu İptal Et",
                Width = 150,
                Height = 28,
                Location = new Point(400, 11)
            };
            _btnIptal.Click += BtnIptal_Click;

            _btnKapat = new Button
            {
                Text = "Kapat",
                Width = 80,
                Height = 28,
                Location = new Point(600, 11),
                DialogResult = DialogResult.Cancel
            };

            _bottomPanel.Controls.Add(_lblToplam);
            _bottomPanel.Controls.Add(_btnIptal);
            _bottomPanel.Controls.Add(_btnKapat);

            // Ana kontrolleri ekle
            Controls.Add(_dgvKuyruk);
            Controls.Add(_topPanel);
            Controls.Add(_bottomPanel);

            CancelButton = _btnKapat;
        }

        private void LoadSampleData()
        {
            var dt = new DataTable();
            dt.Columns.Add("SıraNo", typeof(int));
            dt.Columns.Add("Plaka", typeof(string));
            dt.Columns.Add("Şirket", typeof(string));
            dt.Columns.Add("SorguTipi", typeof(string));
            dt.Columns.Add("Durum", typeof(string));
            dt.Columns.Add("BaşlangıçZamanı", typeof(string));

            dt.Rows.Add(1, "34ABC123", "Anadolu Sigorta", "Trafik", "Bekliyor", DateTime.Now.AddMinutes(-15).ToString("dd.MM.yyyy HH:mm"));
            dt.Rows.Add(2, "06XYZ789", "Allianz Sigorta", "Kasko", "İşleniyor", DateTime.Now.AddMinutes(-8).ToString("dd.MM.yyyy HH:mm"));
            dt.Rows.Add(3, "35DEF456", "HDI Sigorta", "DASK", "Tamamlandı", DateTime.Now.AddMinutes(-30).ToString("dd.MM.yyyy HH:mm"));
            dt.Rows.Add(4, "01GHI012", "Groupama Sigorta", "Trafik", "Bekliyor", DateTime.Now.AddMinutes(-5).ToString("dd.MM.yyyy HH:mm"));

            _dgvKuyruk.DataSource = dt;
            _dgvKuyruk.Columns["SıraNo"].HeaderText = "Sıra No";
            _dgvKuyruk.Columns["Plaka"].HeaderText = "Plaka";
            _dgvKuyruk.Columns["Şirket"].HeaderText = "Şirket";
            _dgvKuyruk.Columns["SorguTipi"].HeaderText = "Sorgu Tipi";
            _dgvKuyruk.Columns["Durum"].HeaderText = "Durum";
            _dgvKuyruk.Columns["BaşlangıçZamanı"].HeaderText = "Başlangıç Zamanı";

            UpdateTotalLabel();
        }

        private void UpdateTotalLabel()
        {
            var count = _dgvKuyruk.Rows.Count;
            _lblToplam.Text = $"Toplam: {count} sorgu";
        }

        private void BtnYenile_Click(object sender, EventArgs e)
        {
            LoadSampleData();
        }

        private void BtnIptal_Click(object sender, EventArgs e)
        {
            if (_dgvKuyruk.CurrentRow == null)
            {
                MessageBox.Show("İptal etmek için bir sorgu seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var plaka = _dgvKuyruk.CurrentRow.Cells["Plaka"].Value?.ToString();
            var durum = _dgvKuyruk.CurrentRow.Cells["Durum"].Value?.ToString();

            if (durum == "Tamamlandı")
            {
                MessageBox.Show("Tamamlanmış sorgular iptal edilemez.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"'{plaka}' plakalı sorguyu iptal etmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // İptal işlemi burada yapılacak
                MessageBox.Show("Sorgu iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSampleData();
            }
        }
    }
}
