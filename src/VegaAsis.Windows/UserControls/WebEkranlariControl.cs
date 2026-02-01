using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.UserControls
{
    public class WebEkranlariControl : UserControl
    {
        private readonly IWebUserService _webUserService;
        private readonly IAuthService _authService;

        private Panel _topPanel;
        private TextBox _txtAra;
        private Button _btnYeniKullanici;
        private DataGridView _dgvWebKullanicilari;
        private Panel _bottomPanel;
        private Button _btnDuzenle;
        private Button _btnSil;
        private List<WebUserDto> _webUsers;

        public WebEkranlariControl(IWebUserService webUserService = null, IAuthService authService = null)
        {
            _webUserService = webUserService;
            _authService = authService;
            SuspendLayout();
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            MinimumSize = new Size(800, 400);

            BuildTopPanel();
            BuildDataGridView();
            BuildBottomPanel();

            ResumeLayout(true);
            Load += WebEkranlariControl_Load;
        }

        private async void WebEkranlariControl_Load(object sender, EventArgs e)
        {
            if (_webUserService != null)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            var userId = _authService?.GetCurrentUserId;
            try
            {
                var list = await _webUserService.GetAllAsync(userId);
                _webUsers = new List<WebUserDto>(list ?? new WebUserDto[0]);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshGrid()
        {
            _dgvWebKullanicilari.Rows.Clear();
            if (_webUsers == null) return;
            foreach (var u in _webUsers)
            {
                var lisans = u.IsLicensed ? "Tam" : u.LicenseOfferOnly ? "Sadece Teklif" : "Yok";
                var row = _dgvWebKullanicilari.Rows[_dgvWebKullanicilari.Rows.Add()];
                row.Cells["ColWebKullanici"].Value = u.WebUsername;
                row.Cells["ColKullanici"].Value = u.Username;
                row.Cells["ColAdSoyad"].Value = u.FullName ?? "-";
                row.Cells["ColLisans"].Value = lisans;
                row.Cells["ColUnlicensed"].Value = u.UnlicensedAgentOnly ? "Evet" : "Hayır";
                row.Tag = u.Id;
            }
            ApplyFilter();
        }

        private void BuildTopPanel()
        {
            _topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(8)
            };

            var lblAra = new Label { Text = "Ara:", AutoSize = true, Location = new Point(8, 16), Font = new Font("Segoe UI", 9F) };
            _txtAra = new TextBox { Width = 200, Location = new Point(45, 13), Font = new Font("Segoe UI", 9F) };
            _txtAra.TextChanged += (s, e) => ApplyFilter();

            _btnYeniKullanici = new Button
            {
                Text = "Yeni Web Kullanıcı",
                Size = new Size(150, 32),
                Location = new Point(260, 9),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnYeniKullanici.FlatAppearance.BorderSize = 0;
            _btnYeniKullanici.Click += BtnYeniKullanici_Click;

            _topPanel.Controls.AddRange(new Control[] { lblAra, _txtAra, _btnYeniKullanici });
            Controls.Add(_topPanel);
        }

        private void BuildDataGridView()
        {
            _dgvWebKullanicilari = new DataGridView
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
            _dgvWebKullanicilari.RowTemplate.Height = 28;
            _dgvWebKullanicilari.EnableHeadersVisualStyles = false;
            _dgvWebKullanicilari.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            _dgvWebKullanicilari.Columns.Add("ColWebKullanici", "Web Kullanıcı Adı");
            _dgvWebKullanicilari.Columns.Add("ColKullanici", "Kullanıcı Adı");
            _dgvWebKullanicilari.Columns.Add("ColAdSoyad", "Ad Soyad");
            _dgvWebKullanicilari.Columns.Add("ColLisans", "Lisans");
            _dgvWebKullanicilari.Columns.Add("ColUnlicensed", "Lisanssız Acente");

            if (_webUserService == null)
            {
                _dgvWebKullanicilari.Rows.Add("web_user_1", "user1", "Ahmet Yılmaz", "Tam", "Hayır");
                _dgvWebKullanicilari.Rows.Add("web_user_2", "user2", "Mehmet Demir", "Sadece Teklif", "Hayır");
                _dgvWebKullanicilari.Rows.Add("web_user_3", "user3", "Ayşe Kaya", "Tam", "Evet");
            }

            Controls.Add(_dgvWebKullanicilari);
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

            _btnDuzenle = new Button
            {
                Text = "Düzenle",
                Size = new Size(100, 32),
                Location = new Point(8, 9),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnDuzenle.FlatAppearance.BorderSize = 0;
            _btnDuzenle.Click += BtnDuzenle_Click;

            _btnSil = new Button
            {
                Text = "Sil",
                Size = new Size(100, 32),
                Location = new Point(116, 9),
                BackColor = Color.FromArgb(198, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnSil.FlatAppearance.BorderSize = 0;
            _btnSil.Click += BtnSil_Click;

            _bottomPanel.Controls.Add(_btnDuzenle);
            _bottomPanel.Controls.Add(_btnSil);
            Controls.Add(_bottomPanel);
        }

        private void ApplyFilter()
        {
            var filter = _txtAra?.Text?.Trim().ToLower();
            if (string.IsNullOrEmpty(filter))
            {
                foreach (DataGridViewRow row in _dgvWebKullanicilari.Rows)
                {
                    row.Visible = true;
                }
                return;
            }
            foreach (DataGridViewRow row in _dgvWebKullanicilari.Rows)
            {
                var visible = false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(filter))
                    {
                        visible = true;
                        break;
                    }
                }
                row.Visible = visible;
            }
        }

        private void BtnYeniKullanici_Click(object sender, EventArgs e)
        {
            if (_webUserService == null)
            {
                MessageBox.Show("Web kullanıcı servisi bağlı değil.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var form = new Forms.WebUserForm(null, _authService))
            {
                if (form.ShowDialog(this) == DialogResult.OK && form.Dto != null)
                {
                    _ = CreateWebUserAsync(form.Dto);
                }
            }
        }

        private async Task CreateWebUserAsync(WebUserDto dto)
        {
            try
            {
                await _webUserService.CreateAsync(dto);
                MessageBox.Show("Web kullanıcısı oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oluşturma hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDuzenle_Click(object sender, EventArgs e)
        {
            if (_webUserService == null || _dgvWebKullanicilari.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlenecek bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var row = _dgvWebKullanicilari.SelectedRows[0];
            var id = row.Tag as Guid?;
            if (!id.HasValue) return;
            var user = _webUsers?.FirstOrDefault(u => u.Id == id.Value);
            if (user == null) return;

            using (var form = new Forms.WebUserForm(user, _authService))
            {
                if (form.ShowDialog(this) == DialogResult.OK && form.Dto != null)
                {
                    _ = UpdateWebUserAsync(id.Value, form.Dto);
                }
            }
        }

        private async Task UpdateWebUserAsync(Guid id, WebUserDto dto)
        {
            try
            {
                await _webUserService.UpdateAsync(id, dto);
                MessageBox.Show("Web kullanıcısı güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSil_Click(object sender, EventArgs e)
        {
            if (_webUserService == null || _dgvWebKullanicilari.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var row = _dgvWebKullanicilari.SelectedRows[0];
            var id = row.Tag as Guid?;
            if (!id.HasValue) return;

            var result = MessageBox.Show("Seçili web kullanıcıyı silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                var ok = await _webUserService.DeleteAsync(id.Value);
                if (ok)
                {
                    MessageBox.Show("Web kullanıcısı silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
