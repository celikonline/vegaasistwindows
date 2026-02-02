using System;
using System.Collections.Generic;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Şirket kodu/adı → ICompanyRobot eşlemesi. Faz 4.2/4.6 – şirket robotları kayıt (AK, Anadolu, ANA gerçek akış).
    /// </summary>
    public static class CompanyRobotRegistry
    {
        private static readonly Dictionary<string, Func<ICompanyRobot>> _factories = new Dictionary<string, Func<ICompanyRobot>>(StringComparer.OrdinalIgnoreCase);

        static CompanyRobotRegistry()
        {
            // AK Sigorta
            Register("ak", () => new TRF_AkSigorta());
            Register("aksigorta", () => new TRF_AkSigorta());

            // ANA Sigorta
            Register("ana", () => new TRF_AnaSigorta());
            Register("anasigorta", () => new TRF_AnaSigorta());

            // Anadolu Sigorta
            Register("anadolu", () => new TRF_Anadolu());
            Register("anadolusigorta", () => new TRF_Anadolu());

            // Allianz Sigorta
            Register("allianz", () => new TRF_Allianz());
            Register("allianzsigorta", () => new TRF_Allianz());

            // Sompo Japan Sigorta
            Register("sompo", () => new TRF_Sompo());
            Register("sompojapan", () => new TRF_Sompo());
            Register("somposigorta", () => new TRF_Sompo());

            // HDI Sigorta
            Register("hdi", () => new TRF_HDI());
            Register("hdisigorta", () => new TRF_HDI());

            // Mapfre Sigorta
            Register("mapfre", () => new TRF_Mapfre());
            Register("mapfresigorta", () => new TRF_Mapfre());

            // Güneş Sigorta
            Register("gunes", () => new TRF_Gunes());
            Register("gunessigorta", () => new TRF_Gunes());

            // Groupama Sigorta
            Register("groupama", () => new TRF_Groupama());
            Register("groupamasigorta", () => new TRF_Groupama());

            // Zurich Sigorta
            Register("zurich", () => new TRF_Zurich());
            Register("zurichsigorta", () => new TRF_Zurich());

            // Neova Sigorta
            Register("neova", () => new TRF_Neova());
            Register("neovasigorta", () => new TRF_Neova());

            // Öncelik 2 Şirketleri (11-30)
            Register("eureko", () => new TRF_Eureko());
            Register("ergo", () => new TRF_Ergo());
            Register("generali", () => new TRF_Generali());
            Register("turknippon", () => new TRF_TurkNippon());
            Register("ray", () => new TRF_Ray());
            Register("doga", () => new TRF_Doga());
            Register("ankara", () => new TRF_Ankara());
            Register("halk", () => new TRF_Halk());
            Register("koru", () => new TRF_Koru());
            Register("orient", () => new TRF_Orient());
            Register("quick", () => new TRF_Quick());
            Register("demirhayat", () => new TRF_DemirHayat());
            Register("gulf", () => new TRF_Gulf());
            Register("magdeburger", () => new TRF_Magdeburger());
            Register("bereket", () => new TRF_Bereket());
            Register("corpus", () => new TRF_Corpus());
            Register("hepiyi", () => new TRF_Hepiyi());
            Register("seker", () => new TRF_Seker());
            Register("turkiye", () => new TRF_Turkiye());
            Register("unico", () => new TRF_Unico());
        }

        /// <summary>Şirket robotu fabrikasını kaydeder.</summary>
        public static void Register(string companyIdOrName, Func<ICompanyRobot> factory)
        {
            if (string.IsNullOrWhiteSpace(companyIdOrName) || factory == null) return;
            _factories[companyIdOrName.Trim()] = factory;
        }

        /// <summary>Şirket kodu veya adına göre robot örneği döndürür; yoksa null.</summary>
        public static ICompanyRobot GetRobot(string companyIdOrName)
        {
            if (string.IsNullOrWhiteSpace(companyIdOrName)) return null;
            var key = companyIdOrName.Trim();
            Func<ICompanyRobot> factory;
            return _factories.TryGetValue(key, out factory) ? factory() : null;
        }

        /// <summary>Kayıtlı tüm şirket kodlarını döndürür.</summary>
        public static IEnumerable<string> GetRegisteredCompanyIds()
        {
            return _factories.Keys;
        }

        /// <summary>Tekil şirket ID'lerini döndürür (alias'lar hariç).</summary>
        public static IEnumerable<string> GetAllCompanyIds()
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _factories.Keys)
            {
                var robot = _factories[key]();
                if (robot != null && !seen.Contains(robot.CompanyId))
                {
                    seen.Add(robot.CompanyId);
                    yield return robot.CompanyId;
                }
            }
        }
    }
}
