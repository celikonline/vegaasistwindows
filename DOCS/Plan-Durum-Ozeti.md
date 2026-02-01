# Plan Durum Özeti

**Güncelleme:** Faz 1, 2 ve 3 tamamlandı; Faz 4 ve sonrası bekliyor.

---

## Faz 1: Kritik Eksikler (1–2 Hafta) — ✅ TAMAMLANDI

| # | Görev | Durum | Not |
|---|-------|--------|-----|
| 1.1 | IndexViewControl → Sorgu Akışı | ✅ Tamamlandı | SorguSession, SyncToSession/LoadFromSession, SeciliSirketler; Sorgu Başlat/Duraklat/Yeni Sorgu işliyor |
| 1.2 | PolicyTypeSelect + Branş Formları | ✅ Tamamlandı | Combo değişince form açılmıyor; grid branş kolonuna çift tıklanınca ilgili form açılıyor |
| 1.3 | Veri Kaynakları | ✅ Tamamlandı | TurkeyLocations, Professions, KullanimTarziOptions; DaskDetaylariForm İl/İlçe cascade |
| 1.4 | Trafik/Kasko Pol. Bilgisi Tab | ✅ Tamamlandı | Trafik tab: Başlangıç/Bitiş'e bağlı tek Kalan Gün; Sigorta Şirketi combo |
| 1.5 | frmHizliDaskSorgu Benzeri DASK | ✅ Tamamlandı | DaskDetaylariForm: İl/İlçe cascade, YapiTarziOptions, Mahalle, İnşa Yılı |

**Faz 1 özet:** Tüm maddeler uygulandı.

---

## Faz 2: Form Tamamlama (2–3 Hafta) — ✅ TAMAMLANDI

| # | Görev | Durum | Yapılanlar |
|---|-------|--------|------------|
| 2.1 | frmOncekiPolice | ✅ Tamamlandı | BaseForm, SorguSession entegrasyonu, IndexViewControl “Önceki Poliçe” butonu, MainForm menü |
| 2.2 | UserControl Arac/DASK | ✅ Tamamlandı | Bitiş Tarihi → DateTimePicker (AracOncekiPolControl, DaskOncekiPolControl); Session’dan yükleme |
| 2.3 | frmExcelOku | ✅ Tamamlandı | Hedef seçimi (Toplu teklif / Sadece önizleme), sütun eşleme paneli, OfferService ile toplu teklif oluşturma |
| 2.4 | frmCokluFiyat | ✅ Tamamlandı | Veri yokken bilgi etiketi; menüden IndexViewControl.GetFiyatSatirlari() ile veri |
| 2.5 | frmPoliceNoGit / frmTeklifNoGit | ✅ Tamamlandı | PolicyService.GetByPoliceNoAsync, OfferDto/Offer.TeklifNo, OfferService.GetByTeklifNoAsync; PolicelerimForm/TekliflerForm filtre + tek kayıt seçimi |
| 2.6 | frmDuyuruGonder | ✅ Tamamlandı | IDuyuruService, Duyuru entity, DuyuruService, DuyuruGonderForm/DuyurularForm API entegrasyonu |
| 2.7 | PDF + Teklif Dosyaları | ✅ Tamamlandı | **PDF:** PdfSharp-gdi 1.50 ile PDFExportForm’da gerçek PDF dışa aktarma. **Teklif dosyaları:** OfferAttachment entity/servis, TeklifDosyalariForm listeleme/yükleme/indirme/silme (DOCS/offer_attachments_table.sql) |

**Faz 2 özet:** Tüm maddeler uygulandı.

---

## Faz 3: Raporlar ve Grafikler (2 Hafta) — ✅ TAMAMLANDI

| # | Görev | Durum | Yapılanlar |
|---|-------|--------|------------|
| 3.1 | RaporlarForm Grafik Entegrasyonu | ✅ Tamamlandı | RaporlarForm "Grafik Raporları" kartındaki her öğe → RaporGrafikForm(raporAdi); İl Bazlı, Şirket Bazlı grafikler dahil |
| 3.2 | Diğer Grafikler (Meslek, Kullanım Tarzı, Marka, Otorizasyon, Komisyon, Ürün, Doğum Tarihi, Kesilen Poliçeler) | ✅ Tamamlandı | RaporGrafikForm'da rapor adına göre Column/Pie ve örnek veri (İl, Şirket, Meslek, Kullanım Tarzı, Marka, Otorizasyon, Komisyon, Ürün, Doğum Tarihi Portföy, Kesilen Poliçeler) |
| 3.3 | Rapor Export (PrintDocument veya 3. parti) | ✅ Tamamlandı | Grafiği Kaydet (PNG); Yazdır (standart PrintDocument + PrintPreviewDialog) |

