using System;
using System.Collections.Generic;

namespace Domain.Models.Configuration
{
    public class TaxRates
    {
        public TaxRates()
        {
            TaxBrackets = new List<TaxBracket>();
        }

        public DateTime? ApplyFrom { get; set; }

        public DateTime? ApplyTo { get; set; }

        public List<TaxBracket> TaxBrackets { get; set; }
    }
}
