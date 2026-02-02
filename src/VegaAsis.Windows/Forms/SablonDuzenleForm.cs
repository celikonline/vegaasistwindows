using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Windows;

namespace VegaAsis.Windows.Forms
{
    public class SablonDuzenleForm : Form
    {
        private ListBox _lstSablonlar;
        private TextBox _txtSablonAdi;
        private ComboBox _cmbBrans;
        private CheckedListBox _chkSirketler;
        private Button _btnKaydet;
        private Button _btnSil;
        private Button _btnYeni;
        private Button _btnKapat;
        private List<SablonDto> _sablonlar = new List<SablonDto>();

        public SablonDuzenleForm()
        {
            InitializeComponent();
            Load += SablonDuzenleForm_Load;
        }

        private async void SablonDuzenleForm_Load(object sender, EventArgs e)
        {
            if (ServiceLocator.IsInitialized)
            {
                await VerileriYukleAsync().ConfigureAwait(true);
            }
            else
            {
                OrnekYukle();
            }
        }

        private async Task VerileriYukleAsync()
        {
            try
            {
                var sablonService = ServiceLocator.Resolve<ISablonService>();
                var companyService = ServiceLocator.Resolve<ICompanySettingsService>();
                var sablonList = await sablonService.GetAllAsync().ConfigureAwait(true);
                var sirketler = await companyService.GetSelectedCompaniesAsync().ConfigureAwait(true);
                if (sirketler == null || sirketler.Count == 0)
                    sirketler = new List<string> { "ANADOLU", "AK SİGORTA", "AXA", "HDI", "ATLAS", "SOMPO" };

                _sablonlar = sablonList != null ? sablonList.ToList() : new List<SablonDto>();
                _lstSablonlar.Items.Clear();
                foreach (var s in _sablonlar)
                    _lstSablonlar.Items.Add(s.Ad ?? "");

                _chkSirketler.Items.Clear();
                foreach (var ad in sirketler)
                    _chkSirketler.Items.Add(ad);

                if (_lstSablonlar.Items.Count > 0)
                    _lstSablonlar.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Şablonlar yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                OrnekYukle();
            }
        }

