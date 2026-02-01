using System;
using System.Collections.Generic;
using System.Linq;

namespace VegaAsis.Windows.Data
{
    public class DistrictInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class CityInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int PlateCode { get; set; }
        public DistrictInfo[] Districts { get; set; }
    }

    public static class TurkeyLocations
    {
        private static readonly CityInfo[] Cities = new[]
        {
            new CityInfo { Name = "ADANA", Code = "01", PlateCode = 1, Districts = new[] {
                new DistrictInfo { Name = "ALADAĞ", Code = "0101" }, new DistrictInfo { Name = "CEYHAN", Code = "0102" },
                new DistrictInfo { Name = "ÇUKUROVA", Code = "0103" }, new DistrictInfo { Name = "FEKE", Code = "0104" },
                new DistrictInfo { Name = "İMAMOĞLU", Code = "0105" }, new DistrictInfo { Name = "KARAİSALI", Code = "0106" },
                new DistrictInfo { Name = "KARATAŞ", Code = "0107" }, new DistrictInfo { Name = "KOZAN", Code = "0108" },
                new DistrictInfo { Name = "POZANTI", Code = "0109" }, new DistrictInfo { Name = "SAİMBEYLİ", Code = "0110" },
                new DistrictInfo { Name = "SARIÇAM", Code = "0111" }, new DistrictInfo { Name = "SEYHAN", Code = "0112" },
                new DistrictInfo { Name = "TUFANBEYLİ", Code = "0113" }, new DistrictInfo { Name = "YUMURTALIK", Code = "0114" },
                new DistrictInfo { Name = "YÜREĞİR", Code = "0115" }
            }},
            new CityInfo { Name = "ANKARA", Code = "06", PlateCode = 6, Districts = new[] {
                new DistrictInfo { Name = "AKYURT", Code = "0601" }, new DistrictInfo { Name = "ALTINDAĞ", Code = "0602" },
                new DistrictInfo { Name = "AYAŞ", Code = "0603" }, new DistrictInfo { Name = "BALA", Code = "0604" },
                new DistrictInfo { Name = "BEYPAZARI", Code = "0605" }, new DistrictInfo { Name = "ÇAMLIDERE", Code = "0606" },
                new DistrictInfo { Name = "ÇANKAYA", Code = "0607" }, new DistrictInfo { Name = "ÇUBUK", Code = "0608" },
                new DistrictInfo { Name = "ELMADAĞ", Code = "0609" }, new DistrictInfo { Name = "ETİMESGUT", Code = "0610" },
                new DistrictInfo { Name = "EVREN", Code = "0611" }, new DistrictInfo { Name = "GÖLBAŞI", Code = "0612" },
                new DistrictInfo { Name = "GÜDÜL", Code = "0613" }, new DistrictInfo { Name = "HAYMANA", Code = "0614" },
                new DistrictInfo { Name = "KAHRAMANKAZAN", Code = "0615" }, new DistrictInfo { Name = "KALECİK", Code = "0616" },
                new DistrictInfo { Name = "KEÇİÖREN", Code = "0617" }, new DistrictInfo { Name = "KIZILCAHAMAM", Code = "0618" },
                new DistrictInfo { Name = "MAMAK", Code = "0619" }, new DistrictInfo { Name = "NALLIHAN", Code = "0620" },
                new DistrictInfo { Name = "POLATLI", Code = "0621" }, new DistrictInfo { Name = "PURSAKLAR", Code = "0622" },
                new DistrictInfo { Name = "SİNCAN", Code = "0623" }, new DistrictInfo { Name = "ŞEREFLİKOÇHİSAR", Code = "0624" },
                new DistrictInfo { Name = "YENİMAHALLE", Code = "0625" }
            }},
            new CityInfo { Name = "ANTALYA", Code = "07", PlateCode = 7, Districts = new[] {
                new DistrictInfo { Name = "AKSEKİ", Code = "0701" }, new DistrictInfo { Name = "AKSU", Code = "0702" },
                new DistrictInfo { Name = "ALANYA", Code = "0703" }, new DistrictInfo { Name = "DEMRE", Code = "0704" },
                new DistrictInfo { Name = "DÖŞEMEALTI", Code = "0705" }, new DistrictInfo { Name = "ELMALI", Code = "0706" },
                new DistrictInfo { Name = "FİNİKE", Code = "0707" }, new DistrictInfo { Name = "GAZİPAŞA", Code = "0708" },
                new DistrictInfo { Name = "GÜNDOĞMUŞ", Code = "0709" }, new DistrictInfo { Name = "İBRADI", Code = "0710" },
                new DistrictInfo { Name = "KAŞ", Code = "0711" }, new DistrictInfo { Name = "KEMER", Code = "0712" },
                new DistrictInfo { Name = "KEPEZ", Code = "0713" }, new DistrictInfo { Name = "KONYAALTI", Code = "0714" },
                new DistrictInfo { Name = "KORKUTELİ", Code = "0715" }, new DistrictInfo { Name = "KUMLUCA", Code = "0716" },
                new DistrictInfo { Name = "MANAVGAT", Code = "0717" }, new DistrictInfo { Name = "MURATPAŞA", Code = "0718" },
                new DistrictInfo { Name = "SERİK", Code = "0719" }
            }},
            new CityInfo { Name = "BURSA", Code = "16", PlateCode = 16, Districts = new[] {
                new DistrictInfo { Name = "BÜYÜKORHAN", Code = "1601" }, new DistrictInfo { Name = "GEMLİK", Code = "1602" },
                new DistrictInfo { Name = "GÜRSU", Code = "1603" }, new DistrictInfo { Name = "HARMANCIK", Code = "1604" },
                new DistrictInfo { Name = "İNEGÖL", Code = "1605" }, new DistrictInfo { Name = "İZNİK", Code = "1606" },
                new DistrictInfo { Name = "KARACABEY", Code = "1607" }, new DistrictInfo { Name = "KELES", Code = "1608" },
                new DistrictInfo { Name = "KESTEL", Code = "1609" }, new DistrictInfo { Name = "MUDANYA", Code = "1610" },
                new DistrictInfo { Name = "MUSTAFAKEMALPAŞA", Code = "1611" }, new DistrictInfo { Name = "NİLÜFER", Code = "1612" },
                new DistrictInfo { Name = "ORHANELİ", Code = "1613" }, new DistrictInfo { Name = "ORHANGAZİ", Code = "1614" },
                new DistrictInfo { Name = "OSMANGAZİ", Code = "1615" }, new DistrictInfo { Name = "YENİŞEHİR", Code = "1616" },
                new DistrictInfo { Name = "YILDIRIM", Code = "1617" }
            }},
            new CityInfo { Name = "İSTANBUL", Code = "34", PlateCode = 34, Districts = new[] {
                new DistrictInfo { Name = "ADALAR", Code = "3401" }, new DistrictInfo { Name = "ARNAVUTKÖY", Code = "3402" },
                new DistrictInfo { Name = "ATAŞEHİR", Code = "3403" }, new DistrictInfo { Name = "AVCILAR", Code = "3404" },
                new DistrictInfo { Name = "BAĞCILAR", Code = "3405" }, new DistrictInfo { Name = "BAHÇELİEVLER", Code = "3406" },
                new DistrictInfo { Name = "BAKIRKÖY", Code = "3407" }, new DistrictInfo { Name = "BAŞAKŞEHİR", Code = "3408" },
                new DistrictInfo { Name = "BAYRAMPAŞA", Code = "3409" }, new DistrictInfo { Name = "BEŞİKTAŞ", Code = "3410" },
                new DistrictInfo { Name = "BEYKOZ", Code = "3411" }, new DistrictInfo { Name = "BEYLİKDÜZÜ", Code = "3412" },
                new DistrictInfo { Name = "BEYOĞLU", Code = "3413" }, new DistrictInfo { Name = "BÜYÜKÇEKMECE", Code = "3414" },
                new DistrictInfo { Name = "ÇATALCA", Code = "3415" }, new DistrictInfo { Name = "ÇEKMEKÖY", Code = "3416" },
                new DistrictInfo { Name = "ESENLER", Code = "3417" }, new DistrictInfo { Name = "ESENYURT", Code = "3418" },
                new DistrictInfo { Name = "EYÜPSULTAN", Code = "3419" }, new DistrictInfo { Name = "FATİH", Code = "3420" },
                new DistrictInfo { Name = "GAZİOSMANPAŞA", Code = "3421" }, new DistrictInfo { Name = "GÜNGÖREN", Code = "3422" },
                new DistrictInfo { Name = "KADIKÖY", Code = "3423" }, new DistrictInfo { Name = "KAĞITHANE", Code = "3424" },
                new DistrictInfo { Name = "KARTAL", Code = "3425" }, new DistrictInfo { Name = "KÜÇÜKÇEKMECE", Code = "3426" },
                new DistrictInfo { Name = "MALTEPE", Code = "3427" }, new DistrictInfo { Name = "PENDİK", Code = "3428" },
                new DistrictInfo { Name = "SANCAKTEPE", Code = "3429" }, new DistrictInfo { Name = "SARIYER", Code = "3430" },
                new DistrictInfo { Name = "SİLİVRİ", Code = "3431" }, new DistrictInfo { Name = "SULTANBEYLİ", Code = "3432" },
                new DistrictInfo { Name = "SULTANGAZİ", Code = "3433" }, new DistrictInfo { Name = "ŞİLE", Code = "3434" },
                new DistrictInfo { Name = "ŞİŞLİ", Code = "3435" }, new DistrictInfo { Name = "TUZLA", Code = "3436" },
                new DistrictInfo { Name = "ÜMRANİYE", Code = "3437" }, new DistrictInfo { Name = "ÜSKÜDAR", Code = "3438" },
                new DistrictInfo { Name = "ZEYTİNBURNU", Code = "3439" }
            }},
            new CityInfo { Name = "İZMİR", Code = "35", PlateCode = 35, Districts = new[] {
                new DistrictInfo { Name = "ALİAĞA", Code = "3501" }, new DistrictInfo { Name = "BALÇOVA", Code = "3502" },
                new DistrictInfo { Name = "BAYINDIR", Code = "3503" }, new DistrictInfo { Name = "BAYRAKLI", Code = "3504" },
                new DistrictInfo { Name = "BERGAMA", Code = "3505" }, new DistrictInfo { Name = "BEYDAĞ", Code = "3506" },
                new DistrictInfo { Name = "BORNOVA", Code = "3507" }, new DistrictInfo { Name = "BUCA", Code = "3508" },
                new DistrictInfo { Name = "ÇEŞME", Code = "3509" }, new DistrictInfo { Name = "ÇİĞLİ", Code = "3510" },
                new DistrictInfo { Name = "DİKİLİ", Code = "3511" }, new DistrictInfo { Name = "FOÇA", Code = "3512" },
                new DistrictInfo { Name = "GAZİEMİR", Code = "3513" }, new DistrictInfo { Name = "GÜZELBAHÇE", Code = "3514" },
                new DistrictInfo { Name = "KARABAĞLAR", Code = "3515" }, new DistrictInfo { Name = "KARABURUN", Code = "3516" },
                new DistrictInfo { Name = "KARŞIYAKA", Code = "3517" }, new DistrictInfo { Name = "KEMALPAŞA", Code = "3518" },
                new DistrictInfo { Name = "KİNİK", Code = "3519" }, new DistrictInfo { Name = "KİRAZ", Code = "3520" },
                new DistrictInfo { Name = "KONAK", Code = "3521" }, new DistrictInfo { Name = "MENDERES", Code = "3522" },
                new DistrictInfo { Name = "MENEMEN", Code = "3523" }, new DistrictInfo { Name = "NARLIDERE", Code = "3524" },
                new DistrictInfo { Name = "ÖDEMİŞ", Code = "3525" }, new DistrictInfo { Name = "SEFERİHİSAR", Code = "3526" },
                new DistrictInfo { Name = "SELÇUK", Code = "3527" }, new DistrictInfo { Name = "TİRE", Code = "3528" },
                new DistrictInfo { Name = "TORBALI", Code = "3529" }, new DistrictInfo { Name = "URLA", Code = "3530" }
            }},
            new CityInfo { Name = "KOCAELİ", Code = "41", PlateCode = 41, Districts = new[] {
                new DistrictInfo { Name = "BAŞİSKELE", Code = "4101" }, new DistrictInfo { Name = "ÇAYIROVA", Code = "4102" },
                new DistrictInfo { Name = "DARICA", Code = "4103" }, new DistrictInfo { Name = "DERİNCE", Code = "4104" },
                new DistrictInfo { Name = "DİLOVASI", Code = "4105" }, new DistrictInfo { Name = "GEBZE", Code = "4106" },
                new DistrictInfo { Name = "GÖLCÜK", Code = "4107" }, new DistrictInfo { Name = "İZMİT", Code = "4108" },
                new DistrictInfo { Name = "KANDIRA", Code = "4109" }, new DistrictInfo { Name = "KARAMÜRSEL", Code = "4110" },
                new DistrictInfo { Name = "KARTEPE", Code = "4111" }, new DistrictInfo { Name = "KÖRFEZ", Code = "4112" }
            }},
            new CityInfo { Name = "KONYA", Code = "42", PlateCode = 42, Districts = new[] {
                new DistrictInfo { Name = "AHIRLI", Code = "4201" }, new DistrictInfo { Name = "AKÖREN", Code = "4202" },
                new DistrictInfo { Name = "AKŞEHİR", Code = "4203" }, new DistrictInfo { Name = "ALTINEKİN", Code = "4204" },
                new DistrictInfo { Name = "BEYŞEHİR", Code = "4205" }, new DistrictInfo { Name = "BOZKIR", Code = "4206" },
                new DistrictInfo { Name = "CİHANBEYLİ", Code = "4207" }, new DistrictInfo { Name = "ÇELTİK", Code = "4208" },
                new DistrictInfo { Name = "ÇUMRA", Code = "4209" }, new DistrictInfo { Name = "DERBENT", Code = "4210" },
                new DistrictInfo { Name = "DEREBUCAK", Code = "4211" }, new DistrictInfo { Name = "DOĞANHİSAR", Code = "4212" },
                new DistrictInfo { Name = "EMİRGAZİ", Code = "4213" }, new DistrictInfo { Name = "EREĞLİ", Code = "4214" },
                new DistrictInfo { Name = "GÜNEYSINIR", Code = "4215" }, new DistrictInfo { Name = "HADİM", Code = "4216" },
                new DistrictInfo { Name = "HALKAPINAR", Code = "4217" }, new DistrictInfo { Name = "HÜYÜK", Code = "4218" },
                new DistrictInfo { Name = "ILGIN", Code = "4219" }, new DistrictInfo { Name = "KADINHANI", Code = "4220" },
                new DistrictInfo { Name = "KARAPINAR", Code = "4221" }, new DistrictInfo { Name = "KARATAY", Code = "4222" },
                new DistrictInfo { Name = "KULU", Code = "4223" }, new DistrictInfo { Name = "MERAM", Code = "4224" },
                new DistrictInfo { Name = "SARAYÖNÜ", Code = "4225" }, new DistrictInfo { Name = "SELÇUKLU", Code = "4226" },
                new DistrictInfo { Name = "SEYDİŞEHİR", Code = "4227" }, new DistrictInfo { Name = "TAŞKENT", Code = "4228" },
                new DistrictInfo { Name = "TUZLUKÇU", Code = "4229" }, new DistrictInfo { Name = "YALIHÜYÜK", Code = "4230" },
                new DistrictInfo { Name = "YUNAK", Code = "4231" }
            }},
            new CityInfo { Name = "MANİSA", Code = "45", PlateCode = 45, Districts = new[] {
                new DistrictInfo { Name = "AHMETLİ", Code = "4501" }, new DistrictInfo { Name = "AKHİSAR", Code = "4502" },
                new DistrictInfo { Name = "ALAŞEHİR", Code = "4503" }, new DistrictInfo { Name = "DEMİRCİ", Code = "4504" },
                new DistrictInfo { Name = "GÖLMARMARA", Code = "4505" }, new DistrictInfo { Name = "GÖRDES", Code = "4506" },
                new DistrictInfo { Name = "KIRKAĞAÇ", Code = "4507" }, new DistrictInfo { Name = "KÖPRÜBAŞI", Code = "4508" },
                new DistrictInfo { Name = "KULA", Code = "4509" }, new DistrictInfo { Name = "SALİHLİ", Code = "4510" },
                new DistrictInfo { Name = "SARIGÖL", Code = "4511" }, new DistrictInfo { Name = "SARUHANLI", Code = "4512" },
                new DistrictInfo { Name = "SELENDİ", Code = "4513" }, new DistrictInfo { Name = "SOMA", Code = "4514" },
                new DistrictInfo { Name = "ŞEHZADELER", Code = "4515" }, new DistrictInfo { Name = "TURGUTLU", Code = "4516" },
                new DistrictInfo { Name = "YUNUSEMRE", Code = "4517" }
            }},
            new CityInfo { Name = "MUĞLA", Code = "48", PlateCode = 48, Districts = new[] {
                new DistrictInfo { Name = "BODRUM", Code = "4801" }, new DistrictInfo { Name = "DALAMAN", Code = "4802" },
                new DistrictInfo { Name = "DATÇA", Code = "4803" }, new DistrictInfo { Name = "FETHİYE", Code = "4804" },
                new DistrictInfo { Name = "KAVAKLIDERE", Code = "4805" }, new DistrictInfo { Name = "KÖYCEĞİZ", Code = "4806" },
                new DistrictInfo { Name = "MARMARİS", Code = "4807" }, new DistrictInfo { Name = "MENTEŞE", Code = "4808" },
                new DistrictInfo { Name = "MİLAS", Code = "4809" }, new DistrictInfo { Name = "ORTACA", Code = "4810" },
                new DistrictInfo { Name = "SEYDİKEMER", Code = "4811" }, new DistrictInfo { Name = "ULA", Code = "4812" },
                new DistrictInfo { Name = "YATAĞAN", Code = "4813" }
            }},
            new CityInfo { Name = "SAKARYA", Code = "54", PlateCode = 54, Districts = new[] {
                new DistrictInfo { Name = "ADAPAZARI", Code = "5401" }, new DistrictInfo { Name = "AKYAZI", Code = "5402" },
                new DistrictInfo { Name = "ARİFİYE", Code = "5403" }, new DistrictInfo { Name = "ERENLER", Code = "5404" },
                new DistrictInfo { Name = "FERİZLİ", Code = "5405" }, new DistrictInfo { Name = "GEYVE", Code = "5406" },
                new DistrictInfo { Name = "HENDEK", Code = "5407" }, new DistrictInfo { Name = "KARAPÜRÇEK", Code = "5408" },
                new DistrictInfo { Name = "KARASU", Code = "5409" }, new DistrictInfo { Name = "KAYNARCA", Code = "5410" },
                new DistrictInfo { Name = "KOCAALİ", Code = "5411" }, new DistrictInfo { Name = "PAMUKOVA", Code = "5412" },
                new DistrictInfo { Name = "SAPANCA", Code = "5413" }, new DistrictInfo { Name = "SERDİVAN", Code = "5414" },
                new DistrictInfo { Name = "SÖĞÜTLÜ", Code = "5415" }, new DistrictInfo { Name = "TARAKLI", Code = "5416" }
            }},
            new CityInfo { Name = "GAZİANTEP", Code = "27", PlateCode = 27, Districts = new[] {
                new DistrictInfo { Name = "ŞAHİNBEY", Code = "2701" }, new DistrictInfo { Name = "ŞEHİTKAMİL", Code = "2702" },
                new DistrictInfo { Name = "NİZİP", Code = "2703" }, new DistrictInfo { Name = "İSLAHİYE", Code = "2704" }
            }},
            new CityInfo { Name = "MERSİN", Code = "33", PlateCode = 33, Districts = new[] {
                new DistrictInfo { Name = "MEZİTLİ", Code = "3301" }, new DistrictInfo { Name = "YENİŞEHİR", Code = "3302" },
                new DistrictInfo { Name = "TOROSLAR", Code = "3303" }, new DistrictInfo { Name = "AKDENİZ", Code = "3304" }
            }},
            new CityInfo { Name = "KAYSERİ", Code = "38", PlateCode = 38, Districts = new[] {
                new DistrictInfo { Name = "MELİKGAZİ", Code = "3801" }, new DistrictInfo { Name = "KOCASİNAN", Code = "3802" },
                new DistrictInfo { Name = "TALAS", Code = "3803" }
            }},
            new CityInfo { Name = "HATAY", Code = "31", PlateCode = 31, Districts = new[] {
                new DistrictInfo { Name = "ANTAKYA", Code = "3101" }, new DistrictInfo { Name = "İSKENDERUN", Code = "3102" }
            }},
            new CityInfo { Name = "SAMSUN", Code = "55", PlateCode = 55, Districts = new[] {
                new DistrictInfo { Name = "İLKADIM", Code = "5501" }, new DistrictInfo { Name = "ATAKUM", Code = "5502" }
            }},
            new CityInfo { Name = "BALIKESİR", Code = "10", PlateCode = 10, Districts = new[] {
                new DistrictInfo { Name = "ALTIEYLÜL", Code = "1001" }, new DistrictInfo { Name = "KARESİ", Code = "1002" }
            }},
            new CityInfo { Name = "DENİZLİ", Code = "20", PlateCode = 20, Districts = new[] {
                new DistrictInfo { Name = "PAMUKKALE", Code = "2001" }, new DistrictInfo { Name = "MERKEZEFENDİ", Code = "2002" }
            }},
            new CityInfo { Name = "TEKİRDAĞ", Code = "59", PlateCode = 59, Districts = new[] {
                new DistrictInfo { Name = "SÜLEYMANPAŞA", Code = "5901" }, new DistrictInfo { Name = "ÇORLU", Code = "5902" }
            }},
            new CityInfo { Name = "AYDIN", Code = "09", PlateCode = 9, Districts = new[] {
                new DistrictInfo { Name = "EFELER", Code = "0901" }, new DistrictInfo { Name = "NAZİLLİ", Code = "0902" }
            }},
            new CityInfo { Name = "TRABZON", Code = "61", PlateCode = 61, Districts = new[] {
                new DistrictInfo { Name = "ORTAHİSAR", Code = "6101" }, new DistrictInfo { Name = "AKÇAABAT", Code = "6102" }
            }}
        };

        public static string[] GetCityNames()
        {
            var names = new string[Cities.Length];
            for (int i = 0; i < Cities.Length; i++)
            {
                names[i] = Cities[i].Name;
            }
            return names;
        }

        public static string[] GetDistrictsByCity(string cityName)
        {
            var city = Cities.FirstOrDefault(c => string.Equals(c.Name, cityName, StringComparison.OrdinalIgnoreCase));
            if (city == null || city.Districts == null)
            {
                return new string[0];
            }
            var names = new string[city.Districts.Length];
            for (int i = 0; i < city.Districts.Length; i++)
            {
                names[i] = city.Districts[i].Name;
            }
            return names;
        }
    }
}
