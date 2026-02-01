using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VegaAsis.Core.Contracts;
using VegaAsis.Core.DTOs;
using VegaAsis.Windows.Data;

namespace VegaAsis.Windows.Forms
{
    public class TeklifDetayForm : Form
    {
        private readonly IOfferService _offerService;
        private readonly Guid? _offerId;
        private readonly Guid _userId;
        private readonly OfferDto _existing;
        private TextBox _txtMusteri, _txtSirket, _txtPlaka, _txtTelefon, _txtAciklama;
        private ComboBox _cmbPoliceTipi, _cmbKullanimTarzi, _cmbMarka;
        private CheckBox _chkCalisildi;
        private Button _btnKaydet, _btnIptal;

        public TeklifDetayForm(IOfferService offerService, Guid userId, OfferDto existing = null)
        {
            _offerService = offerService ?? throw new ArgumentNullException(nameof(offerService));
            _userId = userId;
            _offerId = existing?.Id;
            _existing = existing;

            Text = existing == null ? "Yeni Teklif" : "Teklif Düzenle";
            Size = new Size(480, 460);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            var y = 12;
            AddLabel("Müşteri:", 12, y);
            _txtMusteri = AddTextBox(120, y - 2, 320);
            y += 32;
            AddLabel("Şirket:", 12, y);
            _txtSirket = AddTextBox(120, y - 2, 320);
            y += 32;
            AddLabel("Police Tipi:", 12, y);
            _cmbPoliceTipi = new ComboBox
            {
                Left = 120,
                Top = y - 2,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbPoliceTipi.Items.AddRange(new object[] { "TRAFİK", "KASKO", "TSS", "DASK", "KONUT", "İMM" });
            _cmbPoliceTipi.SelectedIndex = 0;
            Controls.Add(_cmbPoliceTipi);
            y += 32;
            AddLabel("Kullanım Tarzı:", 12, y);
            _cmbKullanimTarzi = new ComboBox
            {
                Left = 120,
                Top = y - 2,
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbKullanimTarzi.Items.AddRange(KullanimTarziOptions.List);
            if (_cmbKullanimTarzi.Items.Count > 0) _cmbKullanimTarzi.SelectedIndex = 0;
            Controls.Add(_cmbKullanimTarzi);
            y += 32;
            AddLabel("Araç Markası:", 12, y);
            _cmbMarka = new ComboBox
            {
                Left = 120,
                Top = y - 2,
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbMarka.Items.AddRange(VehicleBrandsAndTypes.GetBrandDisplays());
            if (_cmbMarka.Items.Count > 0) _cmbMarka.SelectedIndex = 0;
            Controls.Add(_cmbMarka);
            y += 32;
            AddLabel("Plaka:", 12, y);
            _txtPlaka = AddTextBox(120, y - 2, 160);
            y += 32;
            AddLabel("Telefon:", 12, y);
            _txtTelefon = AddTextBox(120, y - 2, 200);
            y += 32;
            AddLabel("Açıklama:", 12, y);
            _txtAciklama = AddTextBox(120, y - 2, 320);
            y += 32;
            _chkCalisildi = new CheckBox { Text = "Çalışıldı", Left = 120, Top = y, Width = 120 };
            Controls.Add(_chkCalisildi);
            y += 36;

            _btnKaydet = new Button { Text = "Kaydet", Left = 120, Top = y, Width = 100, Height = 28 };
            _btnIptal = new Button { Text = "İptal", Left = 228, Top = y, Width = 100, Height = 28 };
            _btnKaydet.Click += BtnKaydet_Click;
            _btnIptal.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(_btnKaydet);
            Controls.Add(_btnIptal);

            if (existing != null)
            {
                _txtMusteri.Text = existing.Musteri;
                _txtSirket.Text = existing.Sirket;
                _txtPlaka.Text = existing.Plaka;
                _txtTelefon.Text = existing.Telefon;
                _txtAciklama.Text = existing.Aciklama;
                _chkCalisildi.Checked = existing.Calisildi;
                var idx = _cmbPoliceTipi.Items.IndexOf(existing.PoliceTipi);
                if (idx >= 0) _cmbPoliceTipi.SelectedIndex = idx;
                if (!string.IsNullOrWhiteSpace(existing.KullanimTarzi))
                {
                    var ktIdx = _cmbKullanimTarzi.Items.IndexOf(existing.KullanimTarzi);
                    if (ktIdx >= 0) _cmbKullanimTarzi.SelectedIndex = ktIdx;
                }
                if (!string.IsNullOrWhiteSpace(existing.AracMarkasi))
                {
                    var mIdx = _cmbMarka.Items.IndexOf(existing.AracMarkasi);
                    if (mIdx >= 0) _cmbMarka.SelectedIndex = mIdx;
                }
            }
        }

        private Label AddLabel(string text, int x, int y)
        {
            var lbl = new Label { Text = text, AutoSize = true, Left = x, Top = y };
            Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            var txt = new TextBox { Left = x, Top = y, Width = width };
            Controls.Add(txt);
            return txt;
        }

        private async void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtMusteri.Text))
            {
                MessageBox.Show("Müşteri adı girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _btnKaydet.Enabled = false;
                OfferDto dto;
                var kullanimTarziVal = _cmbKullanimTarzi?.SelectedItem?.ToString()?.Trim();
                if (string.IsNullOrEmpty(kullanimTarziVal) || string.Equals(kullanimTarziVal, "KULLANIM TARZI SEÇİNİZ", StringComparison.OrdinalIgnoreCase))
                    kullanimTarziVal = null;
                var aracMarkasiVal = _cmbMarka?.SelectedItem?.ToString()?.Trim();

                if (_offerId.HasValue && _existing != null)
                {
                    dto = new OfferDto
                    {
                        Id = _existing.Id,
                        UserId = _existing.UserId,
                        Sirket = _txtSirket.Text?.Trim(),
                        Personel = _existing.Personel,
                        PoliceTipi = _cmbPoliceTipi.SelectedItem?.ToString() ?? "TRAFİK",
                        Trf = _existing.Trf,
                        Ksk = _existing.Ksk,
                        Tss = _existing.Tss,
                        Dsk = _existing.Dsk,
                        Knt = _existing.Knt,
                        Imm = _existing.Imm,
                        Musteri = _txtMusteri.Text.Trim(),
                        TcVergi = _existing.TcVergi,
                        DogumTarihi = _existing.DogumTarihi,
                        Plaka = _txtPlaka.Text?.Trim(),
                        BelgeSeri = _existing.BelgeSeri,
                        SonIslem = _existing.SonIslem,
                        KayitSekli = _existing.KayitSekli,
                        TaliDisAcente = _existing.TaliDisAcente,
                        Telefon = _txtTelefon.Text?.Trim(),
                        Calisildi = _chkCalisildi.Checked,
                        Aciklama = _txtAciklama.Text?.Trim(),
                        Policelestirme = _existing.Policelestirme,
                        AcentemGonderildi = _existing.AcentemGonderildi,
                        AcentemWebeGonderildi = _existing.AcentemWebeGonderildi,
                        Meslek = _existing.Meslek,
                        KullanimTarzi = kullanimTarziVal,
                        AracMarkasi = string.IsNullOrWhiteSpace(aracMarkasiVal) ? null : aracMarkasiVal,
                        ArsivDonemi = _existing.ArsivDonemi,
                        CreatedAt = _existing.CreatedAt,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _offerService.UpdateAsync(_offerId.Value, dto).ConfigureAwait(true);
                }
                else
                {
                    dto = new OfferDto
                    {
                        Musteri = _txtMusteri.Text.Trim(),
                        Sirket = _txtSirket.Text?.Trim(),
                        PoliceTipi = _cmbPoliceTipi.SelectedItem?.ToString() ?? "TRAFİK",
                        Plaka = _txtPlaka.Text?.Trim(),
                        Telefon = _txtTelefon.Text?.Trim(),
                        Aciklama = _txtAciklama.Text?.Trim(),
                        Calisildi = _chkCalisildi.Checked,
                        KullanimTarzi = kullanimTarziVal,
                        AracMarkasi = string.IsNullOrWhiteSpace(aracMarkasiVal) ? null : aracMarkasiVal
                    };
                    await _offerService.CreateAsync(dto, _userId).ConfigureAwait(true);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydetme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                _btnKaydet.Enabled = true;
            }
        }
    }
}
