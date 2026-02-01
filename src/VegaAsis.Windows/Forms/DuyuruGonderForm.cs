using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class DuyuruGonderForm : Form
    {
        private TextBox _txtBaslik;
        private TextBox _txtIcerik;
        private ComboBox _cmbAlici;
        private Button _btnGonder;
        private Button _btnIptal;

        public string Baslik { get; private set; }
        public string Icerik { get; private set; }
        public string Alici { get; private set; }

        public DuyuruGonderForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Duyuru Gönder";
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int y = 20;

            var lblBaslik = new Label { Text = "Başlık:", Location = new Point(15, y), AutoSize = true };
            Controls.Add(lblBaslik);
            _txtBaslik = new TextBox { Location = new Point(120, y - 2), Width = 340 };
            Controls.Add(_txtBaslik);
            y += 35;

            var lblIcerik = new Label { Text = "İçerik:", Location = new Point(15, y), AutoSize = true };
            Controls.Add(lblIcerik);
            _txtIcerik = new TextBox
            {
                Location = new Point(120, y - 2),
                Width = 340,
                Height = 180,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            Controls.Add(_txtIcerik);
            y += 195;

            var lblAlici = new Label { Text = "Alıcı:", Location = new Point(15, y), AutoSize = true };
            Controls.Add(lblAlici);
            _cmbAlici = new ComboBox
            {
                Location = new Point(120, y - 2),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbAlici.Items.AddRange(new object[] { "Tüm Kullanıcılar", "Sadece Admin", "Seçili Gruplar" });
            _cmbAlici.SelectedIndex = 0;
            Controls.Add(_cmbAlici);
            y += 40;

            _btnGonder = new Button
            {
                Text = "Gönder",
                Size = new Size(90, 30),
                Location = new Point(250, y)
            };
            _btnGonder.Click += BtnGonder_Click;
            Controls.Add(_btnGonder);

            _btnIptal = new Button
            {
                Text = "İptal",
                DialogResult = DialogResult.Cancel,
                Size = new Size(90, 30),
                Location = new Point(350, y)
            };
            Controls.Add(_btnIptal);

            AcceptButton = _btnGonder;
            CancelButton = _btnIptal;
        }

        private void BtnGonder_Click(object sender, EventArgs e)
        {
            Baslik = _txtBaslik?.Text?.Trim();
            Icerik = _txtIcerik?.Text?.Trim();
            Alici = _cmbAlici?.SelectedItem?.ToString() ?? "Tüm Kullanıcılar";

            if (string.IsNullOrEmpty(Baslik))
            {
                MessageBox.Show("Lütfen başlık girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
