using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Helpers
{
    public class PayslipDateAttribute : RangeAttribute
    {
        public PayslipDateAttribute()
            : base(typeof(DateTime), DateTime.MinValue.ToString("yyyy-MM-dd"), new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1).ToString("yyyy-MM-dd")) { }
    }
}
