using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace VegaAsis.Windows.Data
{
    public class CompanyLogo
    {
        public string CompanyCode { get; set; }
        public string DisplayText { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
    }

    public static class CompanyLogoData
    {
        public static Dictionary<string, CompanyLogo> Logos = new Dictionary<string, CompanyLogo>
        {
            { "AK SIGORTA", new CompanyLogo { CompanyCode = "AK SIGORTA", DisplayText = "AK", BackgroundColor = Color.FromArgb(139, 0, 0), ForegroundColor = Color.White } },
            { "ALLIANZ", new CompanyLogo { CompanyCode = "ALLIANZ", DisplayText = "AL", BackgroundColor = Color.FromArgb(0, 55, 129), ForegroundColor = Color.White } },
            { "ANADOLU", new CompanyLogo { CompanyCode = "ANADOLU", DisplayText = "A", BackgroundColor = Color.FromArgb(227, 6, 19), ForegroundColor = Color.White } },
            { "AXA", new CompanyLogo { CompanyCode = "AXA", DisplayText = "AXA", BackgroundColor = Color.FromArgb(0, 0, 143), ForegroundColor = Color.White } },
            { "CORPUS", new CompanyLogo { CompanyCode = "CORPUS", DisplayText = "C", BackgroundColor = Color.FromArgb(247, 148, 29), ForegroundColor = Color.White } },
            { "DOGA", new CompanyLogo { CompanyCode = "DOGA", DisplayText = "D", BackgroundColor = Color.FromArgb(0, 150, 57), ForegroundColor = Color.White } },
            { "HEPIYI", new CompanyLogo { CompanyCode = "HEPIYI", DisplayText = "iyi", BackgroundColor = Color.FromArgb(233, 30, 140), ForegroundColor = Color.White } },
            { "NEOVA", new CompanyLogo { CompanyCode = "NEOVA", DisplayText = "N", BackgroundColor = Color.FromArgb(0, 160, 227), ForegroundColor = Color.White } },
            { "QUICK_PORT", new CompanyLogo { CompanyCode = "QUICK_PORT", DisplayText = "Q", BackgroundColor = Color.FromArgb(255, 102, 0), ForegroundColor = Color.White } },
            { "RAY", new CompanyLogo { CompanyCode = "RAY", DisplayText = "R", BackgroundColor = Color.FromArgb(237, 28, 36), ForegroundColor = Color.White } },
            { "SOMPO", new CompanyLogo { CompanyCode = "SOMPO", DisplayText = "S", BackgroundColor = Color.FromArgb(29, 32, 136), ForegroundColor = Color.White } },
            { "HDI", new CompanyLogo { CompanyCode = "HDI", DisplayText = "HDI", BackgroundColor = Color.FromArgb(0, 99, 65), ForegroundColor = Color.White } },
            { "HDI PLUS", new CompanyLogo { CompanyCode = "HDI PLUS", DisplayText = "HDI", BackgroundColor = Color.FromArgb(0, 99, 65), ForegroundColor = Color.White } },
            { "ATLAS", new CompanyLogo { CompanyCode = "ATLAS", DisplayText = "AT", BackgroundColor = Color.FromArgb(0, 102, 179), ForegroundColor = Color.White } },
            { "TURKNIPPON", new CompanyLogo { CompanyCode = "TURKNIPPON", DisplayText = "TN", BackgroundColor = Color.FromArgb(200, 16, 46), ForegroundColor = Color.White } },
            { "ZURICH", new CompanyLogo { CompanyCode = "ZURICH", DisplayText = "Z", BackgroundColor = Color.FromArgb(33, 103, 174), ForegroundColor = Color.White } },
            { "ANA SIGORTA", new CompanyLogo { CompanyCode = "ANA SIGORTA", DisplayText = "AS", BackgroundColor = Color.FromArgb(0, 75, 135), ForegroundColor = Color.White } },
            { "HEDVA", new CompanyLogo { CompanyCode = "HEDVA", DisplayText = "H", BackgroundColor = Color.FromArgb(107, 45, 91), ForegroundColor = Color.White } },
            { "ACIBADEM", new CompanyLogo { CompanyCode = "ACIBADEM", DisplayText = "AC", BackgroundColor = Color.FromArgb(0, 120, 215), ForegroundColor = Color.White } },
        };

        public static Bitmap CreateLogo(string companyCode, int size = 20)
        {
            if (string.IsNullOrEmpty(companyCode))
            {
                return CreateDefaultLogo(size);
            }

            var key = companyCode.ToUpper();
            if (!Logos.TryGetValue(key, out var logo))
            {
                logo = new CompanyLogo
                {
                    DisplayText = companyCode.Length > 0 ? companyCode.Substring(0, Math.Min(2, companyCode.Length)).ToUpper() : "?",
                    BackgroundColor = Color.FromArgb(120, 120, 120),
                    ForegroundColor = Color.White
                };
            }

            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.FillRectangle(new SolidBrush(logo.BackgroundColor), 0, 0, size, size);

                var fontSize = logo.DisplayText.Length > 2 ? size * 0.35f : size * 0.45f;
                using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold))
                using (var brush = new SolidBrush(logo.ForegroundColor))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(logo.DisplayText, font, brush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bitmap;
        }

        private static Bitmap CreateDefaultLogo(int size)
        {
            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillRectangle(Brushes.LightGray, 0, 0, size, size);
                using (var font = new Font("Segoe UI", size * 0.5f, FontStyle.Bold))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("?", font, Brushes.White, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bitmap;
        }
    }
}
