using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class TeklifNoGitForm : Form
    {
        private TextBox _txtTeklifNo;
        private Button _btnGit;
        private Button _btnIptal;

        public string TeklifNo { get; private set; }

        public TeklifNoGitForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Teklif No ile Git";
            Size = new Size(360, 140);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lbl = new Label
            {
                Text = "Teklif No:",
                Location = new Point(12, 18),
                AutoSize = true
            };
            Controls.Add(lbl);

            _txtTeklifNo = new TextBox
            {
                Location = new Point(100, 15),
                Width = 220
            };
            Controls.Add(_txtTeklifNo);

            _btnGit = new Button
            {
                Text = "Git",
                DialogResult = DialogResult.OK,
                Size = new Size(80, 28),
                Location = new Point(120, 55)
            };
            _btnGit.Click += BtnGit_Click;
            Controls.Add(_btnGit);

            _btnIptal = new Button
            {
                Text = "İptal",
                DialogResult = DialogResult.Cancel,
                Size = new Size(80, 28),
                Location = new Point(210, 55)
            };
            Controls.Add(_btnIptal);

            AcceptButton = _btnGit;
            CancelButton = _btnIptal;
        }

        private void BtnGit_Click(object sender, EventArgs e)
        {
            TeklifNo = _txtTeklifNo?.Text?.Trim();
            if (string.IsNullOrEmpty(TeklifNo))
            {
                MessageBox.Show("Lütfen teklif numarası girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }
    }
}
