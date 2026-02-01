using System;
using System.Collections.Generic;
using System.Linq;

namespace VegaAsis.Windows.Data
{
    public class VehicleTypeInfo
    {
        public string Code { get; set; }
        public string Display { get; set; }
    }

    public class VehicleBrandInfo
    {
        public string Code { get; set; }
        public string Display { get; set; }
        public VehicleTypeInfo[] Types { get; set; }
    }

    public static class VehicleBrandsAndTypes
    {
        private static readonly VehicleBrandInfo[] Brands = new[]
        {
            new VehicleBrandInfo
            {
                Code = "59",
                Display = "59 - FORD/OTOSAN",
                Types = new[]
                {
                    new VehicleTypeInfo { Code = "724", Display = "724 - T.CONNECT KOMBİ K210S" },
                    new VehicleTypeInfo { Code = "725", Display = "725 - FIESTA" },
                    new VehicleTypeInfo { Code = "726", Display = "726 - FOCUS" }
                }
            },
            new VehicleBrandInfo
            {
                Code = "1",
                Display = "1 - FIAT",
                Types = new[]
                {
                    new VehicleTypeInfo { Code = "101", Display = "101 - DOBLO" },
                    new VehicleTypeInfo { Code = "102", Display = "102 - EGEBACK" },
                    new VehicleTypeInfo { Code = "103", Display = "103 - LINEA" }
                }
            },
            new VehicleBrandInfo
            {
                Code = "12",
                Display = "12 - TOYOTA",
                Types = new[]
                {
                    new VehicleTypeInfo { Code = "201", Display = "201 - COROLLA" },
                    new VehicleTypeInfo { Code = "202", Display = "202 - YARIS" },
                    new VehicleTypeInfo { Code = "203", Display = "203 - AURIS" }
                }
            },
            new VehicleBrandInfo
            {
                Code = "34",
                Display = "34 - RENAULT",
                Types = new[]
                {
                    new VehicleTypeInfo { Code = "301", Display = "301 - MEGANE" },
                    new VehicleTypeInfo { Code = "302", Display = "302 - FLUENCE" },
                    new VehicleTypeInfo { Code = "303", Display = "303 - CLIO" }
                }
            },
            new VehicleBrandInfo
            {
                Code = "37",
                Display = "37 - HYUNDAI",
                Types = new[]
                {
                    new VehicleTypeInfo { Code = "401", Display = "401 - i20" },
                    new VehicleTypeInfo { Code = "402", Display = "402 - i30" },
                    new VehicleTypeInfo { Code = "403", Display = "403 - TUCSON" }
                }
            }
        };

        public static string[] GetBrandDisplays()
        {
            return Brands.Select(b => b.Display).ToArray();
        }

        public static string[] GetTypesByBrandDisplay(string brandDisplay)
        {
            var brand = Brands.FirstOrDefault(b => string.Equals(b.Display, brandDisplay, StringComparison.OrdinalIgnoreCase));
            if (brand?.Types == null || brand.Types.Length == 0)
            {
                return new[] { "Seçiniz" };
            }
            return brand.Types.Select(t => t.Display).ToArray();
        }
    }
}
