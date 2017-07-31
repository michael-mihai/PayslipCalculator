using System;
using System.ComponentModel.DataAnnotations;
using Domain.Helpers;

namespace Domain.Models
{
    /// <summary>
    /// Holds payslip request data
    /// </summary>
    public class PayslipRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be a positive integer")]
        public int AnnualSalary { get; set; }

        [Required]
        [DataType(DataType.Text)]
        // [DisplayFormat(DataFormatString = "{0}'%'")]
        [Range(0, 50, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int SuperRate { get; set; }

        [Required]
        [PayslipDate(ErrorMessage = "The {0} cannot be the current or a future month.")]
        public DateTime PayStartPeriod { get; set; }
    }

}
