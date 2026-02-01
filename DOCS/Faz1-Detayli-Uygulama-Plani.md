# Faz 1 – Detaylı Uygulama Planı

**Süre:** 1–2 hafta  
**Kaynak:** Open-Hizli-Teklif-VegaAsis-Karsilastirma-Plani.md, Bölüm 6 – Faz 1

---

## Genel Bakış

Faz 1’in amacı: Ana ekrandaki (IndexViewControl) sorgu akışını çalışır hale getirmek, branş formları entegrasyonunu tamamlamak, veri kaynaklarını doldurmak ve DASK formunu zenginleştirmek.

---

## Görev 1.1: Sorgu Akışı (Sorguyu Başlat, Duraklat, Yeni Sorgu Kaydet)

### Mevcut Durum
- `IndexViewControl`: SorguBaslatRequested, DuraklatRequested, YeniSorguKaydetRequested event’leri tanımlı
- `MainForm.ShowIndexView()`: Event’lere MessageBox placeholder bağlı

### Yapılacaklar

| Adım | Açıklama | Dosya | Tahmini |
|------|----------|-------|---------|
| 1.1.1 | **SorguSession sınıfı** oluştur – sağ paneldeki müşteri/araç/poliçe bilgilerini tutacak in-memory model | `VegaAsis.Windows/Models/SorguSession.cs` (yeni) | 2 saat |
| 1.1.2 | **IndexViewControl**: SorguSession referansı ekle; form değerlerini session’a yaz/oku | `IndexViewControl.cs` | 2 saat |
| 1.1.3 | **Sorguyu Başlat**: (a) Session’dan form verilerini al, (b) Seçili şirketleri al, (c) Mock/gerçek sorgu başlat (OfferService.CreateAsync veya geçici mock) | `MainForm.cs`, `IndexViewControl.cs` | 3 saat |
| 1.1.4 | **Duraklat / Devam Et**: Sorgu durumunu (Idle / Running / Paused) tut; buton metnini değiştir | `IndexViewControl.cs`, `SorguSession.cs` | 2 saat |
| 1.1.5 | **Yeni Sorgu Kaydet**: Session temizle, form alanlarını varsayılana döndür, yeni TeklifId üret (opsiyonel) | `IndexViewControl.cs` | 1 saat |

### SorguSession Taslağı

```csharp
// VegaAsis.Windows/Models/SorguSession.cs
public class SorguSession
{
    public Guid? TeklifId { get; set; }
    public SorguDurum Durum { get; set; }  // Idle, Running, Paused
    public string Plaka { get; set; }
    public string TcVergi { get; set; }
    public string BelgeSeri { get; set; }
    public string BelgeNo { get; set; }
    public DateTime? DogumTarihi { get; set; }
    public string MusteriAdi { get; set; }
    public string Meslek { get; set; }
    public string Il { get; set; }
    public string Ilce { get; set; }
    public string KullanimTarzi { get; set; }
    public string Marka { get; set; }
    public string Tip { get; set; }
    public int? ModelYili { get; set; }
    public List<string> SeciliSirketler { get; set; }
    // ... diğer alanlar
}
```

### Bağımlılıklar
- IOfferService (mevcut)
- IAuthService (CurrentUserId için)

---

## Görev 1.2: PolicyTypeSelect + Branş Formları

### Mevcut Durum
- `_cmbPolicyType`: TRAFİK, KASKO, DASK, TSS, KONUT, İMM mevcut
- `BranchFormRequested`: Değişince MainForm’da ilgili form açılıyor (dialog)
- Grid’de Trafik, Kasko, TSS, Konut, Dask, Imm kolonları var

### Yapılacaklar

| Adım | Açıklama | Dosya | Tahmini |
|------|----------|-------|---------|
| 1.2.1 | **PolicyType değişiminde dialog açma davranışını gözden geçir**: Şu an her değişimde form açılıyor – kullanıcı deneyimi için sadece ilgili branş seçildiğinde veya “Detay” butonuna basıldığında açmak daha uygun olabilir | `MainForm.cs`, `IndexViewControl.cs` | 1 saat |
| 1.2.2 | **Grid hücre tıklaması**: Trafik, Kasko, TSS, DASK, Konut, IMM kolonlarına tıklanınca ilgili branş formunu aç (seçili satırdaki şirket bilgisi ile) | `IndexViewControl.cs` | 2 saat |
| 1.2.3 | **BranchFormRequested imzasını genişlet**: Şirket adı ve branş bilgisini parametre olarak geçir | `IndexViewControl.cs`, `MainForm.cs` | 1 saat |
| 1.2.4 | **TRAFİK için TrafikTeklifiForm**: IndexViewControl’de TRAFİK seçildiğinde veya Trafik kolonuna tıklanınca TrafikTeklifiForm açılsın (şu an yoksa PlaceholderForm veya mevcut form kullan) | `MainForm.cs` | 1 saat |

