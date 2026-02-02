# TRF Selector Güncelleme Rehberi

Her şirket robotundaki CSS selector'lar **placeholder** (genel) tanımlıdır. Gerçek portalda çalışması için ilgili TRF dosyasında güncellenmelidir.

**Uygulama durumu:** Tüm 31 TRF dosyasında rehberdeki **çoklu selector (yedek)** stratejisi uygulandı: Türkçe portal için yaygın pattern'ler (`kullaniciAdi`, `sifre`, `btnGiris`, `txtPlaka`, `txtTcKimlikNo`, `btnSorgula` vb.) eklendi. Gerçek portallarda test edilip id/name/class değerleri rehbere göre doğrulanmalıdır.

---

## 1. Selector'lar Nerede?

Her `TRF_*.cs` dosyasının üst kısmında sabitler vardır:

```csharp
private const string SelectorUserName = "input[name=\"username\"], #username";
private const string SelectorPassword = "input[type=\"password\"], #password";
private const string SelectorLoginButton = "button[type=\"submit\"]";
private const string SelectorPlaka = "input[name=\"plaka\"], #plaka";
private const string SelectorTckn = "input[name=\"tckn\"], #tckn";
private const string SelectorSorgulaButton = "button[type=\"submit\"], .btn-sorgula";
```

- **Login:** `SelectorUserName`, `SelectorPassword`, `SelectorLoginButton` (bazı TRF'larda `SelectorCaptcha`)
- **Teklif formu:** `SelectorPlaka`, `SelectorTckn`, `SelectorSorgulaButton`

Ayrıca **LoginUrl** (ve gerekiyorsa teklif sayfası URL’i) gerçek acente portal adresiyle değiştirilmelidir.

---

## 2. Selector Nasıl Bulunur?

1. **Chrome** ile şirketin acente giriş sayfasını açın.
2. **F12** → **Elements** sekmesi.
3. **Ctrl+F** ile sayfada metin arayın (örn. "Kullanıcı", "Şifre", "Giriş").
4. İlgili `<input>` veya `<button>` üzerine sağ tık → **Copy** → **Copy selector** veya **Copy full XPath**.
   - CSS selector tercih edilir (XPath yerine); daha okunaklı ve genelde daha dayanıklıdır.
5. Öncelik sırası:
   - `id` varsa: `#idDegeri`
   - `name` varsa: `input[name="nameDegeri"]`
   - `class` varsa: `.sinifAdi` (tek sınıf yeterli olacak şekilde)
   - Aria/role: `[aria-label="..."]` vb.

### Çoklu selector (yedek)

Portal güncellemelerinde tek selector bozulabilir. Bu yüzden TRF’lerde **virgülle ayrılmış** birden fazla seçenek kullanılır; driver ilk bulduğunu kullanır:

```csharp
private const string SelectorUserName = "input[name=\"kullaniciAdi\"], #txtKullaniciAdi, input[type=\"text\"].form-control";
```

---

## 3. Güncellenecek Dosya ve Alanlar

| TRF Dosyası | Şirket | LoginUrl Kontrol | Kullanıcı | Şifre | Giriş Butonu | Plaka | TCKN | Sorgula |
|-------------|--------|------------------|----------|-------|---------------|-------|------|---------|
| TRF_AkSigorta | AK Sigorta | ✅ (sat2) | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Anadolu | Anadolu | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_AnaSigorta | ANA Sigorta | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Allianz | Allianz | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Sompo | Sompo Japan | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_HDI | HDI | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Mapfre | Mapfre | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Gunes | Güneş | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Groupama | Groupama | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Zurich | Zurich | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Neova | Neova | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Eureko | Eureko | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Ergo | Ergo | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Generali | Generali | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_TurkNippon | Türk Nippon | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Ray | Ray | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Doga | Doğa | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Ankara | Ankara | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Halk | Halk | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Koru | Koru | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Orient | Orient | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Quick | Quick | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_DemirHayat | Demir Hayat | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Gulf | Gulf | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Magdeburger | Magdeburger | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Bereket | Bereket | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Corpus | Corpus | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Hepiyi | Hepiyi | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Seker | Şeker | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Turkiye | Türkiye | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |
| TRF_Unico | Unico | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ |

**Not:** ✓ = Bu alan için selector/URL gerçek portala göre kontrol edilip güncellenmeli. Şu an tüm TRF’lerde generic placeholder kullanılıyor.

---

## 4. Adım Adım Güncelleme

1. Şirket acente portalına tarayıcı ile gir; giriş ve (varsa) teklif sayfası URL’lerini not al.
2. İlgili `TRF_*.cs` dosyasında **LoginUrl** (ve teklif sayfası URL’i) güncelle.
3. Giriş sayfasında kullanıcı adı, şifre, giriş butonu için **SelectorUserName**, **SelectorPassword**, **SelectorLoginButton** değerlerini DevTools ile bulup yaz.
4. Teklif/trafik sayfasında plaka, TCKN, sorgula butonu için **SelectorPlaka**, **SelectorTckn**, **SelectorSorgulaButton** değerlerini bulup yaz.
5. Captcha varsa ilgili TRF’a **SelectorCaptcha** ekleyip LoginAsync içinde kullan (örnek: TRF_Anadolu).
6. Projeyi derleyip **Şirketler / Robot** ekranından o şirket için “Chrome ile Aç” / “Tümüne Giriş” veya teklif akışını test et.

---

## 5. Örnek (Anadolu)

Anadolu için örnek çoklu selector kullanımı (gerçek portalda id/name’e göre düzenlenebilir):

```csharp
private const string SelectorUserName = "input[name=\"kullaniciAdi\"], #txtKullaniciAdi, input[type=\"text\"][id*=\"user\"]";
private const string SelectorPassword = "input[name=\"sifre\"], #txtSifre, input[type=\"password\"]";
private const string SelectorLoginButton = "#btnGiris, button[type=\"submit\"], .giris-btn";
```

Bu yapı diğer şirketler için de aynı mantıkla uygulanır; sadece id/name/class değerleri o portala göre değişir.

---

*Bu rehber, TRF selector’larının gerçek portallara göre güncellenmesi için kullanılır. Güncelleme sonrası mutlaka manuel test yapılmalıdır.*
