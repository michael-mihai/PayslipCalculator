namespace Domain.Models.Configuration
{
    public class TaxBracket
    {
        public int FromIncome { get; set; }

        public int? ToIncome { get; set; }

        public int Base { get; set; }

        public decimal TaxPerDollar { get; set; }
    }
}