### Önerilen Akış
1. `_cmbPolicyType` sadece görsel filtre / aktif branş bilgisi için kullanılsın; değişimde otomatik form açılmasın.
2. Grid’de branş kolonuna (Trafik, Kasko vb.) tıklanınca ilgili form açılsın, seçili şirket bilgisi parametre olarak gönderilsin.
3. Sağ panelde “Detay Görüntüle” benzeri bir buton eklenebilir – tıklanınca aktif branşa göre form açılır.

---

## Görev 1.3: Veri Kaynakları (İl/İlçe, Meslek, Kullanım Tarzı)

### Mevcut Durum
- **TurkeyLocations**: IndexViewControl’de `AddFormRowIlIlce` ile kullanılıyor; `GetCityNames()`, `GetDistrictsByCity()` mevcut. 13 il var (Adana, Ankara, Antalya, Bursa, İstanbul, İzmir, Kocaeli, Konya, Manisa, Muğla, Sakarya).
- **Professions**: IndexViewControl’de Meslek combo’da kullanılıyor.
- **KullanimTarziOptions**: Araç Bilgileri tab’ında Kullanım Tarzı combo’da kullanılıyor.
- **DaskDetaylariForm**: İl combo’da sadece 6 il hardcoded; İlçe TextBox.

### Yapılacaklar

| Adım | Açıklama | Dosya | Tahmini |
|------|----------|-------|---------|
| 1.3.1 | **TurkeyLocations genişletme** (opsiyonel): Eksik illeri ekle – şu an 13 il var, 81 ile tamamlanabilir | `Data/TurkeyLocations.cs` | 2–4 saat |
| 1.3.2 | **DaskDetaylariForm**: İl combo’yu TurkeyLocations.GetCityNames() ile doldur | `Forms/DaskDetaylariForm.cs` | 30 dk |
| 1.3.3 | **DaskDetaylariForm**: İlçe’yi ComboBox yap; İl seçimine göre cascade (TurkeyLocations.GetDistrictsByCity) | `Forms/DaskDetaylariForm.cs` | 1 saat |
| 1.3.4 | **Doğrulama**: IndexViewControl’de Meslek ve Kullanım Tarzı combo’larının doğru dolu olduğunu kontrol et | - | 15 dk |

### Not
IndexViewControl’de veri kaynakları zaten bağlı; ana eksik DaskDetaylariForm’daki İl/İlçe entegrasyonu.

---

## Görev 1.4: Trafik / Kasko Pol. Bilgisi Tab İçerikleri

### Mevcut Durum
- **Trafik Pol. Bilgisi** tab: Sigorta Şirketi, Acente Kodu, Poliçe No, Kademe, Hasarsızlık %, Başlangıç T., Bitiş T., Kalan Gün, Yenileme No – hepsi `AddFormRow` ile eklenmiş, boş.
- **Kasko Pol. Bilgisi** tab: Benzer alanlar mevcut.

### Yapılacaklar

| Adım | Açıklama | Dosya | Tahmini |
|------|----------|-------|---------|
| 1.4.1 | **Sigorta Şirketi combo**: Şirket listesini CompanySettingsService veya sabit listeden doldur | `IndexViewControl.cs` | 1 saat |
| 1.4.2 | **Kalan Gün hesaplama**: Başlangıç/Bitiş tarihi değişince Kalan Gün otomatik hesaplansın | `IndexViewControl.cs` | 1 saat |
| 1.4.3 | **SorguSession bağlantısı**: Tab alanlarının SorguSession ile sync edilmesi (1.1 tamamlandıktan sonra) | `IndexViewControl.cs` | 1 saat |
| 1.4.4 | **Trafik tab için Kısa Vadeli checkbox**: Toolbar’daki “Kısa Vadeli Poliçe Çalış” ile ilişkilendir (opsiyonel) | `IndexViewControl.cs` | 30 dk |

