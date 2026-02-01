using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class CokluFiyatForm : Form
    {
        private DataGridView _dgv;
        private Button _btnKapat;

        public CokluFiyatForm(IEnumerable<FiyatSatir> satirlar = null)
        {
            InitializeComponent();
            if (satirlar != null)
                Yukle(satirlar);
        }

        private void InitializeComponent()
        {
            Text = "Çoklu Fiyat Karşılaştırma";
            Size = new Size(700, 450);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(500, 300);

            _dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            _dgv.Columns.Add("Sirket", "Şirket");
            _dgv.Columns.Add("Trafik", "Trafik");
            _dgv.Columns.Add("Kasko", "Kasko");
            _dgv.Columns.Add("TSS", "TSS");
            _dgv.Columns.Add("DASK", "DASK");
            _dgv.Columns.Add("IMM", "IMM");
            _dgv.Columns.Add("Yuzde", "%");
            _dgv.Columns.Add("Uyari", "Uyarı");
            Controls.Add(_dgv);

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(10) };
            _btnKapat = new Button { Text = "Kapat", Size = new Size(80, 28), Location = new Point(10, 8) };
            _btnKapat.Click += (s, e) => Close();
            pnlBottom.Controls.Add(_btnKapat);
            Controls.Add(pnlBottom);
        }

        public void Yukle(IEnumerable<FiyatSatir> satirlar)
        {
            _dgv.Rows.Clear();
            if (satirlar == null) return;
            foreach (var s in satirlar)
            {
                var trafik = s.Trafik.HasValue ? s.Trafik.Value.ToString("N2", new CultureInfo("tr-TR")) : "-";
                var kasko = s.Kasko.HasValue ? s.Kasko.Value.ToString("N2", new CultureInfo("tr-TR")) : "-";
                var tss = s.Tss.HasValue ? s.Tss.Value.ToString("N2", new CultureInfo("tr-TR")) : "-";
                var dask = s.Dask.HasValue ? s.Dask.Value.ToString("N2", new CultureInfo("tr-TR")) : "-";
                var imm = s.Imm.HasValue ? s.Imm.Value.ToString("N2", new CultureInfo("tr-TR")) : "-";
                var yuzde = s.Yuzde.HasValue ? "%" + s.Yuzde : "-";
                _dgv.Rows.Add(s.Sirket, trafik, kasko, tss, dask, imm, yuzde, s.Uyari ?? "");
            }
        }
    }

    public class FiyatSatir
    {
        public string Sirket { get; set; }
        public decimal? Trafik { get; set; }
        public decimal? Kasko { get; set; }
        public decimal? Tss { get; set; }
        public decimal? Dask { get; set; }
        public decimal? Imm { get; set; }
        public int? Yuzde { get; set; }
        public string Uyari { get; set; }
    }
}
