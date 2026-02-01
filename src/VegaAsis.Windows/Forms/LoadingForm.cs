using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class LoadingForm : Form
    {
        private Label _lblMesaj;
        private ProgressBar _progressBar;

        public string Mesaj
        {
            get { return _lblMesaj?.Text ?? "Yükleniyor..."; }
            set { if (_lblMesaj != null) _lblMesaj.Text = value ?? "Yükleniyor..."; }
        }

        public bool Indeterminate
        {
            get { return _progressBar != null && _progressBar.Style == ProgressBarStyle.Marquee; }
            set { if (_progressBar != null) _progressBar.Style = value ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous; }
        }

        public LoadingForm(string mesaj = "Yükleniyor...")
        {
            InitializeComponent(mesaj);
        }

        private void InitializeComponent(string mesaj)
        {
            Text = "Bekleyin";
            Size = new Size(360, 140);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            TopMost = true;

            _lblMesaj = new Label
            {
                Text = mesaj ?? "Yükleniyor...",
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            Controls.Add(_lblMesaj);

            _progressBar = new ProgressBar
            {
                Location = new Point(20, 55),
                Size = new Size(310, 25),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30
            };
            Controls.Add(_progressBar);
        }

        public static void ShowWhile(Form owner, Action work, string mesaj = "Yükleniyor...")
        {
            LoadingForm loading = null;
            Exception workEx = null;

            var t = new System.Threading.Thread(() =>
            {
                try
                {
                    work();
                }
                catch (Exception ex)
                {
                    workEx = ex;
                }
                finally
                {
                    try
                    {
                        if (loading != null && !loading.IsDisposed)
                            loading.Invoke(new Action(() => loading.Close()));
                    }
                    catch { }
                }
            });

            loading = new LoadingForm(mesaj);
            loading.FormClosed += (s, e) => t.Join(100);
            t.Start();

            loading.ShowDialog(owner);

            if (workEx != null)
                throw workEx;
        }

        /// <summary>
        /// Modal bekleme penceresi gösterir. Kapatmak için dönen formu Close() ile kapatın.
        /// </summary>
        public static LoadingForm ShowModal(Form owner, string mesaj = "Yükleniyor...")
        {
            var f = new LoadingForm(mesaj);
            f.Show(owner);
            return f;
        }
    }
}
