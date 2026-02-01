using System;
using System.Drawing;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.Forms
{
    public class WebUserForm : Form
    {
        private readonly WebUserDto _existing;
        private readonly IAuthService _authService;

        private TextBox _txtWebUsername;
        private TextBox _txtUsername;
        private TextBox _txtFullName;
        private TextBox _txtPassword;
        private TextBox _txtGsm;
        private TextBox _txtEmail;
        private CheckBox _chkIsLicensed;
        private CheckBox _chkLicenseOfferOnly;
        private CheckBox _chkLicenseOnlinePolicy;
        private CheckBox _chkLicenseCompanyScreen;
        private CheckBox _chkUnlicensedAgentOnly;
        private TextBox _txtResponsibleStaff;

        public WebUserDto Dto { get; private set; }

        public WebUserForm(WebUserDto existing, IAuthService authService)
        {
            _existing = existing;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            Text = existing == null ? "Yeni Web Kullanıcı" : "Web Kullanıcı Düzenle";
            Size = new Size(420, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            BuildForm();
            if (existing != null)
            {
                LoadExisting();
            }
        }

        private void BuildForm()
        {
            int y = 12;
            var pad = 8;

            AddRow("Web Kullanıcı Adı:", out _txtWebUsername, ref y);
            AddRow("Kullanıcı Adı:", out _txtUsername, ref y);
            AddRow("Ad Soyad:", out _txtFullName, ref y);
            AddRow("Şifre:", out _txtPassword, ref y);
            AddRow("GSM:", out _txtGsm, ref y);
            AddRow("E-posta:", out _txtEmail, ref y);
            AddRow("Sorumlu Personel:", out _txtResponsibleStaff, ref y);

            y += 8;
            var grpLisans = new GroupBox { Text = "Lisans Ayarları", Location = new Point(pad, y), Size = new Size(380, 120), Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            int ly = 24;
            _chkIsLicensed = AddCheck(grpLisans, "Lisanslı", ref ly);
            _chkLicenseOfferOnly = AddCheck(grpLisans, "Sadece Teklif", ref ly);
            _chkLicenseOnlinePolicy = AddCheck(grpLisans, "Online Poliçe", ref ly);
            _chkLicenseCompanyScreen = AddCheck(grpLisans, "Şirket Ekranı", ref ly);
            _chkUnlicensedAgentOnly = AddCheck(grpLisans, "Lisanssız Acente Only", ref ly);
            Controls.Add(grpLisans);
            y += 128;

            var btnOk = new Button
            {
                Text = "Kaydet",
                Size = new Size(100, 32),
                Location = new Point(pad, y),
                DialogResult = DialogResult.OK
            };
            btnOk.Click += BtnOk_Click;

            var btnCancel = new Button
            {
                Text = "İptal",
                Size = new Size(100, 32),
                Location = new Point(110, y),
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void AddRow(string label, out TextBox txt, ref int y)
        {
            var lbl = new Label { Text = label, Location = new Point(8, y), Width = 120, Font = new Font("Segoe UI", 9F) };
            txt = new TextBox { Location = new Point(135, y - 2), Width = 240, Font = new Font("Segoe UI", 9F) };
            if (label.Contains("Şifre")) txt.UseSystemPasswordChar = true;
            Controls.Add(lbl);
            Controls.Add(txt);
            y += 30;
        }

        private CheckBox AddCheck(Control parent, string text, ref int y)
        {
            var chk = new CheckBox { Text = text, Location = new Point(12, y), AutoSize = true, Font = new Font("Segoe UI", 9F) };
            parent.Controls.Add(chk);
            y += 24;
            return chk;
        }

        private void LoadExisting()
        {
            _txtWebUsername.Text = _existing.WebUsername;
            _txtUsername.Text = _existing.Username;
            _txtFullName.Text = _existing.FullName;
            _txtPassword.Text = _existing.Password;
            _txtGsm.Text = _existing.Gsm;
            _txtEmail.Text = _existing.Email;
            _txtResponsibleStaff.Text = _existing.ResponsibleStaff;
            _chkIsLicensed.Checked = _existing.IsLicensed;
            _chkLicenseOfferOnly.Checked = _existing.LicenseOfferOnly;
            _chkLicenseOnlinePolicy.Checked = _existing.LicenseOnlinePolicy;
            _chkLicenseCompanyScreen.Checked = _existing.LicenseCompanyScreen;
            _chkUnlicensedAgentOnly.Checked = _existing.UnlicensedAgentOnly;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtWebUsername.Text))
            {
                MessageBox.Show("Web kullanıcı adı gerekli.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }
            if (string.IsNullOrWhiteSpace(_txtUsername.Text))
            {
                MessageBox.Show("Kullanıcı adı gerekli.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Dto = new WebUserDto
            {
                Id = _existing?.Id ?? Guid.Empty,
                UserId = _existing?.UserId ?? Guid.Empty,
                WebUsername = _txtWebUsername.Text.Trim(),
                Username = _txtUsername.Text.Trim(),
                FullName = string.IsNullOrWhiteSpace(_txtFullName.Text) ? null : _txtFullName.Text.Trim(),
                Password = string.IsNullOrWhiteSpace(_txtPassword.Text) ? null : _txtPassword.Text,
                Gsm = string.IsNullOrWhiteSpace(_txtGsm.Text) ? null : _txtGsm.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(_txtEmail.Text) ? null : _txtEmail.Text.Trim(),
                ResponsibleStaff = string.IsNullOrWhiteSpace(_txtResponsibleStaff.Text) ? null : _txtResponsibleStaff.Text.Trim(),
                IsLicensed = _chkIsLicensed.Checked,
                LicenseOfferOnly = _chkLicenseOfferOnly.Checked,
                LicenseOnlinePolicy = _chkLicenseOnlinePolicy.Checked,
                LicenseCompanyScreen = _chkLicenseCompanyScreen.Checked,
                UnlicensedAgentOnly = _chkUnlicensedAgentOnly.Checked,
                CreatedAt = _existing?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
