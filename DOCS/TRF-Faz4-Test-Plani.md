# TRF Faz 4 — Test & İyileştirme Planı

Bu doküman, TRF şirket robotlarının test adımlarını ve sonuç takibini tanımlar.

---

## 1. Test Türleri

| # | Test | Açıklama | Nasıl Çalıştırılır |
|---|------|----------|--------------------|
| 1 | **Login testi** | Her şirket için giriş sayfasına gidip credential ile giriş denenir. | Şirketler / Robot → ▶ Başlat → "Tümüne Giriş (sıralı)" veya "Login Testi" formu. |
| 2 | **Teklif sorgu testi** | Giriş sonrası plaka/TCKN ile teklif formu doldurulup sorgu gönderilir. | ▶ Başlat → "Tümünden Teklif Al". |
| 3 | **Captcha akışı** | Captcha olan şirketlerde (örn. Anadolu) manuel/otomatik çözüm akışı. | İlgili TRF'ta SelectorCaptcha + ManuelCaptchaResolver entegrasyonu test edilir. |
| 4 | **Paralel çalıştırma** | Birden fazla şirkete aynı anda teklif sorgusu. | ▶ Başlat → "Tümünden Teklif Al (Paralel)". |

---

## 2. Ön Koşullar

- [ ] Chrome/ChromeDriver uyumlu sürüm yüklü.
- [ ] İlgili şirketler için **Kimlik Bilgileri** ekranından kullanıcı adı/şifre girilmiş (veya test edilecek şirket sayısı kadar credential tanımlı).
- [ ] Veritabanı erişimi (company_credentials tablosu uygulama açılışında oluşturulur).

---

## 3. Login Testi — Adım Adım

1. Uygulamayı aç, giriş yap.
2. **Şirketler / Robot** formunu aç.
3. **Kimlik Bilgileri** ile test edilecek şirketler için kullanıcı adı/şifre kaydet.
4. **▶ Başlat** → **Login Testi** (veya **Tümüne Giriş (sıralı)**) ile testi çalıştır.
5. Sonuçları aşağıdaki **Şirket Bazlı Login Test Sonuçları** tablosuna işle (✅/❌ ve not).

---

## 4. Şirket Bazlı Login Test Sonuçları

| # | Şirket | CompanyId | Login Sonucu | Not |
|---|--------|-----------|--------------|-----|
| 1 | AK Sigorta | ak | | |
| 2 | Anadolu Sigorta | anadolu | | |
| 3 | ANA Sigorta | ana | | |
| 4 | Allianz | allianz | | |
| 5 | Sompo Japan | sompo | | |
| 6 | HDI | hdi | | |
| 7 | Mapfre | mapfre | | |
| 8 | Güneş Sigorta | gunes | | |
| 9 | Groupama | groupama | | |
| 10 | Zurich | zurich | | |
| 11 | Neova | neova | | |
| 12 | Eureko | eureko | | |
| 13 | Ergo | ergo | | |
| 14 | Generali | generali | | |
| 15 | Türk Nippon | turknippon | | |
| 16 | Ray Sigorta | ray | | |
| 17 | Doğa Sigorta | doga | | |
| 18 | Ankara Sigorta | ankara | | |
| 19 | Halk Sigorta | halk | | |
| 20 | Koru Sigorta | koru | | |
| 21 | Orient | orient | | |
| 22 | Quick Sigorta | quick | | |
| 23 | Demir Hayat | demirhayat | | |
| 24 | Gulf Sigorta | gulf | | |
| 25 | Magdeburger | magdeburger | | |
| 26 | Bereket | bereket | | |
| 27 | Corpus | corpus | | |
| 28 | Hepiyi | hepiyi | | |
| 29 | Şeker Sigorta | seker | | |
| 30 | Türkiye Sigorta | turkiye | | |
| 31 | Unico | unico | | |

**Sonuç:** ✅ Başarılı | ❌ Başarısız (not sütununa hata/selector bilgisi yazılabilir)

---

## 5. Teklif Sorgu Testi

- **Çalıştırma:** ▶ Başlat → **Tümünden Teklif Al** (veya seçili şirketlerle).
- **Beklenen:** Her şirket için giriş + teklif formu doldurulur, sorgu gönderilir; sonuç mesajı görünür.
- **Doğrulama:** Hata alan şirketlerde `DOCS/TRF-Selector-Guncelleme.md` ile selector/URL kontrol edilir.

---

## 6. Paralel Çalıştırma Testi

- **Çalıştırma:** ▶ Başlat → **Tümünden Teklif Al (Paralel)**.
- **Beklenen:** Aynı anda en fazla 3 Chrome penceresi açılır, şirketler paralel işlenir.
- **Doğrulama:** Timeout/çakışma olmamalı; sonuç özeti tüm şirketleri kapsamalı.

---

## 7. Captcha Akışı

- Captcha kullanan şirketlerde (ör. Anadolu) giriş sayfasında captcha alanı varsa:
  - TRF dosyasında `SelectorCaptcha` tanımlı olmalı.
  - Gerekirse `ICaptchaResolver` (ManuelCaptchaResolver) ile kullanıcıdan çözüm alınmalı.
- Test: İlgili şirket için "Chrome ile Aç" ile giriş dene; captcha çıkarsa manuel çözüm akışı test edilir.

---

## 8. Test Sonrası İyileştirmeler

- Login veya teklif başarısız olan şirketlerde:
  1. Portal URL’sini (LoginUrl / teklif sayfası) kontrol et.
  2. Chrome F12 ile giriş/teklif formu id/name/class değerlerini al.
  3. `DOCS/TRF-Selector-Guncelleme.md` rehberine göre ilgili TRF dosyasındaki selector’ları güncelle.
  4. Tekrar test et.

---

*Bu plan, TRF Faz 4 testlerinin izlenmesi ve sonuçların kaydedilmesi için kullanılır.*
