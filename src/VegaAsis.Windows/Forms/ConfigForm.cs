using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class ConfigForm : Form
    {
        private GroupBox _grpProxy;
        private CheckBox _chkProxyAktif;
        private TextBox _txtProxyHost;
        private TextBox _txtProxyPort;
        private TextBox _txtProxyUser;
        private TextBox _txtProxyPass;
        private GroupBox _grpGenel;
        private TextBox _txtVarsayilanTarayici;
        private CheckBox _chkOtomatikGuncelleme;
        private Button _btnKaydet;
        private Button _btnBenchmark;
        private Button _btnIptal;

        public ConfigForm()
        {
            InitializeComponent();
            ConfigStorage.Load();
            Yükle();
        }

        private void InitializeComponent()
        {
            Text = "Genel Ayarlar";
            Size = new Size(480, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            _grpProxy = new GroupBox
            {
                Text = "Proxy Ayarları",
                Location = new Point(12, 12),
                Size = new Size(440, 140),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            _chkProxyAktif = new CheckBox { Text = "Proxy kullan", Location = new Point(12, 24), AutoSize = true };
            int py = 48;
            AddRow(_grpProxy, "Host:", ref _txtProxyHost, ref py);
            AddRow(_grpProxy, "Port:", ref _txtProxyPort, ref py);
            AddRow(_grpProxy, "Kullanıcı:", ref _txtProxyUser, ref py);
            AddRow(_grpProxy, "Şifre:", ref _txtProxyPass, ref py);
            _grpProxy.Controls.Add(_chkProxyAktif);
            Controls.Add(_grpProxy);

            _grpGenel = new GroupBox
            {
                Text = "Genel",
                Location = new Point(12, 160),
                Size = new Size(440, 100),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            int gy = 28;
            AddRow(_grpGenel, "Varsayılan tarayıcı:", ref _txtVarsayilanTarayici, ref gy);
            _txtVarsayilanTarayici.Text = "Chrome";
            _chkOtomatikGuncelleme = new CheckBox { Text = "Otomatik güncelleme kontrolü", Location = new Point(12, 58), AutoSize = true, Checked = true };
            _grpGenel.Controls.Add(_chkOtomatikGuncelleme);
            Controls.Add(_grpGenel);

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };
            _btnKaydet = new Button { Text = "Kaydet", Size = new Size(90, 32), Location = new Point(12, 10) };
            _btnBenchmark = new Button { Text = "Benchmark", Size = new Size(90, 32), Location = new Point(110, 10) };
            _btnIptal = new Button { Text = "İptal", DialogResult = DialogResult.Cancel, Size = new Size(80, 32), Location = new Point(208, 10) };
            _btnKaydet.Click += BtnKaydet_Click;
            _btnBenchmark.Click += (s, e) =>
            {
                if (!VegaAsis.Windows.ServiceLocator.IsInitialized) return;
                var admin = new AdminPanelForm(
                    VegaAsis.Windows.ServiceLocator.Resolve<VegaAsis.Core.Contracts.ICompanySettingsService>(),
                    VegaAsis.Windows.ServiceLocator.Resolve<VegaAsis.Core.Contracts.IUserManagementService>(),
                    VegaAsis.Windows.ServiceLocator.Resolve<VegaAsis.Core.Contracts.IAuthService>());
                admin.Show();
                var tc = admin.Controls[0] as TabControl;
                if (tc != null)
                {
                    for (int i = 0; i < tc.TabPages.Count; i++)
                    {
                        if (tc.TabPages[i].Text == "Benchmark") { tc.SelectedIndex = i; break; }
                    }
                }
            };
            pnlBottom.Controls.AddRange(new Control[] { _btnKaydet, _btnBenchmark, _btnIptal });
            Controls.Add(pnlBottom);

            AcceptButton = _btnKaydet;
            CancelButton = _btnIptal;
        }

        private void AddRow(Control parent, string labelText, ref TextBox txt, ref int y)
        {
            var lbl = new Label { Text = labelText, Left = 12, Top = y, Width = 100 };
            txt = new TextBox { Left = 120, Top = y - 2, Width = 200 };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            y += 28;
        }

        private void Yükle()
        {
            _chkProxyAktif.Checked = ConfigStorage.ProxyAktif;
            _txtProxyHost.Text = ConfigStorage.ProxyHost ?? "";
            _txtProxyPort.Text = ConfigStorage.ProxyPort ?? "8080";
            _txtProxyUser.Text = ConfigStorage.ProxyUser ?? "";
            _txtProxyPass.Text = ConfigStorage.ProxyPass ?? "";
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigStorage.ProxyAktif = _chkProxyAktif.Checked;
                ConfigStorage.ProxyHost = _txtProxyHost.Text?.Trim() ?? "";
                ConfigStorage.ProxyPort = _txtProxyPort.Text?.Trim() ?? "8080";
                ConfigStorage.ProxyUser = _txtProxyUser.Text?.Trim() ?? "";
                ConfigStorage.ProxyPass = _txtProxyPass.Text?.Trim() ?? "";
                ConfigStorage.Save();
                MessageBox.Show("Ayarlar kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public static class ConfigStorage
    {
        private static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VegaAsis", "config.txt");

        public static bool ProxyAktif { get; set; }
        public static string ProxyHost { get; set; }
        public static string ProxyPort { get; set; }
        public static string ProxyUser { get; set; }
        public static string ProxyPass { get; set; }

        public static void Save()
        {
            var dir = Path.GetDirectoryName(ConfigPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(ConfigPath, string.Format("ProxyAktif={0}\nProxyHost={1}\nProxyPort={2}\nProxyUser={3}\nProxyPass={4}",
                ProxyAktif, ProxyHost ?? "", ProxyPort ?? "", ProxyUser ?? "", ProxyPass ?? ""));
        }

        public static void Load()
        {
            if (!File.Exists(ConfigPath)) return;
            foreach (var line in File.ReadAllLines(ConfigPath))
            {
                var idx = line.IndexOf('=');
                if (idx < 0) continue;
                var key = line.Substring(0, idx).Trim();
                var val = line.Substring(idx + 1).Trim();
                if (key == "ProxyAktif") ProxyAktif = val == "True" || val == "1";
                else if (key == "ProxyHost") ProxyHost = val;
                else if (key == "ProxyPort") ProxyPort = val;
                else if (key == "ProxyUser") ProxyUser = val;
                else if (key == "ProxyPass") ProxyPass = val;
            }
        }
    }
}