        private void InitializeComponent()
        {
            Text = "Otomatik Sorgu - Şablon Düzenle";
            Size = new Size(550, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(500, 350);

            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 160 };
            _lstSablonlar = new ListBox { Dock = DockStyle.Fill };
            _lstSablonlar.SelectedIndexChanged += (s, e) => SablonSecildi();
            split.Panel1.Controls.Add(_lstSablonlar);

            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            int y = 12;
            rightPanel.Controls.Add(new Label { Text = "Şablon Adı:", Left = 8, Top = y });
            _txtSablonAdi = new TextBox { Left = 100, Top = y - 2, Width = 250, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            rightPanel.Controls.Add(_txtSablonAdi);
            y += 32;
            rightPanel.Controls.Add(new Label { Text = "Branş:", Left = 8, Top = y });
            _cmbBrans = new ComboBox { Left = 100, Top = y - 2, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            _cmbBrans.Items.AddRange(new object[] { "TRAFİK", "KASKO", "DASK", "TSS", "KONUT", "İMM" });
            if (_cmbBrans.Items.Count > 0) _cmbBrans.SelectedIndex = 0;
            rightPanel.Controls.Add(_cmbBrans);
            y += 32;
            rightPanel.Controls.Add(new Label { Text = "Şirketler:", Left = 8, Top = y });
            _chkSirketler = new CheckedListBox { Left = 100, Top = y, Width = 250, Height = 140, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            rightPanel.Controls.Add(_chkSirketler);
            y += 150;
            _btnYeni = new Button { Text = "Yeni", Size = new Size(70, 28), Location = new Point(8, y) };
            _btnKaydet = new Button { Text = "Kaydet", Size = new Size(70, 28), Location = new Point(85, y) };
            _btnSil = new Button { Text = "Sil", Size = new Size(70, 28), Location = new Point(162, y) };
            _btnYeni.Click += (s, e) =>
            {
                _lstSablonlar.SelectedIndex = -1;
                _txtSablonAdi.Text = "Yeni Şablon";
                if (_cmbBrans.Items.Count > 0) _cmbBrans.SelectedIndex = 0;
                for (int i = 0; i < _chkSirketler.Items.Count; i++)
                    _chkSirketler.SetItemChecked(i, false);
            };
            _btnKaydet.Click += BtnKaydet_Click;
            _btnSil.Click += BtnSil_Click;
            rightPanel.Controls.AddRange(new Control[] { _btnYeni, _btnKaydet, _btnSil });
            split.Panel2.Controls.Add(rightPanel);
            Controls.Add(split);

            var pnlAlt = new Panel { Dock = DockStyle.Bottom, Height = 45 };
            _btnKapat = new Button { Text = "Kapat", Size = new Size(80, 28), Location = new Point(458, 8), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            _btnKapat.Click += (s, e) => Close();
            pnlAlt.Controls.Add(_btnKapat);
            Controls.Add(pnlAlt);

            AcceptButton = _btnKaydet;
            CancelButton = _btnKapat;
            ShowInTaskbar = false;
        }

        private void OrnekYukle()
        {
            _lstSablonlar.Items.Clear();
            _lstSablonlar.Items.AddRange(new object[] { "Varsayılan Trafik", "Kasko Hızlı", "DASK Toplu" });
            _chkSirketler.Items.Clear();
            _chkSirketler.Items.AddRange(new object[] { "ANADOLU", "AK SİGORTA", "AXA", "HDI", "ATLAS", "SOMPO" });
        }

        private void SablonSecildi()
        {
            if (_sablonlar == null || _lstSablonlar.SelectedIndex < 0 || _lstSablonlar.SelectedIndex >= _sablonlar.Count)
                return;
            var s = _sablonlar[_lstSablonlar.SelectedIndex];
            _txtSablonAdi.Text = s.Ad ?? "";
            var bransIndex = _cmbBrans.Items.IndexOf(s.Brans);
            if (bransIndex >= 0) _cmbBrans.SelectedIndex = bransIndex;
            for (int i = 0; i < _chkSirketler.Items.Count; i++)
            {
                var ad = _chkSirketler.Items[i]?.ToString();
                var secili = s.SirketAdlari != null && s.SirketAdlari.Any(x => string.Equals(x, ad, StringComparison.OrdinalIgnoreCase));
                _chkSirketler.SetItemChecked(i, secili);
            }
        }

        private async void BtnKaydet_Click(object sender, EventArgs e)
        {
            var ad = _txtSablonAdi.Text?.Trim();
            if (string.IsNullOrEmpty(ad))
            {
                MessageBox.Show("Şablon adı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ServiceLocator.IsInitialized)
            {
                if (_lstSablonlar.SelectedIndex >= 0)
                    _lstSablonlar.Items[_lstSablonlar.SelectedIndex] = ad;
                MessageBox.Show("Şablon kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                var sablonService = ServiceLocator.Resolve<ISablonService>();
                int? id = null;
                if (_lstSablonlar.SelectedIndex >= 0 && _lstSablonlar.SelectedIndex < _sablonlar.Count)
                    id = _sablonlar[_lstSablonlar.SelectedIndex].Id;
                var sirketAdlari = new List<string>();
                for (int i = 0; i < _chkSirketler.Items.Count; i++)
                {
                    if (_chkSirketler.GetItemChecked(i))
                        sirketAdlari.Add(_chkSirketler.Items[i]?.ToString() ?? "");
                }
                var dto = new SablonDto
                {
                    Id = id,
                    Ad = ad,
                    Brans = _cmbBrans.SelectedItem?.ToString() ?? "",
                    SirketAdlari = sirketAdlari
                };
                await sablonService.SaveAsync(dto).ConfigureAwait(true);
                await VerileriYukleAsync().ConfigureAwait(true);
                MessageBox.Show("Şablon kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydetme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSil_Click(object sender, EventArgs e)
        {
            if (_lstSablonlar.SelectedIndex < 0) return;
            if (_lstSablonlar.SelectedIndex >= _sablonlar.Count) return;
            var result = MessageBox.Show("Seçili şablonu silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;
            if (!ServiceLocator.IsInitialized)
            {
                _lstSablonlar.Items.RemoveAt(_lstSablonlar.SelectedIndex);
                return;
            }
            try
            {
                var id = _sablonlar[_lstSablonlar.SelectedIndex].Id ?? 0;
                var sablonService = ServiceLocator.Resolve<ISablonService>();
                var ok = await sablonService.DeleteAsync(id).ConfigureAwait(true);
                if (ok)
                    await VerileriYukleAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
