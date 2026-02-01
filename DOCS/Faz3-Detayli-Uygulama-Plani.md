# Faz 3 – Detaylı Uygulama Planı

**Süre:** 2 hafta  
**Kaynak:** Open-Hizli-Teklif-VegaAsis-Karsilastirma-Plani.md, Bölüm 6 – Faz 3

---

## Genel Bakış

Faz 3’ün amacı: Raporlar ekranındaki (RaporlarForm) grafik raporlarını Open Hızlı Teklif’teki UserControl’lere karşılık gelecek şekilde tek bir grafik formunda toplamak, tüm grafik tiplerine örnek/gerçek veri ve uygun grafik türünü (sütun, pasta vb.) vermek, rapor dışa aktarma ve yazdırmayı standart .NET ile sağlamak.

---

## Mevcut Durum (Uygulanmış)

| Bileşen | Durum | Açıklama |
|---------|--------|----------|
| RaporlarForm | ✅ | Üretim, Poliçe, Finansal, Analiz, **Grafik Raporları** kartları; Grafik Raporları’ndaki her link → RaporGrafikForm(raporAdi) |
| RaporGrafikForm | ✅ | Rapor adına göre Column/Pie ve örnek veri; Grafiği Kaydet (PNG), Yazdır (PrintDocument + PrintPreviewDialog) |
| Grafik tipleri | ✅ | İl Bazlı, Şirket Bazlı, Meslek, Kullanım Tarzı, Marka, Otorizasyon, Komisyon, Ürün, Doğum Tarihi Portföy, Kesilen Poliçeler |

---

## Görev 3.1: RaporlarForm Grafik Entegrasyonu

### Amaç

Open Hızlı Teklif’teki UserControlCityGrafik, UserControlSirketBazliGrafik benzeri grafiklerin VegaAsis’te tek bir form (RaporGrafikForm) ile karşılanması; RaporlarForm’dan her grafik raporuna tıklanınca ilgili grafiğin açılması.

### Yapılacaklar (Tamamlanan / Opsiyonel)

| Adım | Açıklama | Dosya | Durum | Tahmini |
|------|----------|-------|--------|---------|
| 3.1.1 | **Grafik Raporları kartı**: RaporlarForm’da “Grafik Raporları” kartı; linkler GrafikItems dizisi ile tanımlı | RaporlarForm.cs | ✅ | - |
| 3.1.2 | **Tıklama → RaporGrafikForm**: GrafikItems içindeki rapor adına tıklanınca RaporGrafikForm(raporAdi) açılsın | RaporlarForm.cs, AcRaporDetay() | ✅ | - |
| 3.1.3 | **İl Bazlı / Şirket Bazlı**: RaporGrafikForm’da rapor adı “İl Bazlı” veya “Şirket Bazlı” içeriyorsa ilgili Column serisi ve örnek veri | RaporGrafikForm.cs, OlusturGrafik() | ✅ | - |
| 3.1.4 | **(Opsiyonel) Ayrı UserControl’ler**: İleride İl/Şirket grafiklerini RaporlarForm içine gömülü UserControl olarak eklemek istenirse, RaporGrafikForm’daki mantık ayrı UserControl’lere taşınabilir | - | ⏳ | 4 saat |

### Grafik Raporları Listesi (RaporlarForm – GrafikItems)

- İl Bazlı Grafik  
- Şirket Bazlı Grafik  
- Mesleklere Göre Teklif Grafiği  
- Teklif Kullanım Tarzı Grafiği  
- Teklif Marka Grafiği  
- Teklif Otorizasyon Oranları  
- Teklif Komisyon Kazancı  
- Ürün Grafiği  
- Doğum Tarihi Portföy  
- Kesilen Poliçeler  

---

## Görev 3.2: Diğer Grafikler (Meslek, Kullanım Tarzı, Marka, vb.)

### Amaç

Open Hızlı Teklif’teki ayrı UserControl’ler (UserControlMesleklereGoreTeklifGrafigi, UserControlTeklifKullanimTarziGrafigi, UserControlTeklifMarkaGrafigi, UserControlTeklifOtorizasyonOranlari, UserControlTeklifKomisyonKazanci, UserControlUrunGrafigi, UserControlDogumTarihiPortfoy, UserControlKesilenPoliceler) VegaAsis’te RaporGrafikForm içinde rapor adına göre tek formda karşılanıyor.

### Yapılacaklar (Tamamlanan / Opsiyonel)