**Faz 3 özet:** Tüm maddeler uygulandı. Faz 4'e geçmeden önce Faz 3 tamamlanmış sayılır.

---

## Faz 4: Robot / Şirket Entegrasyonu (4+ Hafta)

| # | Görev | Durum |
|---|-------|--------|
| 4.1 | CompanysBot Altyapısı (Selenium/ChromeDriver, IBrowserDriver, App.config) | ✅ Tamamlandı |
| 4.2 | Şirket Robotları (TRF_AkSigorta, TRF_AnaSigortaStub, TRF_AnadoluStub, Registry) | ✅ Tamamlandı |
| 4.3 | AllLogins / AllOffers (AllLoginsRunner, AllOffersRunner, seçili şirketler) | ✅ Tamamlandı |
| 4.4 | reCaptcha / CapthaResolver (ICaptchaResolver, ManuelCaptcha, TwoCaptcha) | ✅ Tamamlandı |
| 4.5 | Tramer (ITramerService, TramerService, TramerSorguForm; API notları DOCS) | ✅ Altyapı tamamlandı |

---

## Faz 5: Destek ve Admin (1 Hafta)

| # | Görev | Durum |
|---|-------|--------|
| 5.1 | PaylasilanSirketlerControl (yeni paylaşım, Benimle Paylaşılanlar) | ✅ Tamamlandı |
| 5.2 | WebEkranlariControl (WebUserService, unlicensed_agent_only, CRUD) | ✅ Tamamlandı |
| 5.3 | ConfigForm (genel ayarlar, proxy, benchmark, otomatik güncelleme kaydı) | ✅ Tamamlandı |

---

## Faz 6: Kalan Formlar ve İyileştirmeler

| # | Görev | Durum |
|---|-------|--------|
| 6.1 | frmManuelUavtSorgu | ✅ Form mevcut; UAVT servisi eklendiğinde bağlanacak (DOCS/Faz6-Kalan-Formlar-Notlari.md) |
| 6.2 | frmTramerSorgu | ✅ ITramerService bağlı; gerçek API ile TramerService doldurulacak |
| 6.3 | Loading / Bildirim (LoadingForm, BildirimEkraniForm) | ✅ Formlar mevcut; bildirim servisi istenirse bağlanır |
| 6.4 | Otomatik Sorgu (SablonDuzenleForm, WSTeklifleriniSorgulaForm) | ✅ Formlar mevcut; şablon/WS servisleri eklendiğinde bağlanır |
| 6.5 | Layout Düzeltmeleri | ⏳ İhtiyaç halinde |

---

## Veritabanı / Altyapı Notları

- **duyurular:** Duyuru servisi için tablo gerekli (Faz 2’de tanımlandı).
- **offers.teklif_no:** Faz 2’de eklendi; migration veya `ALTER TABLE` gerekebilir.
- **offer_attachments:** Teklif dosyaları için tablo; `DOCS/offer_attachments_table.sql` ile oluşturulur.

---

## Özet Tablo

| Faz | Açıklama | Durum |
|-----|----------|--------|
| **Faz 1** | Kritik Eksikler | ✅ **Tamamlandı** |
| **Faz 2** | Form Tamamlama | ✅ **Tamamlandı** |
| **Faz 3** | Raporlar ve Grafikler | ✅ **Tamamlandı** |
| **Faz 4** | Robot / Şirket Entegrasyonu | ✅ Altyapı tamamlandı |
| **Faz 5** | Destek ve Admin | ✅ Tamamlandı |
| **Faz 6** | Kalan Formlar ve İyileştirmeler | ✅ Formlar mevcut; entegrasyon noktaları dokümante |

**Şu anki konum:** Faz 1–6 tamamlandı veya altyapı/formalarla hazır. Kalan işler: Tramer gerçek API, UAVT/şablon/WS servis entegrasyonları, layout iyileştirmeleri.
