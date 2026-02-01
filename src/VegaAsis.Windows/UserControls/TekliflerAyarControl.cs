using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.UserControls
{
    public class TekliflerAyarControl : UserControl
    {
        private GroupBox _grpVarsayilanAyarlar;
        private CheckBox _chkOtomatikFiyatGuncelleme;
        private CheckBox _chkTeklifBildirimiGonder;
        private CheckBox _chkCalisildiOtomatikIsaretle;

        private GroupBox _grpLimitler;
        private NumericUpDown _nudGunlukTeklifLimiti;
        private NumericUpDown _nudAylikTeklifLimiti;

        private GroupBox _grpVarsayilanSirketler;
        private CheckedListBox _clbSirketler;

        private Button _btnKaydet;

        public TekliflerAyarControl()
        {
            SuspendLayout();
            Dock = DockStyle.Fill;
            BackColor = SystemColors.Control;
            Padding = new Padding(8);
            MinimumSize = new Size(500, 400);

            InitializeComponents();

            ResumeLayout(true);
        }

        private void InitializeComponents()
        {
            int y = 8;

            // Varsayılan Ayarlar GroupBox
            _grpVarsayilanAyarlar = new GroupBox
            {
                Text = "Varsayılan Ayarlar",
                Location = new Point(8, y),
                Size = new Size(480, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            int checkY = 20;
            _chkOtomatikFiyatGuncelleme = new CheckBox
            {
                Text = "Otomatik Fiyat Güncelleme",
                Location = new Point(12, checkY),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };
            checkY += 24;

            _chkTeklifBildirimiGonder = new CheckBox
            {
                Text = "Teklif Bildirimi Gönder",
                Location = new Point(12, checkY),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };
            checkY += 24;

            _chkCalisildiOtomatikIsaretle = new CheckBox
            {
                Text = "Çalışıldı Otomatik İşaretle",
                Location = new Point(12, checkY),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };

            _grpVarsayilanAyarlar.Controls.Add(_chkOtomatikFiyatGuncelleme);
            _grpVarsayilanAyarlar.Controls.Add(_chkTeklifBildirimiGonder);
            _grpVarsayilanAyarlar.Controls.Add(_chkCalisildiOtomatikIsaretle);
            Controls.Add(_grpVarsayilanAyarlar);
            y += 108;

            // Limitler GroupBox
            _grpLimitler = new GroupBox
            {
                Text = "Limitler",
                Location = new Point(8, y),
                Size = new Size(480, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            var lblGunluk = new Label
            {
                Text = "Günlük Teklif Limiti:",
                Location = new Point(12, 24),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };

            _nudGunlukTeklifLimiti = new NumericUpDown
            {
                Location = new Point(150, 22),
                Size = new Size(120, 20),
                Minimum = 0,
                Maximum = 10000,
                Value = 0,
                Font = new Font("Segoe UI", 9F)
            };

            var lblAylik = new Label
            {
                Text = "Aylık Teklif Limiti:",
                Location = new Point(12, 52),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };

            _nudAylikTeklifLimiti = new NumericUpDown
            {
                Location = new Point(150, 50),
                Size = new Size(120, 20),
                Minimum = 0,
                Maximum = 100000,
                Value = 0,
                Font = new Font("Segoe UI", 9F)
            };

            _grpLimitler.Controls.Add(lblGunluk);
            _grpLimitler.Controls.Add(_nudGunlukTeklifLimiti);
            _grpLimitler.Controls.Add(lblAylik);
            _grpLimitler.Controls.Add(_nudAylikTeklifLimiti);
            Controls.Add(_grpLimitler);
            y += 88;

            // Varsayılan Şirketler GroupBox
            _grpVarsayilanSirketler = new GroupBox
            {
                Text = "Varsayılan Şirketler",
                Location = new Point(8, y),
                Size = new Size(480, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            _clbSirketler = new CheckedListBox
            {
                Location = new Point(12, 20),
                Size = new Size(456, 170),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F),
                CheckOnClick = true
            };

            // Örnek şirket listesi
            string[] sirketler = { "ALLIANZ", "AXA", "ANADOLU", "GROUPAMA", "HDI", "MAPFRE", "NEOS", "QBE", "RAY", "ZURICH" };
            foreach (var sirket in sirketler)
            {
                _clbSirketler.Items.Add(sirket, false);
            }

            _grpVarsayilanSirketler.Controls.Add(_clbSirketler);
            Controls.Add(_grpVarsayilanSirketler);

            // Kaydet Button (sağ alt)
            _btnKaydet = new Button
            {
                Text = "Kaydet",
                Size = new Size(100, 32),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnKaydet.FlatAppearance.BorderSize = 0;
            _btnKaydet.Location = new Point(388, Height - 40);
            _btnKaydet.Click += BtnKaydet_Click;
            Controls.Add(_btnKaydet);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_btnKaydet != null)
            {
                _btnKaydet.Location = new Point(Width - 108, Height - 40);
            }
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            // TODO: Ayarları kaydetme işlemi
            MessageBox.Show("Ayarlar kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
