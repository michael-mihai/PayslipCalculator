using System.Collections.Generic;

namespace Domain.Models.Configuration
{
    /// <summary>
    /// use this class to provide different tax brackets for different time periods.
    /// For example, assuming the tax rates will change from 1st of March 2018, 
    /// modify the appsettings.json to include the new tax rates and set the ApplyFrom and ApplyTo values correspondingly
    /// </summary>
    public class TaxSettings
    {
        public IList<TaxRates> TaxRates { get; set; }
    }
}