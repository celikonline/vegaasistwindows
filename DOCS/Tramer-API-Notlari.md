# Tramer API Notları

Bu doküman Tramer entegrasyonu için endpoint, format ve kimlik doğrulama bilgilerini toplamak üzere kullanılır. Gerçek API bilgisi proje sahibi veya Open Hızlı Teklif incelemesi ile doldurulacaktır.

---

## Özet

- **ITramerService** ve **TramerService** (VegaAsis.Core / VegaAsis.Data) mevcut; App.config `TramerApiBaseUrl` boşsa boş liste döndürür.
- **TramerSorguForm** ServiceLocator ile ITramerService kullanıyor; plaka veya şasi no ile sorgulama butonu bağlı.
- Varsayılan varsayım: REST/JSON + HTTP GET; opsiyonel API key header desteklenir.

---

## Toplanacak Bilgiler

| Madde | Açıklama | Durum |
|-------|----------|--------|
| **Endpoint URL** | Tramer sorgu API base URL (HTTP/HTTPS) | Bekleniyor |
| **Protokol** | REST JSON, SOAP, form POST vb. | Bekleniyor |
| **Plaka sorgusu** | Parametre adı (plaka, plate vb.), HTTP metodu (GET/POST) | Bekleniyor |
| **Şasi no sorgusu** | Parametre adı (sasiNo, chassis vb.) | Bekleniyor |
| **Kimlik doğrulama** | API key, Basic auth, token; nerede saklanacak (AppSettings, şifreli) | Varsayılan: `TramerApiKeyHeader` + `TramerApiKey` |
| **Yanıt formatı** | JSON alanları (plaka, marka, model, hasar tarihi, şirket vb.) → TramerSonucDto eşlemesi | Bekleniyor |
| **Hata / timeout** | HTTP timeout süresi, hata kodları | Bekleniyor |

---

## Teknik Referans

- **Arayüz:** `VegaAsis.Core/Contracts/ITramerService.cs` – `SorgulaPlakaAsync(string plaka)`, `SorgulaSasiNoAsync(string sasiNo)`.
- **DTO:** `VegaAsis.Core/DTOs/TramerSonucDto.cs` – Plaka, Marka, Model, HasarTarihi, Sirket, Aciklama.
- **Implementasyon:** `VegaAsis.Data/Services/TramerService.cs` – AppSettings `TramerApiBaseUrl` varsa HTTP GET ile çağrı yapar; opsiyonel header auth destekli (`TramerApiKeyHeader`, `TramerApiKey`).

---

## Sonraki Adım

Endpoint ve format netleşince `TramerService.cs` içinde `HttpClient` ile istek atılıp yanıt `TramerSonucDto` listesine dönüştürülecek; gerekirse App.config veya veritabanı ayar tablosuna Tramer URL ve API key alanları eklenir.
