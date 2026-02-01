using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.UserControls
{
    public class PaylasilanSirketlerControl : UserControl
    {
        private readonly ISharedCompanyService _sharedCompanyService;
        private readonly ICompanySettingsService _companySettingsService;
        private readonly IAuthService _authService;

        private Panel _topPanel;
        private ComboBox _cmbSirket;
        private TextBox _txtAcenteKodu;
        private Button _btnPaylas;
        private SplitContainer _splitMain;
        private bool _splitterInitialized;
        private DataGridView _dgvPaylasilanlar;
        private DataGridView _dgvAlinanlar;
        private Button _btnYenile;
        private Button _btnKaldir;
        private List<SharedCompanyDto> _sharedList;
        private List<ReceivedCompanyShareDto> _receivedList;

        public PaylasilanSirketlerControl(
            ISharedCompanyService sharedCompanyService,
            ICompanySettingsService companySettingsService,
            IAuthService authService)
        {
            _sharedCompanyService = sharedCompanyService ?? throw new ArgumentNullException(nameof(sharedCompanyService));
            _companySettingsService = companySettingsService ?? throw new ArgumentNullException(nameof(companySettingsService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            SuspendLayout();
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            MinimumSize = new Size(800, 400);

            BuildTopPanel();
            BuildSplitContent();
            BuildBottomPanel();

            ResumeLayout(true);
            Load += PaylasilanSirketlerControl_Load;
        }

        private async void PaylasilanSirketlerControl_Load(object sender, EventArgs e)
        {
            await LoadCompaniesAsync();
            await LoadDataAsync();
        }

        private async Task LoadCompaniesAsync()
        {
            try
            {
                var companies = await _companySettingsService.GetSelectedCompaniesAsync();
                _cmbSirket.Items.Clear();
                _cmbSirket.Items.Add("Seçin");
                if (companies != null)
                {
                    foreach (var c in companies)
                    {
                        _cmbSirket.Items.Add(c);
                    }
                }
                _cmbSirket.SelectedIndex = 0;
            }
            catch
            {
                _cmbSirket.Items.Clear();
                _cmbSirket.Items.Add("Seçin");
                _cmbSirket.SelectedIndex = 0;
            }
        }

        private async Task LoadDataAsync()
        {
            var userId = _authService.GetCurrentUserId;
            if (!userId.HasValue)
            {
                return;
            }

            try
            {
                _sharedList = new List<SharedCompanyDto>(await _sharedCompanyService.GetAllAsync(userId.Value));
                _receivedList = new List<ReceivedCompanyShareDto>(await _sharedCompanyService.GetReceivedSharesAsync(userId.Value));

                _dgvPaylasilanlar.Rows.Clear();
                foreach (var item in _sharedList)
                {
                    var row = _dgvPaylasilanlar.Rows[_dgvPaylasilanlar.Rows.Add()];
                    row.Cells["ColSirket"].Value = item.CompanyName;
                    row.Cells["ColAcente"].Value = item.SharedWithAgentCode;
                    row.Cells["ColTarih"].Value = item.SharedAt.ToString("dd.MM.yyyy", new CultureInfo("tr-TR"));
                    row.Cells["ColDurum"].Value = item.Status ?? "Aktif";
                    row.Tag = item.Id;
                }

                _dgvAlinanlar.Rows.Clear();
                foreach (var item in _receivedList)
                {
                    var row = _dgvAlinanlar.Rows[_dgvAlinanlar.Rows.Add()];
                    row.Cells["ColFromAcente"].Value = item.FromAgentCode;
                    row.Cells["ColFromSirket"].Value = item.CompanyName;
                    row.Cells["ColFromTarih"].Value = item.ReceivedAt.ToString("dd.MM.yyyy", new CultureInfo("tr-TR"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BuildTopPanel()
        {
            _topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(8)
            };

            var lblSirket = new Label { Text = "Şirket:", AutoSize = true, Location = new Point(8, 18) };
            _cmbSirket = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 140,
                Location = new Point(55, 15),
                Font = new Font("Segoe UI", 9F)
            };

            var lblAcente = new Label { Text = "Acente Kodu:", AutoSize = true, Location = new Point(210, 18) };
            _txtAcenteKodu = new TextBox
            {
                Width = 100,
                Location = new Point(295, 15),
                Font = new Font("Segoe UI", 9F),
                MaxLength = 50
            };

            _btnPaylas = new Button
            {
                Text = "Paylaş",
                Size = new Size(80, 28),
                Location = new Point(410, 12),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnPaylas.FlatAppearance.BorderSize = 0;
            _btnPaylas.Click += BtnPaylas_Click;

            _topPanel.Controls.AddRange(new Control[] { lblSirket, _cmbSirket, lblAcente, _txtAcenteKodu, _btnPaylas });
            Controls.Add(_topPanel);
        }

        private async void BtnPaylas_Click(object sender, EventArgs e)
        {
            var companyName = _cmbSirket.SelectedItem as string;
            if (string.IsNullOrEmpty(companyName) || companyName == "Seçin")
            {
                MessageBox.Show("Lütfen bir şirket seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var agentCode = _txtAcenteKodu.Text?.Trim();
            if (string.IsNullOrEmpty(agentCode))
            {
                MessageBox.Show("Lütfen acente kodu girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var userId = _authService.GetCurrentUserId;
            if (!userId.HasValue)
            {
                MessageBox.Show("Bu işlem için giriş yapmalısınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _btnPaylas.Enabled = false;
            try
            {
                var dto = new SharedCompanyDto
                {
                    OwnerUserId = userId.Value,
                    CompanyName = companyName,
                    SharedWithAgentCode = agentCode,
                    Status = "active"
                };
                await _sharedCompanyService.CreateAsync(dto);
                MessageBox.Show(companyName + " şirketi " + agentCode + " acente ile paylaşıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _txtAcenteKodu.Clear();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Paylaşım sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnPaylas.Enabled = true;
            }
        }

        private void BuildSplitContent()
        {
            _splitMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                Panel1MinSize = 0,
                Panel2MinSize = 0,
                SplitterDistance = 0
            };
            _splitMain.Resize += SplitMain_Resize;

            var p1 = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
            var lbl1 = new Label { Text = "Benim Paylaştıklarım", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Dock = DockStyle.Top, Height = 24 };
            _dgvPaylasilanlar = CreateDgv(true);
            p1.Controls.Add(_dgvPaylasilanlar);
            p1.Controls.Add(lbl1);
            _splitMain.Panel1.Controls.Add(p1);

            var p2 = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
            var lbl2 = new Label { Text = "Benimle Paylaşılanlar", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Dock = DockStyle.Top, Height = 24 };
            _dgvAlinanlar = CreateDgv(false);
            p2.Controls.Add(_dgvAlinanlar);
            p2.Controls.Add(lbl2);
            _splitMain.Panel2.Controls.Add(p2);

            Controls.Add(_splitMain);
        }

        private void SplitMain_Resize(object sender, EventArgs e)
        {
            if (_splitterInitialized) return;
            const int panel1Min = 200;
            const int panel2Min = 200;
            int w = _splitMain.Width;
            if (w < panel1Min + panel2Min) return;
            _splitterInitialized = true;
            _splitMain.Resize -= SplitMain_Resize;
            _splitMain.Panel1MinSize = panel1Min;
            _splitMain.Panel2MinSize = panel2Min;
            _splitMain.SplitterDistance = Math.Max(panel1Min, Math.Min(400, w - panel2Min));
        }

        private DataGridView CreateDgv(bool isShared)
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 28
            };
            dgv.RowTemplate.Height = 26;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            if (isShared)
            {
                dgv.Columns.Add("ColAcente", "Acente");
                dgv.Columns.Add("ColSirket", "Şirket");
                dgv.Columns.Add("ColTarih", "Tarih");
                dgv.Columns.Add("ColDurum", "Durum");
            }
            else
            {
                dgv.Columns.Add("ColFromAcente", "Gönderen");
                dgv.Columns.Add("ColFromSirket", "Şirket");
                dgv.Columns.Add("ColFromTarih", "Tarih");
            }

            return dgv;
        }

        private void BuildBottomPanel()
        {
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(8)
            };

            _btnYenile = new Button
            {
                Text = "Yenile",
                Size = new Size(90, 32),
                Location = new Point(8, 9),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnYenile.FlatAppearance.BorderSize = 0;
            _btnYenile.Click += async (s, ev) => await LoadDataAsync();

            _btnKaldir = new Button
            {
                Text = "Paylaşımı Kaldır",
                Size = new Size(120, 32),
                Location = new Point(106, 9),
                BackColor = Color.FromArgb(198, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnKaldir.FlatAppearance.BorderSize = 0;
            _btnKaldir.Click += BtnKaldir_Click;

            bottomPanel.Controls.Add(_btnYenile);
            bottomPanel.Controls.Add(_btnKaldir);
            Controls.Add(bottomPanel);
        }

        private async void BtnKaldir_Click(object sender, EventArgs e)
        {
            if (_dgvPaylasilanlar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen kaldırılacak paylaşımı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _dgvPaylasilanlar.SelectedRows[0];
            var id = row.Tag as Guid?;
            if (!id.HasValue)
            {
                return;
            }

            var result = MessageBox.Show("Bu paylaşımı kaldırmak istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var ok = await _sharedCompanyService.DeleteAsync(id.Value);
                if (ok)
                {
                    MessageBox.Show("Paylaşım kaldırıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaldırma sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
