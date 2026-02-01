using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Windows;

namespace VegaAsis.Windows.Forms
{
    public class KonutTeminatlariForm : Form
    {
        private DataGridView _dgvTeminatlar;
        private Button _btnKapat;

        public KonutTeminatlariForm()
        {
            InitializeComponent();
            Shown += KonutTeminatlariForm_Shown;
        }

        private async void KonutTeminatlariForm_Shown(object sender, EventArgs e)
        {
            await LoadTeminatlarAsync().ConfigureAwait(true);
        }

        private void InitializeComponent()
        {
            Text = "Konut Teminatları";
            Size = new Size(500, 380);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var grpTeminatlar = new GroupBox
            {
                Text = "Teminat Listesi",
                Location = new Point(12, 12),
                Size = new Size(460, 280),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            _dgvTeminatlar = new DataGridView
            {
                Location = new Point(12, 25),
                Size = new Size(436, 245),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvTeminatlar.Columns.Add("Kod", "Kod");
            _dgvTeminatlar.Columns.Add("Ad", "Teminat Adı");
            _dgvTeminatlar.Columns.Add("Aciklama", "Açıklama");
            _dgvTeminatlar.Columns.Add("Prim", "Prim");
            _dgvTeminatlar.Columns.Add("Secili", "Seçili");
            grpTeminatlar.Controls.Add(_dgvTeminatlar);
            Controls.Add(grpTeminatlar);

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _btnKapat = new Button
            {
                Text = "Kapat",
                DialogResult = DialogResult.Cancel,
                Size = new Size(100, 30),
                Location = new Point(372, 10)
            };
            _btnKapat.Click += (s, e) => Close();
            pnlBottom.Controls.Add(_btnKapat);
            Controls.Add(pnlBottom);
        }

        private async Task LoadTeminatlarAsync()
        {
            if (_dgvTeminatlar == null) return;
            _dgvTeminatlar.Rows.Clear();
            if (!ServiceLocator.IsInitialized) return;
            try
            {
                var service = ServiceLocator.Resolve<VegaAsis.Core.Contracts.IKonutService>();
                if (service == null) return;
                var list = await service.GetTeminatlarAsync().ConfigureAwait(true);
                if (list == null) return;
                foreach (var t in list)
                {
                    _dgvTeminatlar.Rows.Add(
                        t.Kod ?? "",
                        t.Ad ?? "",
                        t.Aciklama ?? "",
                        t.Prim.HasValue ? t.Prim.Value.ToString("N2") : "",
                        t.Secili ? "Evet" : "Hayır");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Konut teminatlar yüklenirken hata: " + ex.Message);
            }
        }
    }
}
