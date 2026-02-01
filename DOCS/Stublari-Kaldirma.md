# Stub Servisleri Nasıl Kaldırılır

DASK, Kasko, İMM, Konut, TSS servisleri şu an **stub** (sahte veri) ile kayıtlı. Gerçek veri (veritabanı veya dış API) kullanmak için aşağıdaki adımları uygulayın.

## Genel mantık

- **Arayüzler (IDaskService, IKaskoService, …) aynen kalır.** Formlar ve diğer kodlar arayüze bağlı olduğu için değişmez.
- Sadece **uygulama sınıfı** değişir: `XxxServiceStub` yerine gerçek veri kullanan `XxxService` yazılır ve DI’da o kaydedilir.
- İsterseniz stub dosyalarını silebilir veya test/yedek için projede bırakabilirsiniz.

---

## Adım 1: Gerçek servis sınıfını yazın

`VegaAsis.Data/Services/` altında yeni bir sınıf oluşturun (örn. `DaskService.cs`) ve ilgili arayüzü uygulayın.

**Seçenek A – Veritabanı:**  
Teminatlar için tablo/entity varsa (örn. `DaskTeminat`), EF ile sorgulayıp DTO’ya çevirin.

**Seçenek B – Dış API:**  
Şirket/merkez API’sine HTTP isteği atıp gelen veriyi DTO listesine dönüştürün.

**Seçenek C – Henüz veri yok:**  
Şimdilik boş liste döndüren bir servis yazın; veri kaynağı hazır olunca içini doldurursunuz.

Örnek (veri yokken boş liste):

```csharp
// VegaAsis.Data/Services/DaskService.cs
public class DaskService : IDaskService
{
    public Task<IReadOnlyList<DaskTeminatDto>> GetTeminatlarAsync(string il = null, string ilce = null)
    {
        // İleride: _db.DaskTeminatlar.Where(...).Select(ToDto).ToListAsync()
        return Task.FromResult<IReadOnlyList<DaskTeminatDto>>(new List<DaskTeminatDto>());
    }
}
```

---

## Adım 2: Program.cs’te stub yerine gerçek servisi kaydedin

`Program.cs` içinde Autofac kayıtlarında **Stub** yerine **gerçek** sınıfı kullanın:

```csharp
// Eski (stub):
builder.RegisterType<DaskServiceStub>().As<IDaskService>().InstancePerLifetimeScope();

// Yeni (gerçek):
builder.RegisterType<DaskService>().As<IDaskService>().InstancePerLifetimeScope();
```

Aynı şekilde diğerleri:

- `KaskoServiceStub` → `KaskoService`
- `ImmServiceStub` → `ImmService`
- `KonutServiceStub` → `KonutService`
- `TssServiceStub` → `TssService`

---

## Adım 3: Stub dosyalarını kaldırın (isteğe bağlı)

Gerçek servisler çalıştığından emin olduktan sonra:

1. **VegaAsis.Data.csproj** içinde ilgili stub `Compile Include` satırlarını silin (örn. `DaskServiceStub.cs`).
2. Dosyayı diskten silebilirsiniz: `Services/DaskServiceStub.cs` vb.

Stubları unit test veya yedek için kullanacaksanız projede bırakıp sadece DI kaydını değiştirmeniz yeterli.

---

## Özet tablo

| Servis    | Gerçek sınıf        | Durum |
|----------|---------------------|--------|
| DASK     | DaskService.cs      | ✅ Stub kaldırıldı; boş liste (API/DB eklenebilir) |
| Kasko    | KaskoService.cs     | ✅ Stub kaldırıldı; boş liste |
| İMM      | ImmService.cs       | ✅ Stub kaldırıldı; boş liste |
| Konut    | KonutService.cs     | ✅ Stub kaldırıldı; boş liste |
| TSS      | TssService.cs       | ✅ Stub kaldırıldı; boş liste |

Tüm stub sınıflar silindi. Gerçek veri için her servisin `GetTeminatlarAsync` / `GetAileBireyleriAsync` içine veritabanı veya API çağrısı eklenebilir.
