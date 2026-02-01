using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class CanliDestekForm : Form
    {
        private Panel _topPanel;
        private Label _lblBaslik;
        private RichTextBox _rtbSohbet;
        private Panel _bottomPanel;
        private TextBox _txtMesaj;
        private Button _btnGonder;

        public CanliDestekForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Canlı Destek";
            Size = new Size(500, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;

            // Üst Panel
            _topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };

            _lblBaslik = new Label
            {
                Text = "VegaAsis Destek Hattı",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 8)
            };
            _topPanel.Controls.Add(_lblBaslik);

            // Sohbet RichTextBox
            _rtbSohbet = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.None
            };
            _rtbSohbet.AppendText("Merhaba! Size nasıl yardımcı olabilirim?" + Environment.NewLine);

            // Alt Panel
            _bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };

            _txtMesaj = new TextBox
            {
                Multiline = true,
                Height = 40,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(10, 10)
            };
            _txtMesaj.KeyDown += TxtMesaj_KeyDown;

            _btnGonder = new Button
            {
                Text = "Gönder",
                Width = 80,
                Height = 40,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Location = new Point(410, 10),
                Font = new Font("Segoe UI", 9F)
            };
            _btnGonder.Click += BtnGonder_Click;

            _bottomPanel.Controls.Add(_txtMesaj);
            _bottomPanel.Controls.Add(_btnGonder);

            // Ana Panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(_rtbSohbet);
            mainPanel.Controls.Add(_topPanel);
            mainPanel.Controls.Add(_bottomPanel);

            Controls.Add(mainPanel);

            // Resize event'i için alt panel'i güncelle
            Resize += CanliDestekForm_Resize;
        }

        private void CanliDestekForm_Resize(object sender, EventArgs e)
        {
            if (_txtMesaj != null && _btnGonder != null && _bottomPanel != null)
            {
                _txtMesaj.Width = _bottomPanel.Width - _btnGonder.Width - 30;
                _btnGonder.Left = _bottomPanel.Width - _btnGonder.Width - 10;
            }
        }

        private void BtnGonder_Click(object sender, EventArgs e)
        {
            MesajGonder();
        }

        private void TxtMesaj_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                MesajGonder();
            }
        }

        private void MesajGonder()
        {
            var mesaj = _txtMesaj.Text.Trim();
            if (string.IsNullOrEmpty(mesaj))
            {
                return;
            }

            // Kullanıcı mesajını ekle
            _rtbSohbet.SelectionColor = Color.Blue;
            _rtbSohbet.AppendText("Sen: " + mesaj + Environment.NewLine);

            // TextBox'ı temizle
            _txtMesaj.Clear();
            _txtMesaj.Focus();

            // Otomatik cevap ekle
            _rtbSohbet.SelectionColor = Color.Black;
            _rtbSohbet.AppendText("Destek ekibimiz en kısa sürede size dönüş yapacaktır." + Environment.NewLine);

            // Scroll'u en alta al
            _rtbSohbet.SelectionStart = _rtbSohbet.Text.Length;
            _rtbSohbet.ScrollToCaret();
        }
    }
}
