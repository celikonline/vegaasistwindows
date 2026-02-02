using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Windows;
using ClosedXML.Excel;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;

namespace VegaAsis.Windows.Forms
{
    public class PolicelerimForm : Form
    {
        private readonly IPolicyService _policyService;
        private DataGridView _grid;
        private Button _btnYenile, _btnYeni, _btnSeclileriSil, _btnExcelAktar;
        private TextBox _txtArama;
        private ComboBox _cmbPersonel, _cmbPoliceTuru, _cmbKayitTipi, _cmbDovizTipi, _cmbLimit;
        private CheckBox _chkTarihFiltre;
        private DateTimePicker _dtpBaslangic, _dtpBitis;
        private List<PolicyDto> _allPolicies = new List<PolicyDto>();
        private readonly string _initialPoliceNo;

        public PolicelerimForm(IPolicyService policyService, string initialPoliceNo = null)
        {
            _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
            _initialPoliceNo = initialPoliceNo;
            Text = "Policelerim";
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
            _grid.CellDoubleClick += (s, e) => DuzenlePolicy();

            var filterPanel = BuildFilterPanel();
            var toolPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 36, Padding = new Padding(4) };
            _btnYenile = new Button { Text = "Yenile", Width = 80, Height = 28 };
            _btnYeni = new Button { Text = "Yeni Poliçe", Width = 90, Height = 28 };
            _btnSeclileriSil = new Button { Text = "Seçilenleri Sil", Width = 110, Height = 28 };
            _btnExcelAktar = new Button { Text = "Excel Aktar", Width = 100, Height = 28 };

            _btnYenile.Click += (s, e) => _ = LoadPoliciesAsync();
            _btnYeni.Click += (s, e) => YeniPolicy();
            _btnSeclileriSil.Click += (s, e) => _ = SilSeclileriAsync();
            _btnExcelAktar.Click += (s, e) => ExcelAktar();

            toolPanel.Controls.Add(_btnYenile);
            toolPanel.Controls.Add(_btnYeni);
            toolPanel.Controls.Add(_btnSeclileriSil);
            toolPanel.Controls.Add(_btnExcelAktar);

            var panel = new Panel { Dock = DockStyle.Fill };
            panel.Controls.Add(_grid);
            panel.Controls.Add(toolPanel);
            panel.Controls.Add(filterPanel);
            Controls.Add(panel);

