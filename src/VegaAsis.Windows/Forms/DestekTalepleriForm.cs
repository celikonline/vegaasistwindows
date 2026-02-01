using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class DestekTalepleriForm : Form
    {
        private Panel _topPanel;
        private ComboBox _cmbDurum;
        private TextBox _txtArama;
        private Button _btnYeniTalep;
        private DataGridView _dgvTalepler;

        public DestekTalepleriForm()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void InitializeComponent()
        {
            Text = "Destek Talepleri";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;

            // Üst Panel
            _topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            // Durum ComboBox
            _cmbDurum = new ComboBox
            {
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbDurum.Items.AddRange(new object[] { "Tümü", "Açık", "Beklemede", "Çözüldü" });
            _cmbDurum.SelectedIndex = 0;

            // Arama TextBox
            _txtArama = new TextBox
            {
                Width = 200,
                Text = ""
            };

            // Yeni Talep Button
            _btnYeniTalep = new Button
            {
                Text = "Yeni Talep",
                Width = 100
            };
            _btnYeniTalep.Click += BtnYeniTalep_Click;

            // Üst Panel'e kontrolleri ekle
            var flowLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0)
            };
            flowLayout.Controls.Add(_cmbDurum);
            flowLayout.Controls.Add(_txtArama);
            flowLayout.Controls.Add(_btnYeniTalep);
            _topPanel.Controls.Add(flowLayout);

            // DataGridView
            _dgvTalepler = new DataGridView
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
            mainPanel.Controls.Add(_dgvTalepler);
            mainPanel.Controls.Add(_topPanel);

            Controls.Add(mainPanel);
        }

        private void LoadSampleData()
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Konu", typeof(string));
            table.Columns.Add("Durum", typeof(string));
            table.Columns.Add("Tarih", typeof(DateTime));
            table.Columns.Add("Öncelik", typeof(string));

            // Örnek veriler
            table.Rows.Add(1, "Sistem yavaşlığı sorunu", "Açık", DateTime.Now.AddDays(-2), "Yüksek");
            table.Rows.Add(2, "Rapor oluşturma hatası", "Beklemede", DateTime.Now.AddDays(-5), "Orta");
            table.Rows.Add(3, "Kullanıcı yetkilendirme sorunu", "Çözüldü", DateTime.Now.AddDays(-10), "Düşük");
            table.Rows.Add(4, "Veri aktarım hatası", "Açık", DateTime.Now.AddDays(-1), "Yüksek");

            _dgvTalepler.DataSource = table;
        }

        private void BtnYeniTalep_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Yeni talep oluşturma özelliği yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
