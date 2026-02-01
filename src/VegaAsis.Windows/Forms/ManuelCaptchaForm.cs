using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    /// <summary>
    /// Manuel captcha çözümü: görsel + metin kutusu + OK. Robot captcha gördüğünde bu form açılır; kullanıcı metni girip OK der.
    /// </summary>
    public class ManuelCaptchaForm : Form
    {
        private PictureBox _pictureBox;
        private TextBox _txtCevap;
        private Button _btnOk;
        private Button _btnIptal;

        public string SolutionText { get; private set; }

        public ManuelCaptchaForm()
        {
            Text = "Captcha – Manuel Giriş";
            Size = new Size(400, 380);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            BuildUi();
        }

        public void SetImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                _pictureBox.Image = null;
                _pictureBox.Refresh();
                return;
            }
            try
            {
                using (var ms = new MemoryStream(imageBytes))
                    _pictureBox.Image = Image.FromStream(ms);
            }
            catch
            {
                _pictureBox.Image = null;
            }
        }

        public void SetImageFromBase64(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) { SetImage(null); return; }
            try
            {
                var bytes = Convert.FromBase64String(base64.Trim());
                SetImage(bytes);
            }
            catch
            {
                SetImage(null);
            }
        }

        private void BuildUi()
        {
            _pictureBox = new PictureBox
            {
                Location = new Point(12, 12),
                Size = new Size(360, 180),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            var lblCevap = new Label { Text = "Captcha metnini girin:", Location = new Point(12, 200), AutoSize = true };
            _txtCevap = new TextBox { Location = new Point(12, 218), Size = new Size(360, 24), MaxLength = 32 };
            _btnOk = new Button { Text = "OK", Size = new Size(90, 28), Location = new Point(192, 252), DialogResult = DialogResult.OK };
            _btnIptal = new Button { Text = "İptal", Size = new Size(90, 28), Location = new Point(288, 252), DialogResult = DialogResult.Cancel };
            AcceptButton = _btnOk;
            CancelButton = _btnIptal;
            _btnOk.Click += (s, e) =>
            {
                SolutionText = _txtCevap?.Text?.Trim() ?? "";
                DialogResult = DialogResult.OK;
                Close();
            };
            _btnIptal.Click += (s, e) =>
            {
                SolutionText = null;
                DialogResult = DialogResult.Cancel;
                Close();
            };
            Controls.AddRange(new Control[] { _pictureBox, lblCevap, _txtCevap, _btnOk, _btnIptal });
        }
    }
}
