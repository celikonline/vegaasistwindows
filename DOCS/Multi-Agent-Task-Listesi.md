# Multi-Agent Task Listesi

**Kural:** Her agent aynı anda yalnızca **1 dosya** üzerinde çalışır. Paralel çalışan agent'lar farklı dosyalara atanır.

**Kaynak plan:** `Open-Hizli-Teklif-VegaAsis-Karsilastirma-Plani.md` (Faz 5, 6, 7)

---

## Faz 5: Destek ve Admin — Dalga Sırası

| Dalga | Agent | Tek dosya | Görev | Durum |
|-------|-------|-----------|--------|--------|
| **1** | **5.C** | `src/VegaAsis.Data/Services/WebUserService.cs` | WebUser CRUD, unlicensed_agent_only filtreleme (GetAllAsync parametresi) | ✅ Tamamlandı |
| **2** | **5.B** | `src/VegaAsis.Windows/UserControls/WebEkranlariControl.cs` | WEB Ekranları: WebUser listesi, unlicensed_agent_only filtresi UI, CRUD arayüzü | ✅ Tamamlandı |
| **2** | **5.A** | `src/VegaAsis.Windows/UserControls/PaylasilanSirketlerControl.cs` | Benimle Paylaşılanlar grid, yeni paylaşım ekleme/silme | ✅ Tamamlandı |
| **2** | **5.D** | `src/VegaAsis.Windows/Forms/ConfigForm.cs` | Proxy, benchmark ayarları, ConfigStorage | ✅ Tamamlandı |
| **2** | **5.F** | `src/VegaAsis.Windows/UserControls/DigerAyarlarControl.cs` | AppSettings binding, admin-only alanlar | ✅ Tamamlandı |
| **3** | **5.E** | `src/VegaAsis.Windows/Forms/AdminPanelForm.cs` | Tab host (Paylaşılan Şirketler / WEB Ekranları / Diğer Ayarlar) | ✅ Zaten mevcut |

---

## Faz 6: Kalan Formlar — Dalga Sırası

| Dalga | Agent | Tek dosya | Görev | Durum |
|-------|-------|-----------|--------|--------|
| **1** | 6.A | `src/VegaAsis.Windows/Forms/ManuelUavtSorguForm.cs` | Manuel UAVT sorgu ekranı | ✅ Tamamlandı |
| **1** | 6.B | `src/VegaAsis.Windows/Forms/TramerSorguForm.cs` | Tramer sorgu ekranı | ✅ Tamamlandı |
| **1** | 6.C | `src/VegaAsis.Windows/Forms/LoadingForm.cs` | frmLoading: bekleme, progress, modal | ✅ Tamamlandı |
| **1** | 6.D | `src/VegaAsis.Windows/Forms/BildirimEkraniForm.cs` | Bildirim listesi / tek bildirim | ✅ Tamamlandı |
| **2** | 6.E | `src/VegaAsis.Windows/Forms/SablonDuzenleForm.cs` | Otomatik sorgu şablonu | ✅ Tamamlandı |
| **2** | 6.F | `src/VegaAsis.Windows/Forms/WSTeklifleriniSorgulaForm.cs` | WS tekliflerini toplu sorgulama | ✅ Tamamlandı |
| **3** | 6.G | `src/VegaAsis.Windows/UserControls/IndexViewControl.cs` | Layout: kaymalar, responsive, anchor/dock | ✅ Tamamlandı |

---

## Faz 7: Test ve Dokümantasyon — Dalga Sırası

| Dalga | Agent | Tek dosya | Görev | Durum |
|-------|-------|-----------|--------|--------|
| **1** | 7.B | `src/VegaAsis.Data/VegaAsisDbContext.cs` | Migration/connection yorumu eklendi | ✅ Tamamlandı |
| **1** | 7.E | `src/VegaAsis.Windows/Program.cs` | DI, global exception handler (ThreadException, UnhandledException) | ✅ Tamamlandı |
| **2** | 7.A | `src/VegaAsis.Windows/MainForm.cs` | OpenForm try/catch, form.Dispose | ✅ Tamamlandı |
| **3** | 7.C | `DOCS/Open-Hizli-Teklif-VegaAsis-Karsilastirma-Plani.md` | Tamamlanan fazlar notu eklendi | ✅ Tamamlandı |
| **3** | 7.D | `README.md` | Kurulum, çalıştırma, ortam, robot/Config | ✅ Tamamlandı |

---

## Faz 1: Kritik Eksikler — Tamamlandı

| Görev | Açıklama | Durum |
|-------|----------|--------|
| 1.1 | IndexViewControl → Sorgu Akışı (Başlat/Duraklat/Yeni Sorgu Kaydet) | ✅ MainForm + SorguSession |
| 1.2 | PolicyTypeSelect + Branş Formları | ✅ Mevcut |
| 1.3 | Veri Kaynakları (İl/İlçe, Meslek, Kullanım Tarzı) | ✅ Mevcut |
| 1.4 | Trafik/Kasko Pol. Bilgisi Tab (Kasko fieldKey + SorguSession) | ✅ |
| 1.5 | DaskDetaylariForm (İl/İlçe/Mahalle, Yapı Tarzı, İnşa Yılı, Kat Sayısı) | ✅ Mevcut |

---

## Son durum

- **Faz 1** (Kritik Eksikler) tamamlandı.
- **Faz 2, Faz 3, Faz 4** (Form, Rapor, Robot altyapı/stub) tamamlandı.
- **Faz 5, Faz 6 ve Faz 7** tamamlandı. Multi-agent planı (Faz 5–7) bitti.
- **Kullanım Tarzı / Araç Markası:** Offer entity/DTO, IndexView + TeklifDetayForm girişi, RaporGrafikForm gerçek veri; migration script: `DOCS/migration-offer-kullanim-tarzi-arac-markasi.sql`.
- **Servis-form entegrasyonu:** DASK/Kasko/İMM/Konut/TSS servisleri DaskDetaylariForm, KaskoTeminatlariForm, ImmTeminatlariForm, KonutTeminatlariForm, TssDetaylariForm ile bağlandı; DASK’ta İl/İlçe değişince teminat yenileme + Yenile butonu.
- **Konut:** KonutTeminatlariForm eklendi; MainForm KONUT dalında açılıyor.
- Plan belgesi (Open-Hizli-Teklif-VegaAsis-Karsilastirma-Plani.md) form/servis/robot tabloları ve Faz 4/Kullanım Tarzı-Marka durumu güncel (Şubat 2025).
