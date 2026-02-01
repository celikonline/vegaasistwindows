namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Tek bir şirket için toplu giriş (AllLogins) sonucu.
    /// </summary>
    public class AllLoginResult
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
