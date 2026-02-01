using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace VegaAsis.Windows.Forms
{
    public class ExcelOkuForm : Form
    {
        private TextBox _txtDosya;
        private Button _btnAc;
        private DataGridView _dgvPreview;
        private Button _btnIceriAktar;
        private Button _btnKapat;

        private string _selectedFilePath;
        private DataTable _loadedData;

        public DataTable LoadedData { get { return _loadedData; } }

        public ExcelOkuForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Excel Oku";
            Size = new Size(800, 550);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(600, 400);

            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            var lblDosya = new Label { Text = "Dosya:", Location = new Point(10, 12), AutoSize = true };
            topPanel.Controls.Add(lblDosya);

            _txtDosya = new TextBox
            {
                Location = new Point(60, 10),
                Width = 520,
                ReadOnly = true
            };
            topPanel.Controls.Add(_txtDosya);

            _btnAc = new Button
            {
                Text = "Aç...",
                Location = new Point(590, 8),
                Size = new Size(70, 26)
            };
            _btnAc.Click += BtnAc_Click;
            topPanel.Controls.Add(_btnAc);

            Controls.Add(topPanel);

            _dgvPreview = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Controls.Add(_dgvPreview);

            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };

            _btnIceriAktar = new Button
            {
                Text = "İçe Aktar",
                Location = new Point(10, 10),
                Size = new Size(100, 28)
            };
            _btnIceriAktar.Click += BtnIceriAktar_Click;
            _btnIceriAktar.Enabled = false;
            bottomPanel.Controls.Add(_btnIceriAktar);

            _btnKapat = new Button
            {
                Text = "Kapat",
                Location = new Point(120, 10),
                Size = new Size(80, 28)
            };
            _btnKapat.Click += (s, e) => Close();
            bottomPanel.Controls.Add(_btnKapat);

            Controls.Add(bottomPanel);
        }

        private void BtnAc_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx|Tüm Dosyalar (*.*)|*.*";
                dlg.Title = "Excel Dosyası Seç";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                _selectedFilePath = dlg.FileName;
                _txtDosya.Text = _selectedFilePath;

                try
                {
                    using (var workbook = new XLWorkbook(_selectedFilePath))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var range = worksheet.RangeUsed();
                        if (range == null)
                        {
                            _loadedData = new DataTable();
                            _dgvPreview.DataSource = null;
                            _btnIceriAktar.Enabled = false;
                            MessageBox.Show("Sayfa boş.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        try
                        {
                            _loadedData = range.AsTable().AsNativeDataTable();
                        }
                        catch
                        {
                            _loadedData = ReadRangeToDataTable(range);
                        }
                    }

                    _dgvPreview.DataSource = _loadedData;
                    _btnIceriAktar.Enabled = _loadedData != null && _loadedData.Rows.Count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Excel okuma hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _loadedData = null;
                    _dgvPreview.DataSource = null;
                    _btnIceriAktar.Enabled = false;
                }
            }
        }

        private static DataTable ReadRangeToDataTable(IXLRange range)
        {
            var dt = new DataTable();
            int colCount = range.ColumnCount();
            int rowCount = range.RowCount();
            if (colCount <= 0 || rowCount <= 0) return dt;

            for (int i = 0; i < colCount; i++)
                dt.Columns.Add("Sütun" + (i + 1));

            for (int r = 1; r <= rowCount; r++)
            {
                var dtRow = dt.NewRow();
                for (int c = 1; c <= colCount; c++)
                {
                    var cell = range.Cell(r, c);
                    dtRow[c - 1] = cell.GetString();
                }
                dt.Rows.Add(dtRow);
            }
            return dt;
        }

        private void BtnIceriAktar_Click(object sender, EventArgs e)
        {
            if (_loadedData == null || _loadedData.Rows.Count == 0)
            {
                MessageBox.Show("Önce Excel dosyası açın.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(string.Format("{0} satır veri yüklendi.\n(İş mantığı entegrasyonu bekleniyor)", _loadedData.Rows.Count), "İçe Aktar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
