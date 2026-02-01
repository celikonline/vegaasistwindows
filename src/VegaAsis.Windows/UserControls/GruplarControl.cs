using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.UserControls
{
    public class GruplarControl : UserControl
    {
        private readonly IUserGroupService _userGroupService;
        private readonly IUserManagementService _userManagementService;
        private readonly IAuthService _authService;
        private SplitContainer _splitContainer;
        private bool _splitterInitialized;
        private Label _lblGruplar;
        private ListBox _lstGruplar;
        private Button _btnGrupEkle;
        private Button _btnGrupSil;

        private Label _lblGrupUyeleri;
        private ListBox _lstUyeler;
        private Button _btnUyeEkle;
        private Button _btnUyeCikar;

        private List<UserGroupDto> _gruplar = new List<UserGroupDto>();
        private List<GroupMemberDto> _uyeler = new List<GroupMemberDto>();

        public GruplarControl(IUserGroupService userGroupService = null, IUserManagementService userManagementService = null, IAuthService authService = null)
        {
            _userGroupService = userGroupService;
            _userManagementService = userManagementService;
            _authService = authService;
            SuspendLayout();
            Dock = DockStyle.Fill;
            BackColor = SystemColors.Control;
            Padding = new Padding(8);
            MinimumSize = new Size(600, 400);

            InitializeComponents();

            ResumeLayout(true);
            Load += GruplarControl_Load;
        }

        private async void GruplarControl_Load(object sender, EventArgs e)
        {
            if (_userGroupService != null)
            {
                await LoadGruplarAsync();
            }
        }

        private async Task LoadGruplarAsync()
        {
            if (_userGroupService == null) return;
            try
            {
                _gruplar = (await _userGroupService.GetAllAsync()).ToList();
                _lstGruplar.Items.Clear();
                _lstGruplar.DataSource = null;
                _lstGruplar.DataSource = _gruplar;
                _lstGruplar.DisplayMember = "GroupName";
            }
            catch { }
        }

        private void InitializeComponents()
        {
            // SplitContainer (Vertical)
            _splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                Panel1MinSize = 0,
                Panel2MinSize = 0,
                SplitterDistance = 0,
                BackColor = SystemColors.Control
            };
            _splitContainer.Resize += SplitContainer_Resize;

            // Panel1 (Sol - Gruplar)
            _splitContainer.Panel1.SuspendLayout();
            _splitContainer.Panel1.Padding = new Padding(4);
            _splitContainer.Panel1.BackColor = SystemColors.Window;

            _lblGruplar = new Label
            {
                Text = "Gruplar",
                Location = new Point(4, 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            _lstGruplar = new ListBox
            {
                Location = new Point(4, 28),
                Size = new Size(192, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F)
            };

            _lstGruplar.DisplayMember = "GroupName";

            _btnGrupEkle = new Button
            {
                Text = "Grup Ekle",
                Size = new Size(90, 28),
                Location = new Point(4, 332),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };
            _btnGrupEkle.Click += BtnGrupEkle_Click;

            _btnGrupSil = new Button
            {
                Text = "Grup Sil",
                Size = new Size(90, 28),
                Location = new Point(106, 332),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };
            _btnGrupSil.Click += BtnGrupSil_Click;

            _splitContainer.Panel1.Controls.Add(_lblGruplar);
            _splitContainer.Panel1.Controls.Add(_lstGruplar);
            _splitContainer.Panel1.Controls.Add(_btnGrupEkle);
            _splitContainer.Panel1.Controls.Add(_btnGrupSil);
            _splitContainer.Panel1.ResumeLayout(true);

            // Panel2 (Sağ - Grup Üyeleri)
            _splitContainer.Panel2.SuspendLayout();
            _splitContainer.Panel2.Padding = new Padding(4);
            _splitContainer.Panel2.BackColor = SystemColors.Window;

            _lblGrupUyeleri = new Label
            {
                Text = "Grup Üyeleri",
                Location = new Point(4, 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            _lstUyeler = new ListBox
            {
                Location = new Point(4, 28),
                Size = new Size(380, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F)
            };

            _btnUyeEkle = new Button
            {
                Text = "Üye Ekle",
                Size = new Size(90, 28),
                Location = new Point(4, 332),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };
            _btnUyeEkle.Click += BtnUyeEkle_Click;

            _btnUyeCikar = new Button
            {
                Text = "Üye Çıkar",
                Size = new Size(90, 28),
                Location = new Point(106, 332),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };
            _btnUyeCikar.Click += BtnUyeCikar_Click;

            _splitContainer.Panel2.Controls.Add(_lblGrupUyeleri);
            _splitContainer.Panel2.Controls.Add(_lstUyeler);
            _splitContainer.Panel2.Controls.Add(_btnUyeEkle);
            _splitContainer.Panel2.Controls.Add(_btnUyeCikar);
            _splitContainer.Panel2.ResumeLayout(true);

            Controls.Add(_splitContainer);

            // Grup seçildiğinde üyeleri yükle
            _lstGruplar.SelectedIndexChanged += LstGruplar_SelectedIndexChanged;
        }

        private void SplitContainer_Resize(object sender, EventArgs e)
        {
            if (_splitterInitialized) return;
            const int panel1Min = 150;
            const int panel2Min = 300;
            int w = _splitContainer.Width;
            if (w < panel1Min + panel2Min) return;
            _splitterInitialized = true;
            _splitContainer.Resize -= SplitContainer_Resize;
            _splitContainer.Panel1MinSize = panel1Min;
            _splitContainer.Panel2MinSize = panel2Min;
            _splitContainer.SplitterDistance = Math.Max(panel1Min, Math.Min(200, w - panel2Min));
        }

        private async void LstGruplar_SelectedIndexChanged(object sender, EventArgs e)
        {
            var g = _lstGruplar.SelectedItem as UserGroupDto;
            if (g == null || _userGroupService == null)
            {
                _lstUyeler.Items.Clear();
                return;
            }
            try
            {
                _uyeler = (await _userGroupService.GetMembersAsync(g.Id)).ToList();
                _lstUyeler.Items.Clear();
                foreach (var m in _uyeler)
                {
                    var name = m.MemberUserId.ToString();
                    if (_userManagementService?.Users != null)
                    {
                        var u = _userManagementService.Users.FirstOrDefault(x => x.UserId == m.MemberUserId);
                        if (u != null) name = u.FullName ?? u.Email ?? name;
                    }
                    _lstUyeler.Items.Add(new MemberItem { MemberUserId = m.MemberUserId, Display = name });
                }
            }
            catch { }
        }

        private class MemberItem
        {
            public Guid MemberUserId { get; set; }
            public string Display { get; set; }
            public override string ToString() => Display ?? MemberUserId.ToString();
        }

        private async void BtnGrupEkle_Click(object sender, EventArgs e)
        {
            if (_userGroupService == null || _authService == null)
            {
                MessageBox.Show("Servis kullanılamıyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var grupAdi = Interaction.InputBox("Grup adını girin:", "Yeni Grup", "");
            if (string.IsNullOrWhiteSpace(grupAdi)) return;
            try
            {
                var userId = _authService.GetCurrentUserId ?? Guid.Empty;
                if (userId == Guid.Empty)
                {
                    MessageBox.Show("Giriş yapmanız gerekiyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var dto = new UserGroupDto { UserId = userId, GroupName = grupAdi.Trim(), Description = "" };
                await _userGroupService.CreateAsync(dto);
                MessageBox.Show("Grup oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadGruplarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnGrupSil_Click(object sender, EventArgs e)
        {
            var g = _lstGruplar.SelectedItem as UserGroupDto;
            if (g == null)
            {
                MessageBox.Show("Lütfen silmek için bir grup seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_userGroupService == null) return;
            if (MessageBox.Show("Seçili grubu silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                await _userGroupService.DeleteAsync(g.Id);
                MessageBox.Show("Grup silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadGruplarAsync();
                _lstUyeler.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnUyeEkle_Click(object sender, EventArgs e)
        {
            var g = _lstGruplar.SelectedItem as UserGroupDto;
            if (g == null)
            {
                MessageBox.Show("Lütfen önce bir grup seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_userGroupService == null || _userManagementService == null) return;
            try
            {
                await _userManagementService.FetchUsersAsync();
                var users = _userManagementService.Users?.ToList() ?? new List<UserDataDto>();
                if (users.Count == 0)
                {
                    MessageBox.Show("Kullanıcı listesi yüklenemedi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                using (var f = new Form { Text = "Üye Ekle", Size = new Size(300, 400), StartPosition = FormStartPosition.CenterParent })
                {
                    var lst = new ListBox { Dock = DockStyle.Fill, DisplayMember = "FullName", ValueMember = "UserId" };
                    lst.DataSource = users;
                    var panel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
                    var btnOk = new Button { Text = "Ekle", DialogResult = DialogResult.OK };
                    panel.Controls.Add(btnOk);
                    f.Controls.Add(lst);
                    f.Controls.Add(panel);
                    f.AcceptButton = btnOk;
                    if (f.ShowDialog() == DialogResult.OK && lst.SelectedItem is UserDataDto sel)
                    {
                        await _userGroupService.AddMemberAsync(g.Id, sel.UserId);
                        MessageBox.Show("Üye eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LstGruplar_SelectedIndexChanged(null, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnUyeCikar_Click(object sender, EventArgs e)
        {
            var g = _lstGruplar.SelectedItem as UserGroupDto;
            var m = _lstUyeler.SelectedItem as MemberItem;
            if (g == null || m == null)
            {
                MessageBox.Show("Lütfen grup ve çıkarılacak üyeyi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_userGroupService == null) return;
            if (MessageBox.Show("Seçili üyeyi gruptan çıkarmak istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                await _userGroupService.RemoveMemberAsync(g.Id, m.MemberUserId);
                MessageBox.Show("Üye gruptan çıkarıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LstGruplar_SelectedIndexChanged(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
