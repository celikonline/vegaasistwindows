using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Windows.Robot;

namespace VegaAsis.Windows.Forms
{
    /// <summary>
    /// Şirket portal giriş bilgilerini yönetmek için form.
    /// Kullanıcı her sigorta şirketi için kullanıcı adı ve şifre tanımlayabilir.
    /// </summary>
    public class SirketKimlikBilgileriForm : BaseForm
    {
        private Panel _topPanel;
        private Label _lblBaslik;
        private Label _lblAciklama;
        private DataGridView _dgvCredentials;
        private Panel _bottomPanel;
        private Button _btnSil;
        private Button _btnKapat;
        private Panel _detailPanel;
        private Label _lblSirket;
        private ComboBox _cmbSirket;
        private Label _lblKullaniciAdi;
        private TextBox _txtKullaniciAdi;
        private Label _lblSifre;
        private TextBox _txtSifre;
        private CheckBox _chkAktif;
        private Button _btnEkle;
        private Button _btnGuncelle;

        private ICompanyCredentialService _credentialService;
        private List<CompanyCredentialDto> _credentials;
        private CompanyCredentialDto _selectedCredential;

        public SirketKimlikBilgileriForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                _credentialService = Resolve<ICompanyCredentialService>();
                LoadCredentialsAsync();
                LoadCompanyList();
            }
            catch (Exception ex)
            {
                ShowError("Servis yüklenemedi: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            Text = "Şirket Giriş Bilgileri";
            Size = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(800, 500);

            // Üst Panel
            _topPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                Padding = new Padding(15, 10, 15, 5),
                BackColor = Color.FromArgb(45, 45, 48)
            };

            _lblBaslik = new Label
            {
                Text = "Şirket Portal Giriş Bilgileri",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(15, 12)
            };

            _lblAciklama = new Label
            {
                Text = "Sigorta şirketlerinin portal kullanıcı adı ve şifrelerini buradan yönetebilirsiniz.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(15, 42)
            };

            _topPanel.Controls.Add(_lblBaslik);
            _topPanel.Controls.Add(_lblAciklama);

            // Sol Panel - Grid
            var leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            _dgvCredentials = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                MultiSelect = false,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false
            };

            _dgvCredentials.Columns.Add("SirketKodu", "Kod");
            _dgvCredentials.Columns.Add("SirketAdi", "Şirket Adı");
            _dgvCredentials.Columns.Add("KullaniciAdi", "Kullanıcı Adı");
            _dgvCredentials.Columns.Add("Durum", "Durum");

            _dgvCredentials.Columns["SirketKodu"].Width = 80;
            _dgvCredentials.Columns["SirketAdi"].Width = 180;
            _dgvCredentials.Columns["KullaniciAdi"].Width = 150;
            _dgvCredentials.Columns["Durum"].Width = 80;

            _dgvCredentials.SelectionChanged += DgvCredentials_SelectionChanged;

            leftPanel.Controls.Add(_dgvCredentials);

