using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public partial class WebcamQRForm : Form
    {
        private Panel _pnlUst;
        private Label _lblBilgi;
        private PictureBox _picWebcam;
        private GroupBox _grpOkunanVeri;
        private TextBox _txtOkunanVeri;
        private Panel _pnlAlt;
        private Button _btnBaslat;
        private Button _btnDurdur;
        private Button _btnKopyala;
        private Button _btnKapat;

        public string OkunanVeri { get; private set; }

        public WebcamQRForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "QR Kod Okuyucu";
            Size = new Size(500, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Üst Panel
            _pnlUst = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top
            };

            _lblBilgi = new Label
            {
                Text = "Kamerayı QR koda tutun",
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Left = 20,
                Top = 10
            };

            _pnlUst.Controls.Add(_lblBilgi);
            Controls.Add(_pnlUst);

            // PictureBox (Webcam görüntüsü)
            _picWebcam = new PictureBox
            {
                Size = new Size(300, 300),
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            _picWebcam.Left = (ClientSize.Width - _picWebcam.Width) / 2;
            _picWebcam.Top = _pnlUst.Bottom + 10;
            _picWebcam.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(_picWebcam);

            // GroupBox - Okunan Veri
            _grpOkunanVeri = new GroupBox
            {
                Text = "Okunan Veri",
                Height = 80,
                Left = 20,
                Top = _picWebcam.Bottom + 10,
                Width = ClientSize.Width - 40,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _txtOkunanVeri = new TextBox
            {
                ReadOnly = true,
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical
            };

            _grpOkunanVeri.Controls.Add(_txtOkunanVeri);
            Controls.Add(_grpOkunanVeri);

            // Alt Panel
            _pnlAlt = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom
            };

            _btnBaslat = new Button
            {
                Text = "Kamerayı Başlat",
                Width = 120,
                Height = 32,
                Left = 20,
                Top = 9
            };
            _btnBaslat.Click += BtnBaslat_Click;

            _btnDurdur = new Button
            {
                Text = "Durdur",
                Width = 80,
                Height = 32,
                Left = _btnBaslat.Right + 10,
                Top = 9,
                Enabled = false
            };
            _btnDurdur.Click += BtnDurdur_Click;

            _btnKopyala = new Button
            {
                Text = "Kopyala",
                Width = 80,
                Height = 32,
                Left = _btnDurdur.Right + 10,
                Top = 9
            };
            _btnKopyala.Click += BtnKopyala_Click;

            _btnKapat = new Button
            {
                Text = "Kapat",
                Width = 80,
                Height = 32,
                Left = ClientSize.Width - 100,
                Top = 9,
                DialogResult = DialogResult.Cancel
            };
            _btnKapat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnKapat.Click += BtnKapat_Click;

            _pnlAlt.Controls.Add(_btnBaslat);
            _pnlAlt.Controls.Add(_btnDurdur);
            _pnlAlt.Controls.Add(_btnKopyala);
            _pnlAlt.Controls.Add(_btnKapat);
            Controls.Add(_pnlAlt);

            // Form resize event'i için PictureBox ve GroupBox'ı güncelle
            Resize += WebcamQRForm_Resize;
        }

        private void WebcamQRForm_Resize(object sender, EventArgs e)
        {
            if (_picWebcam != null)
            {
                _picWebcam.Left = (ClientSize.Width - _picWebcam.Width) / 2;
            }

            if (_grpOkunanVeri != null)
            {
                _grpOkunanVeri.Width = ClientSize.Width - 40;
            }

            if (_btnKapat != null)
            {
                _btnKapat.Left = ClientSize.Width - 100;
            }
        }

        private void BtnBaslat_Click(object sender, EventArgs e)
        {
            // Placeholder: Gerçek webcam entegrasyonu için AForge.NET kullanılabilir
            MessageBox.Show(
                "Kamera başlatma özelliği henüz entegre edilmedi.\n\nGerçek webcam entegrasyonu için AForge.NET kütüphanesi kullanılabilir.",
                "Bilgi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            _btnBaslat.Enabled = false;
            _btnDurdur.Enabled = true;
        }

        private void BtnDurdur_Click(object sender, EventArgs e)
        {
            // Placeholder: Kamera durdurma işlemi
            MessageBox.Show("Kamera durduruldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _btnBaslat.Enabled = true;
            _btnDurdur.Enabled = false;
        }

        private void BtnKopyala_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_txtOkunanVeri.Text))
            {
                Clipboard.SetText(_txtOkunanVeri.Text);
                MessageBox.Show("Veri panoya kopyalandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Kopyalanacak veri yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnKapat_Click(object sender, EventArgs e)
        {
            OkunanVeri = _txtOkunanVeri.Text;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
