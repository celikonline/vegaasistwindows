using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.Forms
{
    public class TekliflerForm : Form
    {
        private readonly IAuthService _authService;
        private readonly IOfferService _offerService;
        private DataGridView _grid;
        private Button _btnYenile, _btnYeni, _btnDuzenle, _btnSil, _btnSeclileriSil, _btnExcelAktar;
        private ComboBox _cmbPersonel, _cmbPoliceTipi, _cmbKaynak, _cmbArsiv;
        private TextBox _txtArama;
        private List<OfferDto> _allOffers = new List<OfferDto>();
        private readonly string _initialTeklifNo;

        private static readonly string[] PersonelListesi = { "TÜMÜ", "BATUHAN", "DENEME", "DİLBER", "ELİF", "ENES", "FEYYAZ KAHYA", "GÜLTUNAY", "METİN", "MUHTEREM", "SONER", "YASEMİN", "İLKNUR" };
        private static readonly string[] KayitKaynaklari = { "TÜMÜ", "Manuel", "Open Acentem", "Web HT", "Web Servis" };
        private static readonly string[] ArsivDonemleri = { "Aktif Dönem (2022 ve Sonrası)", "Arşiv_2021H1", "Arşiv_2020", "Arşiv_2019" };
        private static readonly string[] PoliceTipleri = { "TÜMÜ", "TRAFİK", "KASKO", "TSS", "DASK" };

        public TekliflerForm(IAuthService authService, IOfferService offerService, string initialTeklifNo = null)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _offerService = offerService ?? throw new ArgumentNullException(nameof(offerService));
            _initialTeklifNo = initialTeklifNo;
            Text = "Teklifler";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true
            };
            _grid.CellDoubleClick += (s, e) => DuzenleTeklif();

            var filterPanel = BuildFilterPanel();
            var toolPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(4) };
            _btnYeni = new Button { Text = "Yeni", Width = 80, Height = 28 };
            _btnDuzenle = new Button { Text = "Düzenle", Width = 80, Height = 28 };
            _btnSil = new Button { Text = "Sil", Width = 80, Height = 28 };
            _btnSeclileriSil = new Button { Text = "Seçilenleri Sil", Width = 100, Height = 28 };
            _btnExcelAktar = new Button { Text = "Excel Aktar", Width = 100, Height = 28 };
            _btnYenile = new Button { Text = "Yenile", Width = 80, Height = 28 };

            _btnYeni.Click += (s, e) => YeniTeklif();
            _btnDuzenle.Click += (s, e) => DuzenleTeklif();
            _btnSil.Click += (s, e) => _ = SilTeklifAsync();
            _btnSeclileriSil.Click += (s, e) => _ = SilSeclileriAsync();
            _btnExcelAktar.Click += (s, e) => ExcelAktar();
            _btnYenile.Click += (s, e) => _ = LoadOffersAsync();

            toolPanel.Controls.Add(_btnYeni);
            toolPanel.Controls.Add(_btnDuzenle);
            toolPanel.Controls.Add(_btnSil);
            toolPanel.Controls.Add(_btnSeclileriSil);
            toolPanel.Controls.Add(_btnExcelAktar);
            toolPanel.Controls.Add(_btnYenile);

            var panel = new Panel { Dock = DockStyle.Fill };
            panel.Controls.Add(_grid);
            panel.Controls.Add(toolPanel);
            panel.Controls.Add(filterPanel);

            Controls.Add(panel);

            Load += (s, e) => _ = LoadOffersAsync();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!string.IsNullOrEmpty(_initialTeklifNo) && _txtArama != null)
            {
                _txtArama.Text = _initialTeklifNo;
                ApplyFilter();
            }
        }

        private Panel BuildFilterPanel()
        {
            var p = new Panel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(6, 4, 6, 4) };
            int x = 8;
            _cmbPersonel = CreateFilterCombo(PersonelListesi, ref x, 100);
            _cmbPersonel.SelectedIndexChanged += (s, e) => ApplyFilter();
            _cmbPoliceTipi = CreateFilterCombo(PoliceTipleri, ref x, 90);
            _cmbPoliceTipi.SelectedIndexChanged += (s, e) => ApplyFilter();
            _cmbKaynak = CreateFilterCombo(KayitKaynaklari, ref x, 110);
            _cmbKaynak.SelectedIndexChanged += (s, e) => ApplyFilter();
            x += 8;
            _txtArama = new TextBox { Left = x, Top = 6, Width = 140 };
            _txtArama.TextChanged += (s, e) => ApplyFilter();
            x += 148;
            _cmbArsiv = CreateFilterCombo(ArsivDonemleri, ref x, 200);
            _cmbArsiv.SelectedIndexChanged += (s, e) => ApplyFilter();
            p.Controls.AddRange(new Control[] { _cmbPersonel, _cmbPoliceTipi, _cmbKaynak, _txtArama, _cmbArsiv });
            return p;
        }

        private ComboBox CreateFilterCombo(string[] items, ref int x, int width)
        {
            var c = new ComboBox { Left = x, Top = 6, Width = width, DropDownStyle = ComboBoxStyle.DropDownList };
            c.Items.AddRange(items.Cast<object>().ToArray());
            c.SelectedIndex = 0;
            x += width + 6;
            return c;
        }

        private void ApplyFilter()
        {
            var filtered = GetFilteredOffers();
            _grid.DataSource = null;
            _grid.Columns.Clear();
            if (filtered.Count > 0)
            {
                _grid.DataSource = filtered;
            }
        }

        private List<OfferDto> GetFilteredOffers()
        {
            var arama = _txtArama?.Text?.Trim().ToUpperInvariant() ?? "";
            var personel = _cmbPersonel?.SelectedItem?.ToString() ?? "TÜMÜ";
            var kaynak = _cmbKaynak?.SelectedItem?.ToString() ?? "TÜMÜ";
            var police = _cmbPoliceTipi?.SelectedItem?.ToString() ?? "TÜMÜ";
            var arsiv = _cmbArsiv?.SelectedItem?.ToString() ?? ArsivDonemleri[0];

            return _allOffers.Where(o =>
            {
                var matchArama = string.IsNullOrEmpty(arama) ||
                    (o.Musteri?.ToUpperInvariant().Contains(arama) ?? false) ||
                    (o.Plaka?.ToUpperInvariant().Contains(arama) ?? false) ||
                    (o.TcVergi?.Contains(arama) ?? false) ||
                    o.Id.ToString().ToUpperInvariant().Contains(arama);
                var matchPersonel = personel == "TÜMÜ" || o.Personel == personel;
                var matchKaynak = kaynak == "TÜMÜ" || o.KayitSekli == kaynak;
                var matchPolice = police == "TÜMÜ" || o.PoliceTipi == police;
                var matchArsiv = o.ArsivDonemi == arsiv;
                return matchArama && matchPersonel && matchKaynak && matchPolice && matchArsiv;
            }).ToList();
        }

        private void YeniTeklif()
        {
            var userId = _authService.GetCurrentUserId ?? Guid.Empty;
            if (userId == Guid.Empty)
            {
                MessageBox.Show("Giriş yapmanız gerekiyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var detay = new TeklifDetayForm(_offerService, userId, null))
            {
                if (detay.ShowDialog(this) == DialogResult.OK)
                {
                    _ = LoadOffersAsync();
                }
            }
        }

        private void DuzenleTeklif()
        {
            var dto = GetSelectedOffer();
            if (dto == null)
            {
                MessageBox.Show("Düzenlemek için bir teklif seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var userId = _authService.GetCurrentUserId ?? dto.UserId;
            using (var detay = new TeklifDetayForm(_offerService, userId, dto))
            {
                if (detay.ShowDialog(this) == DialogResult.OK)
                {
                    _ = LoadOffersAsync();
                }
            }
        }

        private async Task SilTeklifAsync()
        {
            var dto = GetSelectedOffer();
            if (dto == null)
            {
                MessageBox.Show("Silmek için bir teklif seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Bu teklifi silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                var ok = await _offerService.DeleteAsync(dto.Id).ConfigureAwait(true);
                if (ok)
                {
                    MessageBox.Show("Teklif silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadOffersAsync().ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private OfferDto GetSelectedOffer()
        {
            if (_grid.CurrentRow?.DataBoundItem == null)
            {
                return null;
            }
            return _grid.CurrentRow.DataBoundItem as OfferDto;
        }

        private async Task LoadOffersAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var userId = _authService.GetCurrentUserId;
                var list = await _offerService.GetAllAsync(userId).ConfigureAwait(true);
                _allOffers = list != null ? list.ToList() : new List<OfferDto>();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Teklifler yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task SilSeclileriAsync()
        {
            var selected = GetSelectedOffers().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir teklif seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show($"{selected.Count} teklifi silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                Cursor = Cursors.WaitCursor;
                var ids = selected.Select(o => o.Id).ToList();
                await _offerService.DeleteMultipleAsync(ids).ConfigureAwait(true);
                MessageBox.Show(selected.Count + " teklif silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadOffersAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private IEnumerable<OfferDto> GetSelectedOffers()
        {
            foreach (DataGridViewRow row in _grid.SelectedRows)
            {
                if (row.DataBoundItem is OfferDto dto)
                {
                    yield return dto;
                }
            }
        }

        private void ExcelAktar()
        {
            var dataToExport = _grid.SelectedRows.Count > 0 ? GetSelectedOffers().ToList() : GetFilteredOffers();
            if (dataToExport.Count == 0)
            {
                MessageBox.Show("Dışa aktarılacak teklif bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Teklifler");
                    ws.Cell(1, 1).Value = "Personel";
                    ws.Cell(1, 2).Value = "Poliçe Tipi";
                    ws.Cell(1, 3).Value = "Müşteri";
                    ws.Cell(1, 4).Value = "TC / Vergi No";
                    ws.Cell(1, 5).Value = "Plaka";
                    ws.Cell(1, 6).Value = "Belge Seri";
                    ws.Cell(1, 7).Value = "Son İşlem";
                    ws.Cell(1, 8).Value = "Kayıt Şekli";
                    ws.Cell(1, 9).Value = "Telefon";
                    ws.Cell(1, 10).Value = "Çalışıldı";
                    ws.Cell(1, 11).Value = "Açıklama";
                    int row = 2;
                    foreach (var o in dataToExport)
                    {
                        ws.Cell(row, 1).Value = o.Personel ?? "-";
                        ws.Cell(row, 2).Value = o.PoliceTipi ?? "-";
                        ws.Cell(row, 3).Value = o.Musteri ?? "-";
                        ws.Cell(row, 4).Value = o.TcVergi ?? "-";
                        ws.Cell(row, 5).Value = o.Plaka ?? "-";
                        ws.Cell(row, 6).Value = o.BelgeSeri ?? "-";
                        ws.Cell(row, 7).Value = o.SonIslem.HasValue ? o.SonIslem.Value.ToString("dd.MM.yyyy HH:mm", new CultureInfo("tr-TR")) : "-";
                        ws.Cell(row, 8).Value = o.KayitSekli ?? "-";
                        ws.Cell(row, 9).Value = o.Telefon ?? "-";
                        ws.Cell(row, 10).Value = o.Calisildi ? "Evet" : "Hayır";
                        ws.Cell(row, 11).Value = o.Aciklama ?? "-";
                        row++;
                    }
                    ws.Columns().AdjustToContents();
                    var fileName = "Teklifler_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".xlsx";
                    wb.SaveAs(fileName);
                }
                MessageBox.Show(dataToExport.Count + " teklif Excel'e aktarıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel aktarma hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