            // Sağ Panel - Detay
            _detailPanel = new Panel
            {
                Width = 280,
                Dock = DockStyle.Right,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            var y = 10;

            _lblSirket = new Label
            {
                Text = "Şirket:",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };
            y += 22;

            _cmbSirket = new ComboBox
            {
                Location = new Point(10, y),
                Width = 245,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            y += 35;

            _lblKullaniciAdi = new Label
            {
                Text = "Kullanıcı Adı:",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };
            y += 22;

            _txtKullaniciAdi = new TextBox
            {
                Location = new Point(10, y),
                Width = 245,
                Font = new Font("Segoe UI", 9F)
            };
            y += 35;

            _lblSifre = new Label
            {
                Text = "Şifre:",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };
            y += 22;

            _txtSifre = new TextBox
            {
                Location = new Point(10, y),
                Width = 245,
                Font = new Font("Segoe UI", 9F),
                PasswordChar = '●'
            };
            y += 35;

            _chkAktif = new CheckBox
            {
                Text = "Aktif",
                Location = new Point(10, y),
                Checked = true,
                Font = new Font("Segoe UI", 9F)
            };
            y += 35;

            _btnEkle = new Button
            {
                Text = "Yeni Ekle",
                Location = new Point(10, y),
                Width = 118,
                Height = 32,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnEkle.FlatAppearance.BorderSize = 0;
            _btnEkle.Click += BtnEkle_Click;

            _btnGuncelle = new Button
            {
                Text = "Güncelle",
                Location = new Point(137, y),
                Width = 118,
                Height = 32,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnGuncelle.FlatAppearance.BorderSize = 0;
            _btnGuncelle.Click += BtnGuncelle_Click;

            _detailPanel.Controls.Add(_lblSirket);
            _detailPanel.Controls.Add(_cmbSirket);
            _detailPanel.Controls.Add(_lblKullaniciAdi);
            _detailPanel.Controls.Add(_txtKullaniciAdi);
            _detailPanel.Controls.Add(_lblSifre);
            _detailPanel.Controls.Add(_txtSifre);
            _detailPanel.Controls.Add(_chkAktif);
            _detailPanel.Controls.Add(_btnEkle);
            _detailPanel.Controls.Add(_btnGuncelle);

            // Alt Panel
            _bottomPanel = new Panel
            {
                Height = 55,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10),
                BackColor = SystemColors.Control
            };

            _btnSil = new Button
            {
                Text = "Seçili Kaydı Sil",
                Width = 120,
                Height = 32,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnSil.FlatAppearance.BorderSize = 0;
            _btnSil.Click += BtnSil_Click;

            _btnKapat = new Button
            {
                Text = "Kapat",
                DialogResult = DialogResult.Cancel,
                Width = 90,
                Height = 32
            };

            _bottomPanel.Controls.Add(_btnSil);
            _bottomPanel.Controls.Add(_btnKapat);

            void PositionBottomButtons(object s, EventArgs e)
            {
                _btnKapat.Location = new Point(_bottomPanel.Width - _btnKapat.Width - 15, (_bottomPanel.Height - _btnKapat.Height) / 2);
                _btnSil.Location = new Point(15, (_bottomPanel.Height - _btnSil.Height) / 2);
            }

            _bottomPanel.Layout += PositionBottomButtons;
            PositionBottomButtons(null, EventArgs.Empty);

            // Ana Panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(leftPanel);
            mainPanel.Controls.Add(_detailPanel);

            Controls.Add(mainPanel);
            Controls.Add(_topPanel);
            Controls.Add(_bottomPanel);

            Controls.SetChildIndex(_topPanel, 0);
            Controls.SetChildIndex(_bottomPanel, 1);
            Controls.SetChildIndex(mainPanel, 2);

            CancelButton = _btnKapat;
        }

        private void LoadCompanyList()
        {
            _cmbSirket.Items.Clear();
            var companies = CompanyRobotRegistry.GetAllCompanyIds()
                .Select(id => new CompanyItem
                {
                    Id = id,
                    Name = CompanyRobotRegistry.GetRobot(id)?.CompanyName ?? id
                })
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var company in companies)
            {
                _cmbSirket.Items.Add(company);
            }

            if (_cmbSirket.Items.Count > 0)
                _cmbSirket.SelectedIndex = 0;
        }

        private async void LoadCredentialsAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _credentials = (await _credentialService.GetAllCredentialsAsync().ConfigureAwait(false))?.ToList()
                    ?? new List<CompanyCredentialDto>();

                if (InvokeRequired)
                    Invoke(new Action(RefreshGrid));
                else
                    RefreshGrid();
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                    Invoke(new Action(() => ShowError("Veriler yüklenemedi: " + ex.Message)));
                else
                    ShowError("Veriler yüklenemedi: " + ex.Message);
            }
            finally
            {
                if (InvokeRequired)
                    Invoke(new Action(() => Cursor = Cursors.Default));
                else
                    Cursor = Cursors.Default;
            }
        }

