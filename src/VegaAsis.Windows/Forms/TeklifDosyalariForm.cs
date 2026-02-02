using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class TeklifDosyalariForm : Form
    {
        private Panel _pnlUst;
        private Panel _pnlAlt;
        private Label _lblTeklifNo;
        private Button _btnDosyaEkle;
        private ListView _lvDosyalar;
        private Button _btnIndir;
        private Button _btnSil;
        private Button _btnKapat;
        private string _storageDir;

        public string TeklifNo { get; set; }

        public TeklifDosyalariForm(string teklifNo)
        {
            TeklifNo = teklifNo ?? throw new ArgumentNullException(nameof(teklifNo));

            InitializeComponent();
            _storageDir = GetStorageDir();
            EnsureStorage();
            LoadFiles();
        }

        private void InitializeComponent()
        {
            Text = "Teklif Dosyaları";
            Size = new Size(600, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Üst Panel
            _pnlUst = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            _lblTeklifNo = new Label
            {
                Text = "Teklif No: " + TeklifNo,
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                AutoSize = true,
                Left = 10,
                Top = 15
            };

            _btnDosyaEkle = new Button
            {
                Text = "Dosya Ekle",
                Width = 100,
                Height = 28,
                Left = 450,
                Top = 11
            };
            _btnDosyaEkle.Click += BtnDosyaEkle_Click;

            _pnlUst.Controls.Add(_lblTeklifNo);
            _pnlUst.Controls.Add(_btnDosyaEkle);
            Controls.Add(_pnlUst);

            // ListView
            _lvDosyalar = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _lvDosyalar.Columns.Add("Dosya Adı", 200);
            _lvDosyalar.Columns.Add("Boyut", 100);
            _lvDosyalar.Columns.Add("Tarih", 120);
            _lvDosyalar.Columns.Add("Tür", 100);

            Controls.Add(_lvDosyalar);

            // Alt Panel
            _pnlAlt = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };

            _btnIndir = new Button
            {
                Text = "İndir",
                Width = 80,
                Height = 28,
                Left = 10,
                Top = 11
            };
            _btnIndir.Click += BtnIndir_Click;

            _btnSil = new Button
            {
                Text = "Sil",
                Width = 80,
                Height = 28,
                Left = 100,
                Top = 11
            };
            _btnSil.Click += BtnSil_Click;

            _btnKapat = new Button
            {
                Text = "Kapat",
                Width = 80,
                Height = 28,
                DialogResult = DialogResult.Cancel,
                Left = 500,
                Top = 11
            };
            _btnKapat.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            _pnlAlt.Controls.Add(_btnIndir);
            _pnlAlt.Controls.Add(_btnSil);
            _pnlAlt.Controls.Add(_btnKapat);
            Controls.Add(_pnlAlt);
        }

        private string GetStorageDir()
        {
            var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VegaAsis", "attachments");
            return Path.Combine(baseDir, TeklifNo);
        }

        private void EnsureStorage()
        {
            if (!Directory.Exists(_storageDir))
                Directory.CreateDirectory(_storageDir);
        }

        private void LoadFiles()
        {
            _lvDosyalar.Items.Clear();
            if (!Directory.Exists(_storageDir)) return;
            var files = new DirectoryInfo(_storageDir).GetFiles()
                .OrderByDescending(f => f.CreationTimeUtc)
                .ToList();
            foreach (var f in files)
            {
                var fileSize = FormatFileSize(f.Length);
                var fileExtension = f.Extension.ToUpperInvariant().TrimStart('.');
                var fileType = GetFileType(fileExtension);
                var item = new ListViewItem(f.Name);
                item.SubItems.Add(fileSize);
                item.SubItems.Add(f.CreationTime.ToString("dd.MM.yyyy HH:mm"));
                item.SubItems.Add(fileType);
                item.Tag = f.FullName;
                _lvDosyalar.Items.Add(item);
            }
        }

        private void BtnDosyaEkle_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Tüm Dosyalar (*.*)|*.*|PDF Dosyaları (*.pdf)|*.pdf|Resim Dosyaları (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|Word Dosyaları (*.doc;*.docx)|*.doc;*.docx";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        EnsureStorage();
                        var fileInfo = new FileInfo(openFileDialog.FileName);
                        var destPath = Path.Combine(_storageDir, fileInfo.Name);
                        if (File.Exists(destPath))
                        {
                            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                            var ext = fileInfo.Extension;
                            destPath = Path.Combine(_storageDir, name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ext);
                        }
                        File.Copy(openFileDialog.FileName, destPath, true);
                        LoadFiles();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Dosya eklenemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnIndir_Click(object sender, EventArgs e)
        {
            if (_lvDosyalar.SelectedItems.Count == 0)
            {
                MessageBox.Show("Lütfen indirmek için bir dosya seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = _lvDosyalar.SelectedItems[0];
            var sourcePath = selectedItem.Tag as string;

            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                MessageBox.Show("Dosya bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = selectedItem.Text;
                saveFileDialog.Filter = "Tüm Dosyalar (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(sourcePath, saveFileDialog.FileName, true);
                        MessageBox.Show("Dosya başarıyla indirildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Dosya indirme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (_lvDosyalar.SelectedItems.Count == 0)
            {
                MessageBox.Show("Lütfen silmek için bir dosya seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = _lvDosyalar.SelectedItems[0];
            var result = MessageBox.Show(
                $"'{selectedItem.Text}' dosyasını silmek istediğinizden emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var path = selectedItem.Tag as string;
                try
                {
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                        File.Delete(path);
                    LoadFiles();
                    MessageBox.Show("Dosya silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Dosya silme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

        private string GetFileType(string extension)
        {
            switch (extension.ToUpper())
            {
                case "PDF":
                    return "PDF";
                case "JPG":
                case "JPEG":
                case "PNG":
                case "GIF":
                case "BMP":
                    return "Resim";
                case "DOC":
                case "DOCX":
                    return "Word";
                case "XLS":
                case "XLSX":
                    return "Excel";
                case "TXT":
                    return "Metin";
                default:
                    return extension;
            }
        }
    }
}
