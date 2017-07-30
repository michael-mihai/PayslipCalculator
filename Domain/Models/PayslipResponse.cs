namespace Domain.Models
{
    public class PayslipResponse
    {
        public string Name { get; set; }

        public string PayPeriod { get; set; } 

        public int GrossIncome { get; set; }

        public int IncomeTax { get; set; }

        public int NetIncome { get; set; }

        public int Superannuation { get; set; }
    }
}