        private void RefreshGrid()
        {
            _dgvCredentials.Rows.Clear();
            if (_credentials == null) return;

            foreach (var cred in _credentials.OrderBy(c => c.CompanyId))
            {
                var robot = CompanyRobotRegistry.GetRobot(cred.CompanyId);
                var companyName = robot?.CompanyName ?? cred.CompanyId;
                var status = cred.IsActive ? "Aktif" : "Pasif";

                var rowIndex = _dgvCredentials.Rows.Add(cred.CompanyId, companyName, cred.Username, status);
                _dgvCredentials.Rows[rowIndex].Tag = cred;
            }
        }

        private void DgvCredentials_SelectionChanged(object sender, EventArgs e)
        {
            if (_dgvCredentials.SelectedRows.Count == 0)
            {
                _selectedCredential = null;
                ClearDetailForm();
                return;
            }

            _selectedCredential = _dgvCredentials.SelectedRows[0].Tag as CompanyCredentialDto;
            if (_selectedCredential != null)
            {
                SelectCompanyInCombo(_selectedCredential.CompanyId);
                _txtKullaniciAdi.Text = _selectedCredential.Username;
                _txtSifre.Text = _selectedCredential.Password;
                _chkAktif.Checked = _selectedCredential.IsActive;
            }
        }

        private void SelectCompanyInCombo(string companyId)
        {
            for (int i = 0; i < _cmbSirket.Items.Count; i++)
            {
                var item = _cmbSirket.Items[i] as CompanyItem;
                if (item != null && item.Id == companyId)
                {
                    _cmbSirket.SelectedIndex = i;
                    return;
                }
            }
        }

        private void ClearDetailForm()
        {
            if (_cmbSirket.Items.Count > 0)
                _cmbSirket.SelectedIndex = 0;
            _txtKullaniciAdi.Clear();
            _txtSifre.Clear();
            _chkAktif.Checked = true;
        }

        private async void BtnEkle_Click(object sender, EventArgs e)
        {
            var company = _cmbSirket.SelectedItem as CompanyItem;
            if (company == null)
            {
                ShowError("Lütfen bir şirket seçin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_txtKullaniciAdi.Text))
            {
                ShowError("Kullanıcı adı boş olamaz.");
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                await _credentialService.SaveCredentialAsync(
                    company.Id,
                    _txtKullaniciAdi.Text.Trim(),
                    _txtSifre.Text,
                    null
                ).ConfigureAwait(false);

                if (!_chkAktif.Checked)
                {
                    await _credentialService.SetActiveAsync(company.Id, false, null).ConfigureAwait(false);
                }

                LoadCredentialsAsync();
                ShowInfo("Kayıt başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                ShowError("Kayıt eklenemedi: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void BtnGuncelle_Click(object sender, EventArgs e)
        {
            var company = _cmbSirket.SelectedItem as CompanyItem;
            if (company == null)
            {
                ShowError("Lütfen bir şirket seçin.");
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                await _credentialService.SaveCredentialAsync(
                    company.Id,
                    _txtKullaniciAdi.Text.Trim(),
                    _txtSifre.Text,
                    null
                ).ConfigureAwait(false);

                await _credentialService.SetActiveAsync(company.Id, _chkAktif.Checked, null).ConfigureAwait(false);

                LoadCredentialsAsync();
                ShowInfo("Kayıt güncellendi.");
            }
            catch (Exception ex)
            {
                ShowError("Kayıt güncellenemedi: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void BtnSil_Click(object sender, EventArgs e)
        {
            if (_selectedCredential == null)
            {
                ShowError("Lütfen silmek için bir kayıt seçin.");
                return;
            }

            if (!Confirm($"{_selectedCredential.CompanyId} şirketi için giriş bilgisi silinecek. Emin misiniz?"))
                return;

            try
            {
                Cursor = Cursors.WaitCursor;
                await _credentialService.DeleteCredentialAsync(_selectedCredential.CompanyId, null).ConfigureAwait(false);
                LoadCredentialsAsync();
                ClearDetailForm();
                ShowInfo("Kayıt silindi.");
            }
            catch (Exception ex)
            {
                ShowError("Kayıt silinemedi: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private class CompanyItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => $"{Name} ({Id})";
        }
    }
}
