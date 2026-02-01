using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.UserControls
{
    public class BenchmarkControl : UserControl
    {
        private Panel _topPanel;
        private Button _btnTestCalistir;
        private Label _lblSonTest;
        private DataGridView _dgvBenchmark;
        private Panel _bottomPanel;
        private ProgressBar _progressBar;
        private Label _lblDurum;

        public BenchmarkControl()
        {
            SuspendLayout();
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            MinimumSize = new Size(800, 500);

            BuildTopPanel();
            BuildDataGridView();
            BuildBottomPanel();

            ResumeLayout(true);
        }

        private void BuildTopPanel()
        {
            _topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(8)
            };

            _btnTestCalistir = new Button
            {
                Text = "Test Çalıştır",
                Size = new Size(120, 32),
                Location = new Point(8, 14),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnTestCalistir.FlatAppearance.BorderSize = 0;
            _btnTestCalistir.Click += BtnTestCalistir_Click;

            _lblSonTest = new Label
            {
                Text = "Son Test: -",
                AutoSize = true,
                Location = new Point(140, 20),
                Font = new Font("Segoe UI", 9F)
            };

            _topPanel.Controls.Add(_btnTestCalistir);
            _topPanel.Controls.Add(_lblSonTest);
            Controls.Add(_topPanel);
        }

        private void BuildDataGridView()
        {
            _dgvBenchmark = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 32
            };
            _dgvBenchmark.RowTemplate.Height = 28;
            _dgvBenchmark.EnableHeadersVisualStyles = false;
            _dgvBenchmark.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            _dgvBenchmark.ColumnHeadersDefaultCellStyle.SelectionBackColor = _dgvBenchmark.ColumnHeadersDefaultCellStyle.BackColor;

            _dgvBenchmark.Columns.Add("TestAdi", "Test Adı");
            _dgvBenchmark.Columns.Add("Sure", "Süre (ms)");
            _dgvBenchmark.Columns.Add("Sonuc", "Sonuç");
            _dgvBenchmark.Columns.Add("Detay", "Detay");

            LoadExampleData();

            Controls.Add(_dgvBenchmark);
        }

        private void LoadExampleData()
        {
            _dgvBenchmark.Rows.Add("DB Bağlantısı", "45", "Başarılı", "PostgreSQL bağlantısı test edildi");
            _dgvBenchmark.Rows.Add("API Yanıt", "120", "Başarılı", "REST API yanıt süresi ölçüldü");
            _dgvBenchmark.Rows.Add("Şirket Sorgu", "230", "Başarılı", "Şirket listesi sorgulandı");
            _dgvBenchmark.Rows.Add("Teklif Oluşturma", "450", "Başarılı", "Yeni teklif oluşturuldu");
            _dgvBenchmark.Rows.Add("Poliçe Sorgu", "180", "Başarılı", "Poliçe bilgileri getirildi");
        }

        private void BuildBottomPanel()
        {
            _bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(8)
            };

            _progressBar = new ProgressBar
            {
                Size = new Size(300, 23),
                Location = new Point(8, 13),
                Style = ProgressBarStyle.Continuous
            };

            _lblDurum = new Label
            {
                Text = "Hazır",
                AutoSize = true,
                Location = new Point(320, 18),
                Font = new Font("Segoe UI", 9F)
            };

            _bottomPanel.Controls.Add(_progressBar);
            _bottomPanel.Controls.Add(_lblDurum);
            Controls.Add(_bottomPanel);
        }

        private void BtnTestCalistir_Click(object sender, EventArgs e)
        {
            _btnTestCalistir.Enabled = false;
            _lblDurum.Text = "Test çalışıyor...";
            _progressBar.Style = ProgressBarStyle.Marquee;
            _progressBar.MarqueeAnimationSpeed = 30;

            var timer = new Timer { Interval = 2000 };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                timer.Dispose();
                _progressBar.Style = ProgressBarStyle.Continuous;
                _progressBar.Value = 100;
                _lblDurum.Text = "Hazır";
                _lblSonTest.Text = "Son Test: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                _btnTestCalistir.Enabled = true;
            };
            timer.Start();
        }
    }
}
