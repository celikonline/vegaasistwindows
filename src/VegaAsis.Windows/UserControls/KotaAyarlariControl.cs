using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.UserControls
{
    public class KotaAyarlariControl : UserControl
    {
        private readonly IQuotaSettingsService _quotaSettingsService;
        private readonly IUserManagementService _userManagementService;
        private Panel _topPanel;
        private ComboBox _cmbKullanici;
        private GroupBox _grpGunlukLimitler;
        private GroupBox _grpAylikLimitler;
        private Button _btnKaydet;
        private Button _btnKaldir;

        private NumericUpDown _nudGunlukSorgu;
        private ProgressBar _pbGunlukSorgu;
        private NumericUpDown _nudGunlukTeklif;
        private ProgressBar _pbGunlukTeklif;
        private NumericUpDown _nudGunlukPolice;
        private ProgressBar _pbGunlukPolice;

        private NumericUpDown _nudAylikSorgu;
        private ProgressBar _pbAylikSorgu;
        private NumericUpDown _nudAylikTeklif;
        private ProgressBar _pbAylikTeklif;
        private NumericUpDown _nudAylikPolice;
        private ProgressBar _pbAylikPolice;

        public KotaAyarlariControl(IQuotaSettingsService quotaSettingsService = null, IUserManagementService userManagementService = null)
        {
            _quotaSettingsService = quotaSettingsService;
            _userManagementService = userManagementService;
            SuspendLayout();
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            MinimumSize = new Size(700, 500);
            Padding = new Padding(8);

            BuildTopPanel();
            BuildGunlukLimitler();
            BuildAylikLimitler();
            BuildKaydetButton();

            ResumeLayout(true);
            Load += KotaAyarlariControl_Load;
        }

        private async void KotaAyarlariControl_Load(object sender, EventArgs e)
        {
            await LoadKullaniciListAsync();
        }

        private async Task LoadKullaniciListAsync()
        {
            try
            {
                _cmbKullanici.Items.Clear();
                _cmbKullanici.Items.Add(new ComboItem { UserId = Guid.Empty, Display = "Kullanıcı Seçiniz" });
                if (_userManagementService != null)
                {
                    await _userManagementService.FetchUsersAsync();
                    foreach (var u in _userManagementService.Users ?? Enumerable.Empty<UserDataDto>())
                    {
                        _cmbKullanici.Items.Add(new ComboItem { UserId = u.UserId, Display = u.FullName ?? u.Email ?? u.UserId.ToString() });
                    }
                }
                else if (_quotaSettingsService != null)
                {
                    var list = await _quotaSettingsService.GetAllAsync();
                    foreach (var q in list)
                    {
                        _cmbKullanici.Items.Add(new ComboItem { UserId = q.UserId, Display = q.UserId.ToString() });
                    }
                }
                _cmbKullanici.DisplayMember = "Display";
                _cmbKullanici.ValueMember = "UserId";
                _cmbKullanici.SelectedIndex = 0;
            }
            catch { }
        }

        private class ComboItem
        {
            public Guid UserId { get; set; }
            public string Display { get; set; }
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

            var lblKullanici = new Label
            {
                Text = "Kullanıcı:",
                AutoSize = true,
                Location = new Point(8, 16),
                Font = new Font("Segoe UI", 9F)
            };

            _cmbKullanici = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200,
                Location = new Point(80, 13),
                Font = new Font("Segoe UI", 9F)
            };
            _cmbKullanici.SelectedIndexChanged += CmbKullanici_SelectedIndexChanged;

            _topPanel.Controls.Add(lblKullanici);
            _topPanel.Controls.Add(_cmbKullanici);
            Controls.Add(_topPanel);
        }

        private void BuildGunlukLimitler()
        {
            _grpGunlukLimitler = new GroupBox
            {
                Text = "Günlük Limitler",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(8, 58),
                Size = new Size(320, 180),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            int y = 24;
            y = AddLimitRow(_grpGunlukLimitler, "Sorgu Limiti:", y, out _nudGunlukSorgu, out _pbGunlukSorgu);
            y = AddLimitRow(_grpGunlukLimitler, "Teklif Limiti:", y, out _nudGunlukTeklif, out _pbGunlukTeklif);
            y = AddLimitRow(_grpGunlukLimitler, "Poliçe Limiti:", y, out _nudGunlukPolice, out _pbGunlukPolice);

            Controls.Add(_grpGunlukLimitler);
        }

        private void BuildAylikLimitler()
        {
            _grpAylikLimitler = new GroupBox
            {
                Text = "Aylık Limitler",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(340, 58),
                Size = new Size(320, 180),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            int y = 24;
            y = AddLimitRow(_grpAylikLimitler, "Sorgu Limiti:", y, out _nudAylikSorgu, out _pbAylikSorgu);
            y = AddLimitRow(_grpAylikLimitler, "Teklif Limiti:", y, out _nudAylikTeklif, out _pbAylikTeklif);
            y = AddLimitRow(_grpAylikLimitler, "Poliçe Limiti:", y, out _nudAylikPolice, out _pbAylikPolice);

            Controls.Add(_grpAylikLimitler);
        }

        private int AddLimitRow(GroupBox parent, string labelText, int y, out NumericUpDown nud, out ProgressBar pb)
        {
            var lbl = new Label
            {
                Text = labelText,
                AutoSize = true,
                Location = new Point(8, y),
                Font = new Font("Segoe UI", 9F)
            };

            nud = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 10000,
                Value = 100,
                Width = 100,
                Location = new Point(100, y - 2),
                Font = new Font("Segoe UI", 9F)
            };

            pb = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Width = 180,
                Height = 20,
                Location = new Point(210, y),
                Style = ProgressBarStyle.Continuous
            };

            parent.Controls.Add(lbl);
            parent.Controls.Add(nud);
            parent.Controls.Add(pb);

            return y + 50;
        }

        private void UpdateProgressBar(NumericUpDown nud, ProgressBar pb)
        {
            if (pb != null && nud != null && nud.Maximum > 0)
            {
                var usage = (int)((nud.Value / nud.Maximum) * 100);
                pb.Value = Math.Min(usage, 100);
            }
        }

        private void BuildKaydetButton()
        {
            _btnKaydet = new Button
            {
                Text = "Kaydet",
                Size = new Size(120, 32),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            _btnKaydet.FlatAppearance.BorderSize = 0;
            _btnKaydet.Click += BtnKaydet_Click;

            _btnKaldir = new Button
            {
                Text = "Kota Kısıtını Kaldır",
                Size = new Size(140, 32),
                BackColor = Color.FromArgb(198, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            _btnKaldir.FlatAppearance.BorderSize = 0;
            _btnKaldir.Click += BtnKaldir_Click;

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
            btnPanel.Controls.Add(_btnKaydet);
            btnPanel.Controls.Add(_btnKaldir);
            Controls.Add(btnPanel);
        }

        private async void CmbKullanici_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = _cmbKullanici.SelectedItem as ComboItem;
            if (item == null || item.UserId == Guid.Empty || _quotaSettingsService == null) return;
            try
            {
                var quota = await _quotaSettingsService.GetByUserAsync(item.UserId);
                if (quota != null)
                {
                    _nudGunlukSorgu.Value = Math.Min(quota.DailyQueryLimit, _nudGunlukSorgu.Maximum);
                    _nudAylikSorgu.Value = Math.Min(quota.MonthlyQueryLimit, _nudAylikSorgu.Maximum);
                    _nudGunlukTeklif.Value = Math.Min(quota.DailyOfferLimit, _nudGunlukTeklif.Maximum);
                    _nudAylikTeklif.Value = Math.Min(quota.MonthlyOfferLimit, _nudAylikTeklif.Maximum);
                    _nudGunlukPolice.Value = Math.Min(quota.DailyPolicyLimit, _nudGunlukPolice.Maximum);
                    _nudAylikPolice.Value = Math.Min(quota.MonthlyPolicyLimit, _nudAylikPolice.Maximum);
                    _pbGunlukSorgu.Value = quota.DailyQueryLimit > 0 ? Math.Min(100, (quota.CurrentDailyQueries * 100) / quota.DailyQueryLimit) : 0;
                    _pbAylikSorgu.Value = quota.MonthlyQueryLimit > 0 ? Math.Min(100, (quota.CurrentMonthlyQueries * 100) / quota.MonthlyQueryLimit) : 0;
                }
                else
                {
                    _nudGunlukSorgu.Value = 100;
                    _nudAylikSorgu.Value = 3000;
                    _nudGunlukTeklif.Value = 50;
                    _nudAylikTeklif.Value = 1500;
                    _nudGunlukPolice.Value = 20;
                    _nudAylikPolice.Value = 600;
                }
            }
            catch { }
        }


        private async void BtnKaydet_Click(object sender, EventArgs e)
        {
            var item = _cmbKullanici.SelectedItem as ComboItem;
            if (item == null || item.UserId == Guid.Empty)
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_quotaSettingsService == null)
            {
                MessageBox.Show("Kota servisi kullanılamıyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var dto = new QuotaSettingsDto
                {
                    UserId = item.UserId,
                    DailyQueryLimit = (int)_nudGunlukSorgu.Value,
                    MonthlyQueryLimit = (int)_nudAylikSorgu.Value,
                    DailyOfferLimit = (int)_nudGunlukTeklif.Value,
                    MonthlyOfferLimit = (int)_nudAylikTeklif.Value,
                    DailyPolicyLimit = (int)_nudGunlukPolice.Value,
                    MonthlyPolicyLimit = (int)_nudAylikPolice.Value
                };
                await _quotaSettingsService.UpdateAsync(dto);
                MessageBox.Show("Kota ayarları kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydetme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnKaldir_Click(object sender, EventArgs e)
        {
            var item = _cmbKullanici.SelectedItem as ComboItem;
            if (item == null || item.UserId == Guid.Empty)
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_quotaSettingsService == null) return;
            if (MessageBox.Show("Bu kullanıcının kota kısıtını kaldırmak istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                var ok = await _quotaSettingsService.DeleteAsync(item.UserId);
                if (ok)
                {
                    MessageBox.Show("Kota kısıtı kaldırıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _nudGunlukSorgu.Value = 100;
                    _nudAylikSorgu.Value = 3000;
                    _nudGunlukTeklif.Value = 50;
                    _nudAylikTeklif.Value = 1500;
                    _nudGunlukPolice.Value = 20;
                    _nudAylikPolice.Value = 600;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
