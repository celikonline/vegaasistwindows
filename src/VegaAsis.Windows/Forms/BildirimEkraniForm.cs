using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class BildirimEkraniForm : Form
    {
        private ListView _lstBildirimler;
        private Button _btnTemizle;
        private Button _btnKapat;
        private Label _lblBilgi;

        public BildirimEkraniForm()
        {
            InitializeComponent();
            OrnekBildirimYukle();
        }

        private void InitializeComponent()
        {
            Text = "Bildirimler";
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(400, 300);

            _lblBilgi = new Label
            {
                Text = "Sistem bildirimleri",
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(8, 6, 0, 0),
                Font = new Font("Segoe UI", 9F)
            };
            Controls.Add(_lblBilgi);

            _lstBildirimler = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Columns = { "Tarih", "Tip", "Başlık", "İçerik" }
            };
            _lstBildirimler.Columns[0].Width = 80;
            _lstBildirimler.Columns[1].Width = 70;
            _lstBildirimler.Columns[2].Width = 120;
            _lstBildirimler.Columns[3].Width = 200;
            _lstBildirimler.DoubleClick += LstBildirimler_DoubleClick;
            Controls.Add(_lstBildirimler);

            var pnlAlt = new Panel { Dock = DockStyle.Bottom, Height = 45 };
            _btnTemizle = new Button { Text = "Temizle", Size = new Size(80, 28), Location = new Point(12, 8) };
            _btnTemizle.Click += (s, e) => { _lstBildirimler.Items.Clear(); };
            _btnKapat = new Button { Text = "Kapat", Size = new Size(80, 28), Location = new Point(398, 8) };
            _btnKapat.Click += (s, e) => Close();
            pnlAlt.Controls.Add(_btnTemizle);
            pnlAlt.Controls.Add(_btnKapat);
            Controls.Add(pnlAlt);
        }

        private void LstBildirimler_DoubleClick(object sender, EventArgs e)
        {
            if (_lstBildirimler.SelectedItems.Count == 0) return;
            var item = _lstBildirimler.SelectedItems[0];
            var baslik = item.SubItems.Count > 2 ? item.SubItems[2].Text : "";
            var icerik = item.SubItems.Count > 3 ? item.SubItems[3].Text : "";
            MessageBox.Show(icerik, string.IsNullOrEmpty(baslik) ? "Bildirim" : baslik, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Tek bir bildirim göstermek için kullanılır.
        /// </summary>
        public static void ShowSingle(string baslik, string icerik, Form owner = null)
        {
            using (var f = new BildirimEkraniForm())
            {
                f.SetSingleNotification(baslik ?? "Bildirim", icerik ?? "");
                if (owner != null)
                    f.ShowDialog(owner);
                else
                    f.ShowDialog();
            }
        }

        /// <summary>
        /// Listeyi temizleyip tek bildirim gösterir (ShowSingle tarafından kullanılır).
        /// </summary>
        public void SetSingleNotification(string baslik, string icerik)
        {
            _lstBildirimler.Items.Clear();
            _lstBildirimler.Items.Add(new ListViewItem(new[] { DateTime.Now.ToString("dd.MM HH:mm"), "Bilgi", baslik, icerik }));
        }

        private void OrnekBildirimYukle()
        {
            var items = new[]
            {
                new[] { DateTime.Now.AddHours(-1).ToString("dd.MM HH:mm"), "Bilgi", "Hoş geldiniz", "VegaAsis uygulamasına giriş yaptınız." },
                new[] { DateTime.Now.AddHours(-2).ToString("dd.MM HH:mm"), "Uyarı", "Güncelleme", "Yeni sürüm mevcut." },
                new[] { DateTime.Now.AddDays(-1).ToString("dd.MM HH:mm"), "Duyuru", "Bakım", "Pazar gecesi bakım planlandı." }
            };
            foreach (var row in items)
            {
                _lstBildirimler.Items.Add(new ListViewItem(row));
            }
        }
    }
}
