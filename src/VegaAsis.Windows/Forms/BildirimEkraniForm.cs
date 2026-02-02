using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Windows;

namespace VegaAsis.Windows.Forms
{
    public class BildirimEkraniForm : Form
    {
        private ListView _lstBildirimler;
        private Button _btnTemizle;
        private Button _btnKapat;
        private Label _lblBilgi;
        private CheckBox _chkUnreadOnly;
        private Label _lblSayac;
        private List<BildirimDto> _currentList = new List<BildirimDto>();

        public BildirimEkraniForm()
        {
            InitializeComponent();
            Load += BildirimEkraniForm_Load;
        }

        private async void BildirimEkraniForm_Load(object sender, EventArgs e)
        {
            if (ServiceLocator.IsInitialized)
            {
                await VerileriYukleAsync().ConfigureAwait(true);
            }
            else
            {
                OrnekBildirimYukle();
            }
        }

        private async Task VerileriYukleAsync()
        {
            try
            {
                var service = ServiceLocator.Resolve<IBildirimService>();
                string role = null;
                Guid? userId = null;
                try
                {
                    var auth = ServiceLocator.Resolve<IAuthService>();
                    role = auth?.GetCurrentProfile()?.Role;
                    userId = auth?.GetCurrentUserId;
                }
                catch { }
                var list = await service.GetAllByRoleAsync(role, userId).ConfigureAwait(true);
                _currentList = list != null ? list.ToList() : new List<BildirimDto>();
                ApplyFilterAndRender();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bildirimler yüklenirken hata: " + ex.Message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                OrnekBildirimYukle();
            }
        }

        private void InitializeComponent()
        {
            Text = "Bildirimler";
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(400, 300);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 32, Padding = new Padding(8, 6, 8, 0) };
            _lblBilgi = new Label
            {
                Text = "Sistem bildirimleri",
                AutoSize = true,
                Left = 0,
                Top = 6,
                Font = new Font("Segoe UI", 9F)
            };
            _chkUnreadOnly = new CheckBox
            {
                Text = "Sadece okunmamış",
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Top = 4
            };
            _chkUnreadOnly.CheckedChanged += (s, e) => ApplyFilterAndRender();
            _lblSayac = new Label
            {
                Text = "Okunmamış: 0 / Toplam: 0",
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Top = 6
            };
            pnlTop.Controls.Add(_lblBilgi);
            pnlTop.Controls.Add(_chkUnreadOnly);
            pnlTop.Controls.Add(_lblSayac);
            Controls.Add(pnlTop);

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
            _btnTemizle = new Button { Text = "Temizle", Size = new Size(80, 28), Location = new Point(12, 8), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            _btnTemizle.Click += (s, e) => { _currentList.Clear(); ApplyFilterAndRender(); };
            _btnKapat = new Button { Text = "Kapat", Size = new Size(80, 28), Location = new Point(398, 8), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            _btnKapat.Click += (s, e) => Close();
            pnlAlt.Controls.Add(_btnTemizle);
            pnlAlt.Controls.Add(_btnKapat);
            Controls.Add(pnlAlt);
            pnlTop.Resize += (s, e) =>
            {
                var right = pnlTop.ClientSize.Width - 8;
                _lblSayac.Left = right - _lblSayac.Width;
                _chkUnreadOnly.Left = _lblSayac.Left - _chkUnreadOnly.Width - 12;
            };
        }

        private async void LstBildirimler_DoubleClick(object sender, EventArgs e)
        {
            if (_lstBildirimler.SelectedItems.Count == 0) return;
            var item = _lstBildirimler.SelectedItems[0];
            var baslik = item.SubItems.Count > 2 ? item.SubItems[2].Text : "";
            var icerik = item.SubItems.Count > 3 ? item.SubItems[3].Text : "";
            MessageBox.Show(icerik, string.IsNullOrEmpty(baslik) ? "Bildirim" : baslik, MessageBoxButtons.OK, MessageBoxIcon.Information);
            await MarkReadAsync(item).ConfigureAwait(true);
        }

        private async Task MarkReadAsync(ListViewItem item)
        {
            try
            {
                if (item == null || item.Tag == null) return;
                var announcementId = (Guid)item.Tag;
                if (!ServiceLocator.IsInitialized) return;
                var auth = ServiceLocator.Resolve<IAuthService>();
                var userId = auth?.GetCurrentUserId;
                if (!userId.HasValue) return;
                var service = ServiceLocator.Resolve<IBildirimService>();
                await service.MarkAsReadAsync(announcementId, userId.Value).ConfigureAwait(true);
                var dto = _currentList.FirstOrDefault(x => x.Id.HasValue && x.Id.Value == announcementId);
                if (dto != null) dto.IsRead = true;
                ApplyFilterAndRender();
            }
            catch
            {
                // okundu işaretleme hatası yoksayılır
            }
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
            _currentList = new List<BildirimDto>
            {
                new BildirimDto
                {
                    Id = Guid.NewGuid(),
                    Tarih = DateTime.Now,
                    Tip = "Bilgi",
                    Baslik = baslik,
                    Icerik = icerik,
                    IsRead = false
                }
            };
            ApplyFilterAndRender();
        }

        private void OrnekBildirimYukle()
        {
            _currentList = new List<BildirimDto>
            {
                new BildirimDto
                {
                    Id = Guid.NewGuid(),
                    Tarih = DateTime.Now.AddHours(-1),
                    Tip = "Bilgi",
                    Baslik = "Hoş geldiniz",
                    Icerik = "VegaAsis uygulamasına giriş yaptınız.",
                    IsRead = false
                },
                new BildirimDto
                {
                    Id = Guid.NewGuid(),
                    Tarih = DateTime.Now.AddHours(-2),
                    Tip = "Uyarı",
                    Baslik = "Güncelleme",
                    Icerik = "Yeni sürüm mevcut.",
                    IsRead = true
                },
                new BildirimDto
                {
                    Id = Guid.NewGuid(),
                    Tarih = DateTime.Now.AddDays(-1),
                    Tip = "Duyuru",
                    Baslik = "Bakım",
                    Icerik = "Pazar gecesi bakım planlandı.",
                    IsRead = true
                }
            };
            ApplyFilterAndRender();
        }

        private void ApplyFilterAndRender()
        {
            _lstBildirimler.Items.Clear();
            var list = _currentList ?? new List<BildirimDto>();
            var showUnreadOnly = _chkUnreadOnly != null && _chkUnreadOnly.Checked;
            foreach (var dto in list.Where(x => !showUnreadOnly || !x.IsRead))
            {
                var tarihStr = dto.Tarih.HasValue ? dto.Tarih.Value.ToString("dd.MM HH:mm") : "";
                var item = new ListViewItem(new[] { tarihStr, dto.Tip ?? "", dto.Baslik ?? "", dto.Icerik ?? "" });
                if (dto.Id.HasValue) item.Tag = dto.Id.Value;
                if (!dto.IsRead)
                    item.Font = new Font(_lstBildirimler.Font, FontStyle.Bold);
                _lstBildirimler.Items.Add(item);
            }
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            if (_lblSayac == null) return;
            var total = _currentList != null ? _currentList.Count : 0;
            var unread = _currentList != null ? _currentList.Count(x => !x.IsRead) : 0;
            _lblSayac.Text = "Okunmamış: " + unread + " / Toplam: " + total;
        }
    }
}
