using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace VegaAsis.Windows.Forms
{
    public class PDFExportForm : Form
    {
        private GroupBox _grpFormat;
        private GroupBox _grpBolumler;
        private GroupBox _grpKayitYeri;
        private Panel _pnlAlt;
        
        private RadioButton _rbPdf;
        private RadioButton _rbExcel;
        private RadioButton _rbPng;
        
        private CheckBox _chkMusteriBilgileri;
        private CheckBox _chkTeklifDetaylari;
        private CheckBox _chkFiyatTablosu;
        private CheckBox _chkSirketLogosu;
        
        private TextBox _txtDosyaAdi;
        private Button _btnGozat;
        private TextBox _txtKlasor;
        
        private Button _btnExport;
        private Button _btnIptal;
        
        private FolderBrowserDialog _folderDialog;

        public PDFExportForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Dışa Aktar";
            Size = new Size(450, 350);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            _folderDialog = new FolderBrowserDialog();

            // Format Seçimi GroupBox
            _grpFormat = new GroupBox
            {
                Text = "Format Seçimi",
                Left = 12,
                Top = 12,
                Width = 200,
                Height = 100
            };

            _rbPdf = new RadioButton
            {
                Text = "PDF",
                Left = 12,
                Top = 20,
                Checked = true
            };
            _grpFormat.Controls.Add(_rbPdf);

            _rbExcel = new RadioButton
            {
                Text = "Excel (.xlsx)",
                Left = 12,
                Top = 45
            };
            _grpFormat.Controls.Add(_rbExcel);

            _rbPng = new RadioButton
            {
                Text = "PNG (görsel)",
                Left = 12,
                Top = 70
            };
            _grpFormat.Controls.Add(_rbPng);

            Controls.Add(_grpFormat);

            // Dahil Edilecek Bölümler GroupBox
            _grpBolumler = new GroupBox
            {
                Text = "Dahil Edilecek Bölümler",
                Left = 220,
                Top = 12,
                Width = 210,
                Height = 100
            };

            _chkMusteriBilgileri = new CheckBox
            {
                Text = "Müşteri Bilgileri",
                Left = 12,
                Top = 20,
                Checked = true
            };
            _grpBolumler.Controls.Add(_chkMusteriBilgileri);

            _chkTeklifDetaylari = new CheckBox
            {
                Text = "Teklif Detayları",
                Left = 12,
                Top = 40,
                Checked = true
            };
            _grpBolumler.Controls.Add(_chkTeklifDetaylari);

            _chkFiyatTablosu = new CheckBox
            {
                Text = "Fiyat Tablosu",
                Left = 12,
                Top = 60,
                Checked = true
            };
            _grpBolumler.Controls.Add(_chkFiyatTablosu);

            _chkSirketLogosu = new CheckBox
            {
                Text = "Şirket Logosu",
                Left = 12,
                Top = 80
            };
            _grpBolumler.Controls.Add(_chkSirketLogosu);

            Controls.Add(_grpBolumler);

            // Kayıt Yeri GroupBox
            _grpKayitYeri = new GroupBox
            {
                Text = "Kayıt Yeri",
                Left = 12,
                Top = 120,
                Width = 418,
                Height = 80
            };

            var lblDosyaAdi = new Label
            {
                Text = "Dosya adı:",
                Left = 12,
                Top = 20,
                AutoSize = true
            };
            _grpKayitYeri.Controls.Add(lblDosyaAdi);

            _txtDosyaAdi = new TextBox
            {
                Left = 12,
                Top = 38,
                Width = 280
            };
            _grpKayitYeri.Controls.Add(_txtDosyaAdi);

            _btnGozat = new Button
            {
                Text = "...",
                Left = 300,
                Top = 36,
                Width = 30,
                Height = 23
            };
            _btnGozat.Click += BtnGozat_Click;
            _grpKayitYeri.Controls.Add(_btnGozat);

            var lblKlasor = new Label
            {
                Text = "Klasör:",
                Left = 12,
                Top = 65,
                AutoSize = true
            };
            _grpKayitYeri.Controls.Add(lblKlasor);

            _txtKlasor = new TextBox
            {
                Left = 60,
                Top = 63,
                Width = 270,
                ReadOnly = true
            };
            _grpKayitYeri.Controls.Add(_txtKlasor);

            Controls.Add(_grpKayitYeri);

            // Alt Panel
            _pnlAlt = new Panel
            {
                Left = 0,
                Top = 300,
                Width = 450,
                Height = 50
            };

            _btnExport = new Button
            {
                Text = "Dışa Aktar",
                Left = 250,
                Top = 10,
                Width = 100,
                Height = 30
            };
            _btnExport.Click += BtnExport_Click;
            _pnlAlt.Controls.Add(_btnExport);

            _btnIptal = new Button
            {
                Text = "İptal",
                Left = 360,
                Top = 10,
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };
            _btnIptal.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            _pnlAlt.Controls.Add(_btnIptal);

            Controls.Add(_pnlAlt);

            AcceptButton = _btnExport;
            CancelButton = _btnIptal;
        }

        private void BtnGozat_Click(object sender, EventArgs e)
        {
            if (_folderDialog.ShowDialog() == DialogResult.OK)
            {
                _txtKlasor.Text = _folderDialog.SelectedPath;
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtDosyaAdi.Text))
            {
                MessageBox.Show("Dosya adı girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_txtKlasor.Text))
            {
                MessageBox.Show("Kayıt klasörü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var format = _rbPdf.Checked ? "pdf" : _rbExcel.Checked ? "xlsx" : "png";
                var dosyaAdi = Path.GetFileNameWithoutExtension(_txtDosyaAdi.Text.Trim());
                if (string.IsNullOrEmpty(dosyaAdi)) dosyaAdi = "Teklif";
                var tamYol = Path.Combine(_txtKlasor.Text.Trim(), dosyaAdi + "." + format);

                if (_rbExcel.Checked)
                {
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Teklif");
                        if (_chkMusteriBilgileri.Checked)
                        {
                            ws.Cell(1, 1).Value = "Müşteri Bilgileri";
                            ws.Cell(2, 1).Value = "Bu bölüm seçildi.";
                        }
                        if (_chkTeklifDetaylari.Checked)
                        {
                            ws.Cell(4, 1).Value = "Teklif Detayları";
                            ws.Cell(5, 1).Value = "Bu bölüm seçildi.";
                        }
                        if (_chkFiyatTablosu.Checked)
                        {
                            ws.Cell(7, 1).Value = "Fiyat Tablosu";
                            ws.Cell(8, 1).Value = "Şirket"; ws.Cell(8, 2).Value = "Fiyat";
                        }
                        wb.SaveAs(tamYol);
                    }
                }
                else
                {
                    MessageBox.Show("PDF ve PNG formatı için dışa aktarma henüz desteklenmiyor. Lütfen Excel formatını seçin.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MessageBox.Show("Dışa aktarma tamamlandı:\n" + tamYol, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string SelectedFormat
        {
            get
            {
                if (_rbPdf.Checked) return "PDF";
                if (_rbExcel.Checked) return "Excel";
                if (_rbPng.Checked) return "PNG";
                return "PDF";
            }
        }

        public bool IncludeMusteriBilgileri => _chkMusteriBilgileri.Checked;
        public bool IncludeTeklifDetaylari => _chkTeklifDetaylari.Checked;
        public bool IncludeFiyatTablosu => _chkFiyatTablosu.Checked;
        public bool IncludeSirketLogosu => _chkSirketLogosu.Checked;

        public string DosyaAdi => _txtDosyaAdi.Text?.Trim();
        public string KlasorYolu => _txtKlasor.Text?.Trim();
    }
}
