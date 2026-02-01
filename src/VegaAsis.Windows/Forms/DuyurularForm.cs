using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class DuyurularForm : Form
    {
        private Panel _topPanel;
        private ComboBox _cmbOkunmaDurumu;
        private TextBox _txtArama;
        private Button _btnDuyuruGonder;
        private DataGridView _dgvDuyurular;

        public DuyurularForm()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void InitializeComponent()
        {
            Text = "Duyurular";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;

            // Üst Panel
            _topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            // ComboBox - Okunma Durumu
            _cmbOkunmaDurumu = new ComboBox
            {
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbOkunmaDurumu.Items.AddRange(new object[] { "Tümü", "Okunmamış", "Okunmuş" });
            _cmbOkunmaDurumu.SelectedIndex = 0;

            // TextBox - Arama
            _txtArama = new TextBox
            {
                Width = 200,
                Text = ""
            };

            // Button - Duyuru Gönder
            _btnDuyuruGonder = new Button
            {
                Text = "Duyuru Gönder",
                Width = 120
            };
            _btnDuyuruGonder.Click += (s, e) =>
            {
                using (var form = new DuyuruGonderForm())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        MessageBox.Show(string.Format("Duyuru gönderildi: {0}\n(Entegrasyon bekleniyor)", form.Baslik), "Duyuru Gönder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };

            // FlowLayoutPanel ile üst panel düzenlemesi
            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0)
            };
            flowPanel.Controls.Add(_cmbOkunmaDurumu);
            flowPanel.Controls.Add(_txtArama);
            flowPanel.Controls.Add(_btnDuyuruGonder);
            _topPanel.Controls.Add(flowPanel);

            // DataGridView
            _dgvDuyurular = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Ana Panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(_dgvDuyurular);
            mainPanel.Controls.Add(_topPanel);

            Controls.Add(mainPanel);
        }

        private void LoadSampleData()
        {
            var table = new DataTable();
            table.Columns.Add("Başlık", typeof(string));
            table.Columns.Add("Gönderen", typeof(string));
            table.Columns.Add("Tarih", typeof(DateTime));
            table.Columns.Add("Okundu", typeof(bool));

            // Örnek veriler
            table.Rows.Add("Yeni Sistem Güncellemesi", "Sistem Yöneticisi", DateTime.Now.AddDays(-2), false);
            table.Rows.Add("Toplantı Duyurusu", "Ahmet Yılmaz", DateTime.Now.AddDays(-5), true);
            table.Rows.Add("Yıllık İzin Planlaması", "İnsan Kaynakları", DateTime.Now.AddDays(-7), false);
            table.Rows.Add("Güvenlik Uyarısı", "Bilgi İşlem", DateTime.Now.AddDays(-1), true);

            _dgvDuyurular.DataSource = table;

            // Kolon başlıklarını ayarla
            if (_dgvDuyurular.Columns.Count > 0)
            {
                _dgvDuyurular.Columns["Başlık"].HeaderText = "Başlık";
                _dgvDuyurular.Columns["Gönderen"].HeaderText = "Gönderen";
                _dgvDuyurular.Columns["Tarih"].HeaderText = "Tarih";
                _dgvDuyurular.Columns["Okundu"].HeaderText = "Okundu";

                // Tarih kolonunu formatla
                _dgvDuyurular.Columns["Tarih"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            }
        }
    }
}
