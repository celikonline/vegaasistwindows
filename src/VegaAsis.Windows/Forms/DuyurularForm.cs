using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Windows;

namespace VegaAsis.Windows.Forms
{
    public class DuyurularForm : Form
    {
        private Panel _topPanel;
        private ComboBox _cmbOkunmaDurumu;
        private TextBox _txtArama;
        private Button _btnDuyuruGonder;
        private DataGridView _dgvDuyurular;

        private IAnnouncementService _announcementService;
        private IAuthService _authService;

        public DuyurularForm()
        {
            InitializeComponent();
            if (ServiceLocator.IsInitialized)
            {
                _announcementService = ServiceLocator.Resolve<IAnnouncementService>();
                _authService = ServiceLocator.Resolve<IAuthService>();
            }
            Shown += DuyurularForm_Shown;
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
            _btnDuyuruGonder.Click += BtnDuyuruGonder_Click;

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

        private async void DuyurularForm_Shown(object sender, EventArgs e)
        {
            await LoadAnnouncementsAsync().ConfigureAwait(true);
        }

        private async Task LoadAnnouncementsAsync()
        {
            if (_announcementService == null)
            {
                LoadSampleData();
                return;
            }
            try
            {
                var userId = _authService?.GetCurrentUserId;
                var list = await _announcementService.GetAllAsync(userId).ConfigureAwait(true);
                var table = new DataTable();
                table.Columns.Add("Başlık", typeof(string));
                table.Columns.Add("İçerik", typeof(string));
                table.Columns.Add("Gönderen", typeof(string));
                table.Columns.Add("Tarih", typeof(DateTime));
                table.Columns.Add("Hedef", typeof(string));

                IUserManagementService userMgmt = null;
                if (ServiceLocator.IsInitialized)
                {
                    try { userMgmt = ServiceLocator.Resolve<IUserManagementService>(); }
                    catch { /* optional */ }
                }
                foreach (var dto in list)
                {
                    var gonderen = "Sistem";
                    if (dto.CreatedBy.HasValue && userMgmt?.Users != null)
                    {
                        var user = userMgmt.Users.FirstOrDefault(u => u.UserId == dto.CreatedBy.Value);
                        if (user != null)
                            gonderen = user.FullName ?? gonderen;
                    }
                    var icerikKisa = string.IsNullOrEmpty(dto.Content) ? "" : (dto.Content.Length > 80 ? dto.Content.Substring(0, 80) + "..." : dto.Content);
                    table.Rows.Add(dto.Title, icerikKisa, gonderen, dto.CreatedAt, dto.TargetRole ?? "Tümü");
                }

                _dgvDuyurular.DataSource = table;
                if (_dgvDuyurular.Columns.Count > 0)
                {
                    _dgvDuyurular.Columns["Tarih"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Duyurular yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadSampleData();
            }
        }

        private void LoadSampleData()
        {
            var table = new DataTable();
            table.Columns.Add("Başlık", typeof(string));
            table.Columns.Add("İçerik", typeof(string));
            table.Columns.Add("Gönderen", typeof(string));
            table.Columns.Add("Tarih", typeof(DateTime));
            table.Columns.Add("Hedef", typeof(string));
            _dgvDuyurular.DataSource = table;
            if (_dgvDuyurular.Columns.Count > 0)
                _dgvDuyurular.Columns["Tarih"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
        }

        private async void BtnDuyuruGonder_Click(object sender, EventArgs e)
        {
            using (var form = new DuyuruGonderForm())
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;
                if (_announcementService == null)
                {
                    MessageBox.Show("Duyuru servisi kullanılamıyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var dto = new AnnouncementDto
                {
                    Title = form.Baslik,
                    Content = form.Icerik,
                    TargetRole = MapTargetRole(form.Alici)
                };
                try
                {
                    await _announcementService.CreateAsync(dto, _authService?.GetCurrentUserId).ConfigureAwait(true);
                    await LoadAnnouncementsAsync().ConfigureAwait(true);
                    MessageBox.Show(string.Format("Duyuru gönderildi: {0}", form.Baslik), "Duyuru Gönder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Duyuru kaydedilirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private static string MapTargetRole(string alici)
        {
            if (string.IsNullOrEmpty(alici)) return null;
            if (alici.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0) return "admin";
            if (alici.IndexOf("Gruplar", StringComparison.OrdinalIgnoreCase) >= 0) return "groups";
            return null;
        }
    }
}
