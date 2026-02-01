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
            Register("ak", () => new TRF_AkSigorta());
            Register("aksigorta", () => new TRF_AkSigorta());
            Register("ana", () => new TRF_AnaSigortaStub());
            Register("anasigorta", () => new TRF_AnaSigortaStub());
            Register("anadolu", () => new TRF_AnadoluStub());
            Register("anadolusigorta", () => new TRF_AnadoluStub());
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
    }
}
