using System.Threading.Tasks;

namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Captcha görselini 3. parti servise gönderip metin çözümü almak için arayüz.
    /// </summary>
    public interface ICaptchaResolver
    {
        /// <summary>Captcha görselini (byte[]) çözer; sonuç metnini döndürür.</summary>
        Task<string> SolveAsync(byte[] captchaImageBytes);

        /// <summary>Captcha görselini (Base64 string) çözer; sonuç metnini döndürür.</summary>
        Task<string> SolveAsync(string imageBase64);
    }
}