            Load += (s, e) => _ = LoadPoliciesAsync();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!string.IsNullOrEmpty(_initialPoliceNo) && _txtArama != null)
            {
                _txtArama.Text = _initialPoliceNo;
                ApplyFilter();
            }
        }

        private Panel BuildFilterPanel()
        {
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 38,
                Padding = new Padding(6, 6, 6, 4),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            flow.Controls.Add(new Label { Text = "Ara:", AutoSize = true, Margin = new Padding(0, 6, 2, 0) });
            _txtArama = new TextBox { Width = 100, Height = 22 };
            _txtArama.TextChanged += (s, e) => ApplyFilter();
            flow.Controls.Add(_txtArama);
            flow.Controls.Add(new Label { Text = "Personel:", AutoSize = true, Margin = new Padding(12, 6, 2, 0) });
            _cmbPersonel = CreateCombo(ref flow, 85);
            _cmbPersonel.SelectedIndexChanged += (s, e) => ApplyFilter();
            flow.Controls.Add(new Label { Text = "Poliçe Türü:", AutoSize = true, Margin = new Padding(8, 6, 2, 0) });
            _cmbPoliceTuru = CreateCombo(ref flow, 85);
            _cmbPoliceTuru.SelectedIndexChanged += (s, e) => ApplyFilter();
            flow.Controls.Add(new Label { Text = "Kayıt:", AutoSize = true, Margin = new Padding(8, 6, 2, 0) });
            _cmbKayitTipi = CreateCombo(ref flow, 80);
            _cmbKayitTipi.Items.AddRange(new object[] { "TÜMÜ", "Manuel", "Otomatik", "API" });
            _cmbKayitTipi.SelectedIndex = 0;
            _cmbKayitTipi.SelectedIndexChanged += (s, e) => ApplyFilter();
            flow.Controls.Add(new Label { Text = "Döviz:", AutoSize = true, Margin = new Padding(8, 6, 2, 0) });
            _cmbDovizTipi = CreateCombo(ref flow, 55);
            _cmbDovizTipi.Items.AddRange(new object[] { "TÜMÜ", "TL", "USD", "EUR" });
            _cmbDovizTipi.SelectedIndex = 0;
            _cmbDovizTipi.SelectedIndexChanged += (s, e) => ApplyFilter();
            var btnAra = new Button { Text = "Ara", Width = 50, Height = 24 };
            btnAra.Click += (s, e) => ApplyFilter();
            flow.Controls.Add(btnAra);
            _chkTarihFiltre = new CheckBox { Text = "Tarih Arası", AutoSize = true, Margin = new Padding(12, 6, 0, 0) };
            _chkTarihFiltre.CheckedChanged += (s, e) => { _dtpBaslangic.Enabled = _dtpBitis.Enabled = _chkTarihFiltre.Checked; ApplyFilter(); };
            flow.Controls.Add(_chkTarihFiltre);
            _dtpBaslangic = new DateTimePicker { Width = 95, Format = DateTimePickerFormat.Short, Enabled = false };
            _dtpBaslangic.ValueChanged += (s, e) => ApplyFilter();
            flow.Controls.Add(_dtpBaslangic);
            _dtpBitis = new DateTimePicker { Width = 95, Format = DateTimePickerFormat.Short, Enabled = false };
            _dtpBitis.ValueChanged += (s, e) => ApplyFilter();
            flow.Controls.Add(_dtpBitis);
            flow.Controls.Add(new Label { Text = "Limit:", AutoSize = true, Margin = new Padding(12, 6, 2, 0) });
            _cmbLimit = CreateCombo(ref flow, 70);
            _cmbLimit.Items.AddRange(new object[] { "50", "100", "200", "500" });
            _cmbLimit.SelectedIndex = 0;
            _cmbLimit.SelectedIndexChanged += (s, e) => ApplyFilter();
            return flow;
        }

        private ComboBox CreateCombo(ref FlowLayoutPanel flow, int width)
        {
            var c = new ComboBox { Width = width, DropDownStyle = ComboBoxStyle.DropDownList };
            c.Items.Add("TÜMÜ");
            c.SelectedIndex = 0;
            flow.Controls.Add(c);
            return c;
        }

        private void ApplyFilter()
        {
            var filtered = GetFilteredPolicies();
            _grid.DataSource = null;
            _grid.Columns.Clear();
            if (filtered.Count > 0)
            {
                _grid.DataSource = filtered;
            }
        }

        private List<PolicyDto> GetFilteredPolicies()
        {
            var arama = _txtArama?.Text?.Trim().ToUpperInvariant() ?? "";
            var personel = _cmbPersonel?.SelectedItem?.ToString() ?? "TÜMÜ";
            var policeTuru = _cmbPoliceTuru?.SelectedItem?.ToString() ?? "TÜMÜ";
            var kayitTipi = _cmbKayitTipi?.SelectedItem?.ToString() ?? "TÜMÜ";
            var doviz = _cmbDovizTipi?.SelectedItem?.ToString() ?? "TÜMÜ";
            var limit = int.TryParse(_cmbLimit?.SelectedItem?.ToString(), out var l) ? l : 50;

            var q = _allPolicies.AsEnumerable();
            if (!string.IsNullOrEmpty(arama))
            {
                q = q.Where(p =>
                    (p.PoliceNo?.ToUpperInvariant().Contains(arama) ?? false) ||
                    (p.Musteri?.ToUpperInvariant().Contains(arama) ?? false) ||
                    (p.Plaka?.ToUpperInvariant().Contains(arama) ?? false));
            }
            if (personel != "TÜMÜ") q = q.Where(p => p.Personel == personel);
            if (policeTuru != "TÜMÜ") q = q.Where(p => p.PoliceTuru == policeTuru);
            if (kayitTipi != "TÜMÜ") q = q.Where(p => p.KayitTipi == kayitTipi);
            if (doviz != "TÜMÜ") q = q.Where(p => p.DovizTipi == doviz);
            if (_chkTarihFiltre?.Checked == true && _dtpBaslangic != null && _dtpBitis != null)
            {
                var bas = _dtpBaslangic.Value.Date;
                var bit = _dtpBitis.Value.Date;
                q = q.Where(p => p.BaslangicTarihi.HasValue && p.BaslangicTarihi.Value.Date >= bas && p.BaslangicTarihi.Value.Date <= bit);
            }
            return q.Take(limit).ToList();
        }

        private void DuzenlePolicy()
        {
            var dto = GetSelectedPolicy();
            if (dto == null)
            {
                MessageBox.Show("Düzenlemek için bir poliçe seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var detay = new PoliceDetayForm(dto))
            {
                if (detay.ShowDialog(this) == DialogResult.OK && detay.EditedPolicy != null)
                {
                    _ = KaydetVeYenileAsync(detay.EditedPolicy);
                }
            }
        }

        private void YeniPolicy()
        {
            if (!ServiceLocator.IsInitialized)
            {
                MessageBox.Show("Servisler başlatılamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var auth = ServiceLocator.Resolve<IAuthService>();
            var userId = auth?.GetCurrentUserId;
            if (!userId.HasValue)
            {
                MessageBox.Show("Giriş yapmalısınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var yeni = new PolicyDto
            {
                Id = Guid.Empty,
                UserId = userId.Value,
                PoliceTuru = "TRAFİK",
                DovizTipi = "TL",
                KayitTipi = "Manuel",
                KayitTarihi = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            using (var detay = new PoliceDetayForm(yeni))
            {
                if (detay.ShowDialog(this) == DialogResult.OK && detay.EditedPolicy != null)
                {
                    _ = KaydetYeniAsync(detay.EditedPolicy);
                }
            }
        }

        private async Task KaydetYeniAsync(PolicyDto dto)
        {
            try
            {
                await _policyService.CreateAsync(dto).ConfigureAwait(true);
                MessageBox.Show("Yeni poliçe oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPoliciesAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydetme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task KaydetVeYenileAsync(PolicyDto dto)
        {
            try
            {
                await _policyService.UpdateAsync(dto).ConfigureAwait(true);
                MessageBox.Show("Poliçe güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPoliciesAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private PolicyDto GetSelectedPolicy()
        {
            if (_grid.CurrentRow?.DataBoundItem is PolicyDto dto) return dto;
            return null;
        }

        private async Task LoadPoliciesAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var list = await _policyService.GetAllAsync(null).ConfigureAwait(true);
                _allPolicies = list != null ? list.ToList() : new List<PolicyDto>();
                PopulateFilterCombos();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Policeler yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void PopulateFilterCombos()
        {
            var personeller = _allPolicies.Select(p => p.Personel).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToList();
            var policeTurleri = _allPolicies.Select(p => p.PoliceTuru).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToList();
            _cmbPersonel.Items.Clear();
            _cmbPersonel.Items.Add("TÜMÜ");
            foreach (var pr in personeller) _cmbPersonel.Items.Add(pr);
            _cmbPersonel.SelectedIndex = 0;
            _cmbPoliceTuru.Items.Clear();
            _cmbPoliceTuru.Items.Add("TÜMÜ");
            foreach (var pt in policeTurleri) _cmbPoliceTuru.Items.Add(pt);
            _cmbPoliceTuru.SelectedIndex = 0;
        }

        private async Task SilSeclileriAsync()
        {
            var selected = GetSelectedPolicies().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir poliçe seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(selected.Count + " poliçeyi silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                Cursor = Cursors.WaitCursor;
                var count = await _policyService.DeleteMultipleAsync(selected.Select(p => p.Id)).ConfigureAwait(true);
                MessageBox.Show(count + " poliçe silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPoliciesAsync().ConfigureAwait(true);
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

        private IEnumerable<PolicyDto> GetSelectedPolicies()
        {
            foreach (DataGridViewRow row in _grid.SelectedRows)
            {
                if (row.DataBoundItem is PolicyDto dto) yield return dto;
            }
        }

        private void ExcelAktar()
        {
            var data = _grid.SelectedRows.Count > 0 ? GetSelectedPolicies().ToList() : GetFilteredPolicies();
            if (data.Count == 0)
            {
                MessageBox.Show("Dışa aktarılacak poliçe bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Poliçeler");
                    var headers = new[] { "Personel", "Şirket", "Poliçe No", "Zeyil No", "Yenileme No", "Başlangıç", "Bitiş", "Prim", "Döviz", "Poliçe Türü", "Kayıt Tarihi", "Müşteri", "Plaka", "Açıklama" };
                    for (int i = 0; i < headers.Length; i++) ws.Cell(1, i + 1).Value = headers[i];
                    int row = 2;
                    var ci = new CultureInfo("tr-TR");
                    foreach (var p in data)
                    {
                        ws.Cell(row, 1).Value = p.Personel ?? "-";
                        ws.Cell(row, 2).Value = p.Sirket ?? "-";
                        ws.Cell(row, 3).Value = p.PoliceNo ?? "-";
                        ws.Cell(row, 4).Value = p.ZeyilNo ?? "-";
                        ws.Cell(row, 5).Value = p.YenilemeNo ?? "-";
                        ws.Cell(row, 6).Value = p.BaslangicTarihi?.ToString("dd.MM.yyyy", ci) ?? "-";
                        ws.Cell(row, 7).Value = p.BitisTarihi?.ToString("dd.MM.yyyy", ci) ?? "-";
                        ws.Cell(row, 8).Value = p.Prim.HasValue ? p.Prim.Value.ToString("N2", ci) : "-";
                        ws.Cell(row, 9).Value = p.DovizTipi ?? "TL";
                        ws.Cell(row, 10).Value = p.PoliceTuru ?? "-";
                        ws.Cell(row, 11).Value = p.KayitTarihi.ToString("dd.MM.yyyy", ci);
                        ws.Cell(row, 12).Value = p.Musteri ?? "-";
                        ws.Cell(row, 13).Value = p.Plaka ?? "-";
                        ws.Cell(row, 14).Value = p.Aciklama ?? "-";
                        row++;
                    }
                    ws.Columns().AdjustToContents();
                    wb.SaveAs("Policeler_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".xlsx");
                }
                MessageBox.Show(data.Count + " poliçe Excel'e aktarıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel aktarma hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
