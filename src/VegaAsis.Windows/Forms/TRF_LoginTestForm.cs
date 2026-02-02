using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Windows.Robot;

namespace VegaAsis.Windows.Forms
{
    /// <summary>
    /// Faz 4 — Tüm şirketler için login testi çalıştırır ve sonuçları grid'de gösterir.
    /// DOCS/TRF-Faz4-Test-Plani.md ile birlikte kullanılır.
    /// </summary>
    public class TRF_LoginTestForm : BaseForm
    {
        private DataGridView _dgv;
        private Button _btnCalistir;
        private Button _btnKopyala;
        private Button _btnKapat;
        private Label _lblAciklama;

        public TRF_LoginTestForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "TRF Login Testi (Faz 4)";
            Size = new Size(700, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(600, 400);

            _lblAciklama = new Label
            {
                Text = "Tüm şirketler için giriş testi. Kimlik Bilgileri'nden credential tanımlı şirketler test edilir.",
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(10, 8, 10, 0),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.DimGray
            };

            _dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };
            _dgv.Columns.Add("CompanyId", "Kod");
            _dgv.Columns.Add("CompanyName", "Şirket");
            _dgv.Columns.Add("Success", "Başarılı");
            _dgv.Columns.Add("Message", "Mesaj");
            _dgv.Columns["CompanyId"].Width = 80;
            _dgv.Columns["Success"].Width = 70;

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _btnCalistir = new Button
            {
                Text = "Testi Çalıştır",
                Size = new Size(120, 32),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnCalistir.FlatAppearance.BorderSize = 0;
            _btnCalistir.Click += BtnCalistir_Click;

            _btnKopyala = new Button { Text = "Raporu Kopyala", Size = new Size(120, 32) };
            _btnKopyala.Click += BtnKopyala_Click;

            _btnKapat = new Button { Text = "Kapat", Size = new Size(90, 32), DialogResult = DialogResult.Cancel };
            panel.Controls.Add(_btnCalistir);
            panel.Controls.Add(_btnKopyala);
            panel.Controls.Add(_btnKapat);

            void LayoutPanel(object s, EventArgs e)
            {
                var p = (Panel)s;
                _btnKapat.Location = new Point(p.Width - _btnKapat.Width - 12, (p.Height - _btnKapat.Height) / 2);
                _btnKopyala.Location = new Point(_btnKapat.Left - _btnKopyala.Width - 8, (p.Height - _btnKopyala.Height) / 2);
                _btnCalistir.Location = new Point(12, (p.Height - _btnCalistir.Height) / 2);
            }
            panel.Layout += LayoutPanel;
            LayoutPanel(panel, EventArgs.Empty);

            Controls.Add(_dgv);
            Controls.Add(panel);
            Controls.Add(_lblAciklama);
            Controls.SetChildIndex(_lblAciklama, 0);
            Controls.SetChildIndex(panel, 1);
            Controls.SetChildIndex(_dgv, 2);
            CancelButton = _btnKapat;
        }

        private async void BtnCalistir_Click(object sender, EventArgs e)
        {
            _btnCalistir.Enabled = false;
            Cursor = Cursors.WaitCursor;
            _dgv.Rows.Clear();

            IBrowserDriver driver = null;
            try
            {
                driver = new ChromeBrowserDriver(headless: false);
            }
            catch (Exception ex)
            {
                ShowError("Chrome başlatılamadı: " + ex.Message);
                _btnCalistir.Enabled = true;
                Cursor = Cursors.Default;
                return;
            }

            try
            {
                var companyIds = CompanyRobotRegistry.GetAllCompanyIds().ToList();
                Guid? userId = null;
                try
                {
                    if (ServiceLocator.IsInitialized)
                    {
                        var auth = ServiceLocator.Resolve<IAuthService>();
                        if (auth != null) userId = auth.GetCurrentUserId;
                    }
                }
                catch { }

                var results = await AllLoginsRunner.RunAsync(driver, companyIds, userId).ConfigureAwait(true);

                if (InvokeRequired)
                {
                    Invoke(new Action(() => FillGrid(results)));
                }
                else
                {
                    FillGrid(results);
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                    Invoke(new Action(() => ShowError("Test hatası: " + ex.Message)));
                else
                    ShowError("Test hatası: " + ex.Message);
            }
            finally
            {
                if (driver != null)
                {
                    try { driver.Dispose(); } catch { }
                }
                if (InvokeRequired)
                    Invoke(new Action(() => { _btnCalistir.Enabled = true; Cursor = Cursors.Default; }));
                else
                {
                    _btnCalistir.Enabled = true;
                    Cursor = Cursors.Default;
                }
            }
        }

        private void FillGrid(List<AllLoginResult> results)
        {
            _dgv.Rows.Clear();
            foreach (var r in results ?? new List<AllLoginResult>())
            {
                _dgv.Rows.Add(r.CompanyId, r.CompanyName, r.Success ? "Evet" : "Hayır", r.Message ?? "");
            }
        }

        private void BtnKopyala_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine("TRF Login Test Raporu\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            sb.AppendLine("Kod\tŞirket\tBaşarılı\tMesaj");
            foreach (DataGridViewRow row in _dgv.Rows)
            {
                if (row.IsNewRow) continue;
                var kod = row.Cells["CompanyId"]?.Value?.ToString() ?? "";
                var sirket = row.Cells["CompanyName"]?.Value?.ToString() ?? "";
                var ok = row.Cells["Success"]?.Value?.ToString() ?? "";
                var msg = row.Cells["Message"]?.Value?.ToString() ?? "";
                sb.AppendLine(kod + "\t" + sirket + "\t" + ok + "\t" + msg);
            }
            try
            {
                Clipboard.SetText(sb.ToString());
                ShowInfo("Rapor panoya kopyalandı.");
            }
            catch (Exception ex)
            {
                ShowError("Kopyalama hatası: " + ex.Message);
            }
        }
    }
}
