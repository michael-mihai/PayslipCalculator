using System.Collections.Generic;
using Domain.Models.Configuration;

namespace Domain.Helpers
{
    public class TaxHelper
    {
        public static TaxSettings DefaultSettings = new TaxSettings()
        {
            TaxRates = new List<TaxRates>()
            {
                new TaxRates()
                {
                    TaxBrackets = new List<TaxBracket>()
                    {
                        new TaxBracket() {FromIncome = 0, ToIncome = 18200, Base = 0, TaxPerDollar = 0 },
                        new TaxBracket() {FromIncome = 18200, ToIncome = 37000, Base = 0, TaxPerDollar = 0.19m },
                        new TaxBracket() {FromIncome = 37000, ToIncome = 80000, Base = 3572, TaxPerDollar = 0.325m },
                        new TaxBracket() {FromIncome = 80000, ToIncome = 180000, Base = 17547, TaxPerDollar = 0.37m },
                        new TaxBracket() {FromIncome = 180000, ToIncome = null, Base = 54547, TaxPerDollar = 0.45m }
                    }
                }
            }
        };
    }
}
