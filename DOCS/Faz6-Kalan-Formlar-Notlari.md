# Faz 6 – Kalan Formlar ve İyileştirmeler Notları

Bu doküman Faz 6 formlarının mevcut durumunu ve entegrasyon noktalarını özetler.

---

## 6.1 frmManuelUavtSorgu (ManuelUavtSorguForm)

| Özellik | Durum |
|--------|--------|
| Form | ✅ Mevcut – TC/Vergi No veya Plaka ile sorgu alanları, sonuç grid |
| İş mantığı | ✅ IUavtService ServiceLocator ile çözülüyor; SorgulaTcVergiAsync / SorgulaPlakaAsync çağrılıyor; sonuçlar grid'e yazılıyor |
| Servis | IUavtService, UavtSonucDto, UavtService (App.config UavtApiBaseUrl ile HTTP GET; boşsa boş liste) |
| Sonraki adım | Gerçek UAVT API URL'si App.config'e eklendiğinde sorgu çalışır |

---

## 6.2 frmTramerSorgu (TramerSorguForm)

| Özellik | Durum |
|--------|--------|
| Form | ✅ Mevcut – Plaka / Şasi No sorgu, sonuç grid |
| İş mantığı | ✅ ITramerService ServiceLocator ile çözülüyor; SorgulaPlakaAsync / SorgulaSasiNoAsync çağrılıyor |
| Not | TramerService App.config TramerApiBaseUrl ile HTTP GET; boşsa boş liste. DOCS/Tramer-API-Notlari.md |

---

## 6.3 Loading / Bildirim

| Bileşen | Durum |
|---------|--------|
| LoadingForm | ✅ Mevcut – Mesaj, Indeterminate progress; ShowWhile(owner, work, mesaj) ile arka planda iş + modal gösterim |
| BildirimEkraniForm | ✅ IBildirimService ile bağlandı – Form açılışta servisten liste yükleniyor; ServiceLocator yoksa örnek veri |
| Servis | IBildirimService, BildirimDto, BildirimService (stub – örnek 3 kayıt); Program.cs'te kayıtlı |
| Sonraki adım | Bildirimler DB/duyuru kaynağından beslenmek istenirse BildirimService içi doldurulacak |

---

## 6.4 Otomatik Sorgu

| Form | Durum |
|------|--------|
| SablonDuzenleForm | ✅ ISablonService + ICompanySettingsService ile bağlandı – listeleme, kaydet, sil servisten |
| WSTeklifleriniSorgulaForm | ✅ ISablonService + IWsSorguService ile bağlandı – şablon combo servisten, Sorgula IWsSorguService.SorgulaAsync |
| Servis | ISablonService, IWsSorguService, SablonService (bellek), WsSorguService (stub); Program.cs'te kayıtlı |
| Sonraki adım | WsSorguService gerçek WS/API entegrasyonu; SablonService istenirse DB'ye taşınabilir |

---

## 6.5 Layout Düzeltmeleri

| Form | Yapılan |
|------|--------|
| SablonDuzenleForm | Şablon adı, branş, şirketler alanları ve Kapat butonu için Anchor (yatay esneme / sağda sabit) |
| WSTeklifleriniSorgulaForm | Sorgula butonu ve ProgressBar Anchor (sağda sabit / yatay esneme); Kapat Anchor Bottom, Right |
| BildirimEkraniForm | Temizle/Kapat butonları Anchor Bottom, Left / Bottom, Right |
| ManuelUavtSorguForm | Sorgu grubu Anchor Top, Left, Right; Sorgula Anchor Top, Right; Kapat Anchor Bottom, Right |
| TramerSorguForm | Sorgu grubu Anchor Top, Left, Right; Sorgula Anchor Top, Right; Kapat Anchor Bottom, Right |

Genel UI/UX iyileştirmeleri ihtiyaç halinde genişletilebilir (yazı tipi tutarlılığı, ek formlar).