| Adım | Açıklama | Dosya | Durum | Tahmini |
|------|----------|-------|--------|---------|
| 3.2.1 | **Rapor adı → grafik tipi ve veri**: OlusturGrafik() içinde rapor adına göre SeriesChartType (Column/Pie) ve Points (örnek veri) atanması | RaporGrafikForm.cs | ✅ | - |
| 3.2.2 | **Meslek / Kullanım Tarzı / Ürün**: Pasta grafik; örnek kategoriler ve yüzdeler | RaporGrafikForm.cs | ✅ | - |
| 3.2.3 | **Marka / Otorizasyon / Komisyon / Doğum Tarihi / Kesilen Poliçeler**: Sütun grafik; örnek kategoriler ve değerler | RaporGrafikForm.cs | ✅ | - |
| 3.2.4 | **(Opsiyonel) Gerçek veri**: IOfferService, IPolicyService veya yeni IReportService ile teklif/poliçe verilerinden grafik verisi üretmek; tarih aralığı filtresi | RaporGrafikForm.cs, yeni servis/DTO | ⏳ | 1–2 gün |

### Rapor Adı – Grafik Tipi – Veri Eşlemesi (Mevcut)

| Rapor adı | Grafik tipi | Örnek veri kaynağı (şu an) |
|-----------|--------------|----------------------------|
| İl Bazlı Grafik | Column | Sabit il listesi + sayılar |
| Şirket Bazlı Grafik | Column | Sabit şirket listesi + sayılar |
| Mesleklere Göre Teklif Grafiği | Pie | Serbest Meslek, Memur, Emekli, Öğrenci, Diğer |
| Teklif Kullanım Tarzı Grafiği | Pie | Özel/Ticari/Resmi Kullanım, Diğer |
| Teklif Marka Grafiği | Column | Fiat, Renault, VW, Ford, Toyota, Hyundai |
| Teklif Otorizasyon Oranları | Column | Aylık (Ocak–Haziran) % |
| Teklif Komisyon Kazancı | Column | Trafik, Kasko, DASK, TSS, İMM (TL) |
| Ürün Grafiği | Pie | Trafik, Kasko, DASK, TSS, İMM % |
| Doğum Tarihi Portföy | Column | Yaş grupları (18–25, 26–35, …) |
| Kesilen Poliçeler | Column | Aylık adet (Ocak–Haziran) |

---

## Görev 3.3: Rapor Export (Yazdırma ve Dışa Aktarma)

### Amaç

DevExpress XtraPrinting yerine standart PrintDocument veya 3. parti kütüphane kullanmak; grafik için PNG dışa aktarma ve yazdırma.

### Yapılacaklar (Tamamlanan / Opsiyonel)

| Adım | Açıklama | Dosya | Durum | Tahmini |
|------|----------|-------|--------|---------|
| 3.3.1 | **Grafiği Kaydet (PNG)**: SaveFileDialog → Chart.SaveImage(path, ChartImageFormat.Png) | RaporGrafikForm.cs, BtnExport_Click | ✅ | - |
| 3.3.2 | **Yazdır**: PrintDocument + PrintPage (DrawToBitmap → Graphics.DrawImage); PrintPreviewDialog ile önizleme | RaporGrafikForm.cs, BtnYazdir_Click | ✅ | - |
| 3.3.3 | **(Opsiyonel) PDF export**: Grafiği PDF sayfasına yazmak (PdfSharp veya benzeri kütüphane projede varsa); “PDF olarak kaydet” butonu | RaporGrafikForm.cs | ⏳ | 2 saat |
| 3.3.4 | **(Opsiyonel) Tablo raporları export**: Üretim/Poliçe/Finansal/Analiz raporları için Excel/PDF export (PlaceholderForm yerine gerçek rapor ekranı gelirse) | RaporlarForm, yeni formlar | ⏳ | 1 gün |

### Bağımlılıklar

- System.Windows.Forms.DataVisualization (Chart) – mevcut  
- System.Drawing.Printing (PrintDocument) – mevcut  
- (Opsiyonel) PDFsharp veya benzeri – projede kullanılıyorsa  

---

## Opsiyonel / İleride Yapılabilecekler

| Konu | Açıklama | Tahmini |
|------|----------|---------|
| **Gerçek veri servisi** | IReportService veya IOfferService.GetOfferCountByIlAsync(), GetOfferCountBySirketAsync() vb. ile grafik verisini DB’den çekmek | 1–2 gün |
| **Tarih filtresi** | RaporGrafikForm açılmadan veya form içinde “Başlangıç–Bitiş tarihi” seçimi; veriyi bu aralığa göre filtrelemek | 0,5 gün |
| **Grafik tipi seçimi** | Aynı rapor için Column/Pie/Bar seçeneği (kullanıcı tercihi) | 1 saat |
| **Üretim/Poliçe/Finansal raporları** | PlaceholderForm yerine gerçek tablo/rapor ekranı + export (Excel/PDF) | 2+ gün |

