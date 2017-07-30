using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Domain.Helpers;
using Domain.Models;
using Domain.Models.Configuration;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    public class PayslipService : IPayslipService
    {
        private readonly TaxSettings _taxSettings;
        private readonly ILogger _logger;

        public PayslipService(TaxSettings settings, ILogger<PayslipService> logger)
        {
            _logger = logger;
            _taxSettings = (settings?.TaxRates == null || !settings.TaxRates.Any()) 
                ? TaxHelper.DefaultSettings
                : settings;
        }

        public PayslipResponse GeneratePayslip(PayslipRequest payslipRequest)
        {
            // validate request
            if (!IsValidRequest(payslipRequest))
            {
                if (payslipRequest == null)
                {
                    _logger.LogTrace(DomainConstants.LOG_EVENT_VALIDATE_REQUEST, "Null payslip request.");
                }
                else
                {
                    _logger.LogTrace(DomainConstants.LOG_EVENT_VALIDATE_REQUEST, "Failed to validate payslip request: {0}, {1}, {2}, {3}, {4}",
                        payslipRequest.FirstName, payslipRequest.LastName, payslipRequest.AnnualSalary, payslipRequest.SuperRate, payslipRequest.PayStartPeriod);
                }
                return null;
            }

            var payslipMonth = payslipRequest.PayStartPeriod;

            var grossIncome = (int) Math.Round((decimal) (payslipRequest.AnnualSalary / 12));
            var supperannuation = (int) Math.Round((decimal) (grossIncome * payslipRequest.SuperRate / 100));
            var payPeriod = string.Format("1 {0} - {1} {0}", payslipMonth.ToString("MMMM"), new DateTime(payslipMonth.Year, payslipMonth.Month, 1).AddMonths(1).AddDays(-1).Day);
            var incomeTax = GetMonthlyIncomeTax(payslipRequest.AnnualSalary, payslipMonth);
            var netIncome = grossIncome - incomeTax;

            return new PayslipResponse()
            {
                Name = string.Format("{0} {1}", payslipRequest.FirstName.Trim(), payslipRequest.LastName.Trim()),
                GrossIncome = grossIncome,
                IncomeTax = incomeTax,
                NetIncome = netIncome,
                Superannuation = supperannuation,
                PayPeriod = payPeriod
            };
        }

        private int GetMonthlyIncomeTax(int employeeAnnualSalary, DateTime taxForMonth)
        {
            // find tax rates applying to the required date
            var correspondingRates = _taxSettings
                .TaxRates
                .FirstOrDefault(x => (!x.ApplyFrom.HasValue || x.ApplyFrom <= taxForMonth) && (!x.ApplyTo.HasValue || taxForMonth <=  x.ApplyTo.Value));

            if (correspondingRates == null)
            {
                _logger.LogError(DomainConstants.LOG_EVENT_VALIDATE_REQUEST, "Missing tax settings for required date: {0}", taxForMonth);
                throw new Exception(DomainConstants.EXCEPTION_MISSING_TAX_SETTINGS);
            }

            // find tax rates for the income bracket
            var correspondingTaxBracket = correspondingRates
                .TaxBrackets
                .FirstOrDefault(x => (employeeAnnualSalary == 0 || x.FromIncome < employeeAnnualSalary) &&
                            (!x.ToIncome.HasValue || employeeAnnualSalary <= x.ToIncome.Value));
            if (correspondingTaxBracket == null)
            {
                _logger.LogError(DomainConstants.LOG_EVENT_VALIDATE_REQUEST, "Invalid tax brackets settings! The configuration is not covering income value: {0}", employeeAnnualSalary);
                throw new Exception(DomainConstants.EXCEPTION_MISSING_TAX_BRACKET);
            }

            var taxPerYear = correspondingTaxBracket.Base +
                             (employeeAnnualSalary - correspondingTaxBracket.FromIncome) * correspondingTaxBracket.TaxPerDollar;

            return (int) Math.Round(taxPerYear / 12);
        }

        private bool IsValidRequest(PayslipRequest payslipRequest)
        {
            var isValid = false;

            if (payslipRequest != null)
            {
                var context = new ValidationContext(payslipRequest);
                var validationResults = new List<ValidationResult>();
                isValid = Validator.TryValidateObject(payslipRequest, context, validationResults, true);
            }

            return isValid;
        }
    }
}
