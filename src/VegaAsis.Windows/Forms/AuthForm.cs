using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;

namespace VegaAsis.Windows.Forms
{
    public class AuthForm : Form
    {
        private readonly IAuthService _authService;
        private TextBox _txtEmail;
        private Button _btnGiris;
        private Label _lblEmail;

        public AuthForm(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            Text = "VegaAsis - Giriş";
            Size = new Size(400, 180);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            _lblEmail = new Label
            {
                Text = "E-posta:",
                Left = 20,
                Top = 24,
                Width = 80,
                Height = 20
            };

            _txtEmail = new TextBox
            {
                Left = 100,
                Top = 22,
                Width = 260,
                Height = 22,
                Text = "ozgurcelik2018@ogr.iu.edu.tr"
            };

            _btnGiris = new Button
            {
                Text = "Giriş",
                Left = 100,
                Top = 56,
                Width = 120,
                Height = 32
            };
            _btnGiris.Click += BtnGiris_Click;

            Controls.Add(_lblEmail);
            Controls.Add(_txtEmail);
            Controls.Add(_btnGiris);
        }

        private async void BtnGiris_Click(object sender, EventArgs e)
        {
            var email = _txtEmail.Text?.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("E-posta adresi girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _btnGiris.Enabled = false;
                var ok = await _authService.LoginAsync(email).ConfigureAwait(true);
                if (ok)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Bu e-posta ile kayıtlı kullanıcı bulunamadı.", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                _btnGiris.Enabled = true;
            }
        }
    }
}