---

## Uygulama Sırası (Önerilen)

```
Faz 3 temel (tamamlandı):
  3.1.1–3.1.3  RaporlarForm + RaporGrafikForm entegrasyonu
  3.2.1–3.2.3  Tüm grafik tipleri için örnek veri
  3.3.1–3.3.2  PNG export + PrintDocument yazdırma

İsteğe bağlı:
  3.2.4  Gerçek veri (IOfferService / IReportService)
  3.3.3  Grafik PDF export
  3.1.4  Ayrı UserControl’ler (ihtiyaç halinde)
```

---

## Bağımlılık Grafiği

```
RaporlarForm (kartlar + linkler)
        │
        ▼
RaporGrafikForm(raporAdi)
        │
        ├── OlusturGrafik() → Chart (Column/Pie, örnek veri)
        ├── BtnExport_Click → PNG
        └── BtnYazdir_Click → PrintDocument

(İleride) IReportService / IOfferService
        │
        └── RaporGrafikForm → gerçek veri
```

---

## Test Kontrol Listesi

- [ ] Raporlar → Grafik Raporları kartında tüm linkler görünüyor  
- [ ] İl Bazlı Grafik tıklanınca sütun grafik ve il listesi açılıyor  
- [ ] Şirket Bazlı Grafik tıklanınca sütun grafik ve şirket listesi açılıyor  
- [ ] Mesleklere Göre Teklif Grafiği tıklanınca pasta grafik açılıyor  
- [ ] Diğer grafik raporları (Kullanım Tarzı, Marka, Otorizasyon, Komisyon, Ürün, Doğum Tarihi, Kesilen Poliçeler) doğru grafik tipi ve veri ile açılıyor  
- [ ] Grafiği Kaydet ile PNG dosyası kaydediliyor  
- [ ] Yazdır ile önizleme ve yazdırma çalışıyor  
- [ ] Üretim/Poliçe/Finansal/Analiz raporları PlaceholderForm açıyor (grafik değil)  

---

## Dosya Değişiklik Özeti

| Dosya | İşlem | Açıklama |
|-------|--------|----------|
| RaporlarForm.cs | Mevcut | Grafik Raporları kartı, GrafikItems, AcRaporDetay → RaporGrafikForm |
| RaporGrafikForm.cs | Mevcut | OlusturGrafik (rapor adına göre), PNG export, PrintDocument yazdırma |
| (İleride) IReportService.cs | Opsiyonel | Rapor verisi için servis arayüzü |
| (İleride) ReportService.cs | Opsiyonel | Teklif/poliçe gruplama ve sayım |

---

## Özet Tablo – Open Hızlı Teklif Karşılıkları

| Open Hızlı Teklif | VegaAsis | Durum |
|-------------------|----------|--------|
| UserControlRaporlar | RaporlarForm | ✅ |
| UserControlCityGrafik | RaporGrafikForm("İl Bazlı Grafik") | ✅ |
| UserControlSirketBazliGrafik | RaporGrafikForm("Şirket Bazlı Grafik") | ✅ |
| UserControlMesleklereGoreTeklifGrafigi | RaporGrafikForm("Mesleklere Göre Teklif Grafiği") | ✅ |
| UserControlTeklifKullanimTarziGrafigi | RaporGrafikForm("Teklif Kullanım Tarzı Grafiği") | ✅ |
| UserControlTeklifMarkaGrafigi | RaporGrafikForm("Teklif Marka Grafiği") | ✅ |
| UserControlTeklifOtorizasyonOranlari | RaporGrafikForm("Teklif Otorizasyon Oranları") | ✅ |
| UserControlTeklifKomisyonKazanci | RaporGrafikForm("Teklif Komisyon Kazancı") | ✅ |
| UserControlUrunGrafigi | RaporGrafikForm("Ürün Grafiği") | ✅ |
| UserControlDogumTarihiPortfoy | RaporGrafikForm("Doğum Tarihi Portföy") | ✅ |
| UserControlKesilenPoliceler | RaporGrafikForm("Kesilen Poliçeler") | ✅ |
| XtraPrinting (yazdır/export) | PrintDocument + Chart.SaveImage(PNG) | ✅ |

---

*Bu plan Faz 3 için detaylı rehber niteliğindedir. Temel maddeler uygulandı; opsiyonel maddeler ihtiyaca göre plana eklenebilir.*