---

## Görev 1.5: DASK Formu Zenginleştirme (frmHizliDaskSorgu Benzeri)

### Mevcut Durum
- Yapı Tarzı: 4 seçenek (Betonarme, Yığma, Çelik, Diğer)
- İl: 6 il hardcoded
- İlçe: TextBox
- Mahalle: TextBox
- Kat Sayısı, Bulunduğu Kat, Brüt Yüzölçümü, İnşaat Yılı: Mevcut

### Yapılacaklar

| Adım | Açıklama | Dosya | Tahmini |
|------|----------|-------|---------|
| 1.5.1 | **İl / İlçe cascade**: TurkeyLocations kullan; İl ComboBox + İlçe ComboBox (1.3.2, 1.3.3 ile aynı) | `Forms/DaskDetaylariForm.cs` | 1 saat |
| 1.5.2 | **Yapı Tarzı listesi genişlet**: Orijinalde YapiTarzi listesi var; gerekirse “Çatı Tipi”, “Bina Türü” gibi ek alanlar eklenebilir | `Forms/DaskDetaylariForm.cs` | 30 dk |
| 1.5.3 | **İnşaat Yılı**: ComboBox veya NumericUpDown (mevcut); 1900–şimdi aralığı yeterli | - | - |
| 1.5.4 | **Mahalle**: TextBox kalabilir (API yoksa); veya ileride DASK API’den mahalle listesi çekilebilir | - | - |
| 1.5.5 | **KONUT ayrımı**: DASK vs KONUT için farklı alan gösterimi (opsiyonel – aynı formda kalabilir) | - | 1 saat |

---

## Uygulama Sırası (Önerilen)

```
Gün 1–2:  1.3 (Veri kaynakları) + 1.5 (DASK)     → Hızlı kazanım, az bağımlılık
Gün 3–4:  1.1 (Sorgu akışı)                      → SorguSession + event handler’lar
Gün 5:    1.2 (Branş formları)                   → Grid tıklama, form parametreleri
Gün 6–7:  1.4 (Pol. Bilgisi tab)                 → Tab içerikleri, şirket listesi
```

---

## Bağımlılık Grafiği

```
1.3 (Veri) ──┬──► 1.5 (DASK)
             │
1.1 (Session) ├──► 1.4 (Tab içerik)
             │
1.2 (Branş) ─┘
```

- 1.3 ve 1.5 birbirinden bağımsız; paralel yapılabilir.
- 1.1 tamamlanmadan 1.4’te SorguSession bağlantısı yapılamaz.
- 1.2, 1.1’den bağımsız; grid tıklama sadece form açma ile ilgili.

---

## Test Kontrol Listesi

- [ ] Sorguyu Başlat tıklanınca session verileri alınıyor, mock/gerçek kayıt oluşuyor
- [ ] Duraklat / Devam Et buton metni ve durum değişiyor
- [ ] Yeni Sorgu Kaydet formu temizliyor
- [ ] Grid’de Trafik/Kasko/DASK vb. kolona tıklanınca ilgili form açılıyor
- [ ] DaskDetaylariForm’da İl seçilince İlçe combo doluyor
- [ ] IndexViewControl’de İl/İlçe cascade çalışıyor
- [ ] Trafik/Kasko Pol. Bilgisi tab’larında Sigorta Şirketi listesi dolu
- [ ] Kalan Gün otomatik hesaplanıyor

---

## Dosya Değişiklik Özeti

| Dosya | İşlem |
|-------|-------|
| `VegaAsis.Windows/Models/SorguSession.cs` | Yeni |
| `IndexViewControl.cs` | SorguSession, grid CellClick, tab düzenlemeleri |
| `MainForm.cs` | Sorgu akışı handler’ları, BranchFormRequested parametreleri |
| `Forms/DaskDetaylariForm.cs` | TurkeyLocations, İlçe cascade |
| `Data/TurkeyLocations.cs` | Opsiyonel: eksik iller |
| `Data/YapiTarziOptions.cs` | Opsiyonel: DASK yapı tarzları (ayrı dosya veya DaskDetaylariForm içi) |

---

*Bu plan Faz 1 için detaylı rehber niteliğindedir. İlerleme durumuna göre güncellenebilir.*
