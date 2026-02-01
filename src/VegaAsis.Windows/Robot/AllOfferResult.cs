namespace VegaAsis.Windows.Robot
{
    /// <summary>
    /// Tek bir şirket için toplu teklif (AllOffers) sonucu.
    /// </summary>
    public class AllOfferResult
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool LoginSuccess { get; set; }
        public string OfferResult { get; set; }
    }
}
