# Faz 6 – Kalan Formlar ve İyileştirmeler Notları

Bu doküman Faz 6 formlarının mevcut durumunu ve entegrasyon noktalarını özetler.

---

## 6.1 frmManuelUavtSorgu (ManuelUavtSorguForm)

| Özellik | Durum |
|--------|--------|
| Form | ✅ Mevcut – TC/Vergi No veya Plaka ile sorgu alanları, sonuç grid |
| İş mantığı | ✅ IUavtService ServiceLocator ile çözülüyor; SorgulaTcVergiAsync / SorgulaPlakaAsync çağrılıyor; sonuçlar grid'e yazılıyor |
| Servis | IUavtService, UavtSonucDto, UavtService (stub – boş liste döner); Program.cs'te kayıtlı |
| Sonraki adım | Gerçek UAVT API/entegrasyon bilgisi geldiğinde UavtService içi doldurulacak |

---

## 6.2 frmTramerSorgu (TramerSorguForm)

| Özellik | Durum |
|--------|--------|
| Form | ✅ Mevcut – Plaka / Şasi No sorgu, sonuç grid |
| İş mantığı | ✅ ITramerService ServiceLocator ile çözülüyor; SorgulaPlakaAsync / SorgulaSasiNoAsync çağrılıyor |
| Not | TramerService şu an boş liste döndürüyor; gerçek API bilgisi DOCS/Tramer-API-Notlari.md ile eklenecek |

---

## 6.3 Loading / Bildirim

| Bileşen | Durum |
|---------|--------|
| LoadingForm | ✅ Mevcut – Mesaj, Indeterminate progress; ShowWhile(owner, work, mesaj) ile arka planda iş + modal gösterim |
| BildirimEkraniForm | ✅ Mevcut – ListView (Tarih, Tip, Başlık, İçerik), Temizle/Kapat; OrnekBildirimYukle() ile örnek veri |
| Sonraki adım | Bildirimler gerçek kaynaktan (servis/DB) beslenmek istenirse bildirim servisi ve forma bağlama |

---

## 6.4 Otomatik Sorgu

| Form | Durum |
|------|--------|
| SablonDuzenleForm | ✅ Mevcut – Şablon listesi, adı, branş, şirketler (CheckedListBox); Yeni/Kaydet/Sil; OrnekYukle() ile örnek |
| WSTeklifleriniSorgulaForm | ✅ Mevcut – Şablon combo, başlangıç/bitiş tarihi, Sorgula; ProgressBar ve grid; BtnSorgula stub |
| Sonraki adım | Şablon ve WS sorgu servisleri tanımlandığında formlar bu servislere bağlanacak |

---

## 6.5 Layout Düzeltmeleri

Genel UI/UX iyileştirmeleri ihtiyaç halinde yapılır (anchor, minimum boyut, yazı tipi tutarlılığı vb.).
