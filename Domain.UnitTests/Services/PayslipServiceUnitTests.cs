using System;
using System.Collections.Generic;
using Domain.Helpers;
using Domain.Models;
using Domain.Models.Configuration;
using Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Domain.UnitTests.Services
{

    [TestClass]
    public class PayslipServiceUnitTests
    {
        private PayslipService _payslipService;
        private Mock<ILogger<PayslipService>> _loggerMock;

        [TestInitialize]
        public void Init()
        {
            _loggerMock = new Mock<ILogger<PayslipService>>();
            //_loggerMock.Setup(x => x.LogTrace(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();
            //_loggerMock.Setup(x => x.LogError(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();

            _payslipService = new PayslipService(null, _loggerMock.Object);
        }

        [TestMethod]
        public void GeneratePayslip_NullRequest_ReturnsNull()
        {
            var response = _payslipService.GeneratePayslip(null);

            Assert.IsNull(response);
        }
        
        [TestMethod]
        public void GeneratePayslip_NoEmployeeData_ReturnsNull()
        {
            var payslipRequest = new PayslipRequest() { PayStartPeriod = new DateTime(2015, 2, 2) };

            var response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.IsNull(response);
        }

        [TestMethod]
        public void GeneratePayslip_NegativSalary_ReturnsNull()
        {
            var payslipRequest = GetMockRequestData();
            payslipRequest.AnnualSalary = -50000;
            
            var response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.IsNull(response);
        }

        [TestMethod]
        public void GeneratePayslip_NegativRate_ReturnsNull()
        {
            var payslipRequest = GetMockRequestData();
            payslipRequest.SuperRate = -5;
            
            var response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.IsNull(response);
        }

        [TestMethod]
        public void GeneratePayslip_FuturePayslipDate_ReturnsNull()
        {
            var payslipRequest = GetMockRequestData();
            payslipRequest.PayStartPeriod = DateTime.Parse("01 December");

            var response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.IsNull(response);
        }

        [TestMethod]
        public void GeneratePayslip_CorrectNameReturned()
        {
            var payslipRequest = GetMockRequestData();
            
            var response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.IsNotNull(response);
            Assert.AreEqual("John Smith", response.Name);
        }

        [TestMethod]
        public void GeneratePayslip_PayPeriod_CorrectFormat()
        {
            var payslipRequest = GetMockRequestData();

            payslipRequest.PayStartPeriod = DateTime.Parse("12/05/2017");
            var response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.AreEqual("1 May - 31 May" ,response.PayPeriod);

            payslipRequest.PayStartPeriod = DateTime.Parse("21 Feb");
            response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.AreEqual("1 February - 28 February", response.PayPeriod);

            payslipRequest.PayStartPeriod = DateTime.Parse("21 Feb 2016");
            response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.AreEqual("1 February - 29 February", response.PayPeriod);

            payslipRequest.PayStartPeriod = DateTime.Parse("05-Apr-05");
            response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.AreEqual("1 April - 30 April", response.PayPeriod);
        }

        [TestMethod]
        public void GeneratePayslip_Calculations_CorrectValue()
        {
            // some tests on some provided test data
            var payslipRequest = GetMockRequestData();
            payslipRequest.SuperRate = 9;
            payslipRequest.AnnualSalary = 60050;
            var response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.AreEqual(5004, response.GrossIncome);
            Assert.AreEqual(922, response.IncomeTax);
            Assert.AreEqual(4082, response.NetIncome);
            Assert.AreEqual(450, response.Superannuation);

            payslipRequest.SuperRate = 10;
            payslipRequest.AnnualSalary = 120000;
            response = _payslipService.GeneratePayslip(payslipRequest);

            Assert.AreEqual(10000, response.GrossIncome);
            Assert.AreEqual(2696, response.IncomeTax);
            Assert.AreEqual(7304, response.NetIncome);
            Assert.AreEqual(1000, response.Superannuation);
            
            // here are never ending posibilities for testing
            TestCalculations(0, 1, new TaxBracket() { FromIncome = 0, ToIncome = 18200, Base = 0, TaxPerDollar = 0 });
            TestCalculations(10000, 1, new TaxBracket() { FromIncome = 0, ToIncome = 18200, Base = 0, TaxPerDollar = 0 });
            TestCalculations(40000, 1, new TaxBracket() { FromIncome = 37000, ToIncome = 80000, Base = 3572, TaxPerDollar = 0.325m });
            TestCalculations(90000, 1, new TaxBracket() { FromIncome = 80000, ToIncome = 180000, Base = 17547, TaxPerDollar = 0.37m });
            TestCalculations(190000, 1, new TaxBracket() { FromIncome = 180000, ToIncome = null, Base = 54547, TaxPerDollar = 0.45m });
        }
        
        private void TestCalculations(int salary, int rate, TaxBracket taxBracket)
        {
            var payslipRequest = GetMockRequestData();
            payslipRequest.AnnualSalary = salary;
            payslipRequest.SuperRate = rate;
            var response = _payslipService.GeneratePayslip(payslipRequest);

            var expectedGrossIncome = (int)Math.Round((decimal)(salary / 12));
            var expectedSuperannuation = (int)Math.Round((decimal)(expectedGrossIncome * rate / 100));
            var expectedIncomeTax = salary == 0 ? 0
                : (int)Math.Round((taxBracket.Base + (salary - taxBracket.FromIncome) * taxBracket.TaxPerDollar) / 12);

            Assert.AreEqual(expectedGrossIncome, response.GrossIncome);
            Assert.AreEqual(expectedSuperannuation, response.Superannuation);
            Assert.AreEqual(expectedIncomeTax, response.IncomeTax);
            Assert.AreEqual(expectedGrossIncome - expectedIncomeTax, response.NetIncome);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), DomainConstants.EXCEPTION_MISSING_TAX_SETTINGS)]
        public void PayslipService_MissingTaxRatesForDate__ThrowsException()
        {
            var payslipRequest = GetMockRequestData();
            var taxSettings = new TaxSettings()
            {
                TaxRates = new List<TaxRates>()
                {
                    new TaxRates(){ ApplyFrom = null, ApplyTo = new DateTime(2015, 1, 1), TaxBrackets = new List<TaxBracket>()
                    {
                        new TaxBracket(){Base = 0, FromIncome = 0, ToIncome = 110000, TaxPerDollar = 0.10m },
                        new TaxBracket(){Base = 11000, FromIncome = 125000, ToIncome = null, TaxPerDollar = 0.20m }
                    }},
                    new TaxRates(){ ApplyFrom = new DateTime(2016, 1, 1), ApplyTo = null, TaxBrackets = new List<TaxBracket>()
                    {
                        new TaxBracket(){Base = 0, FromIncome = 0, ToIncome = 110000, TaxPerDollar = 0.10m },
                        new TaxBracket(){Base = 11000, FromIncome = 110000, ToIncome = null, TaxPerDollar = 0.20m }
                    }},
                }
            };

            _payslipService = new PayslipService(taxSettings, _loggerMock.Object);

            var response = _payslipService.GeneratePayslip(payslipRequest);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), DomainConstants.EXCEPTION_MISSING_TAX_BRACKET)]
        public void PayslipService_MissingTaxBraketForIncome_ThrowsException()
        {
            var payslipRequest = GetMockRequestData();
            var taxSettings = new TaxSettings()
            {
                TaxRates = new List<TaxRates>()
                {
                    new TaxRates(){ ApplyFrom = null, ApplyTo = new DateTime(2015, 9, 30), TaxBrackets = new List<TaxBracket>()
                    {
                        new TaxBracket(){Base = 0, FromIncome = 0, ToIncome = 110000, TaxPerDollar = 0.10m },
                        new TaxBracket(){Base = 11000, FromIncome = 125000, ToIncome = null, TaxPerDollar = 0.20m }
                    }},
                    new TaxRates(){ ApplyFrom = new DateTime(2015, 10, 1), ApplyTo = null, TaxBrackets = new List<TaxBracket>()
                    {
                        new TaxBracket(){Base = 0, FromIncome = 0, ToIncome = 110000, TaxPerDollar = 0.10m },
                        new TaxBracket(){Base = 11000, FromIncome = 110000, ToIncome = null, TaxPerDollar = 0.20m }
                    }},
                }
            };

            _payslipService = new PayslipService(taxSettings, _loggerMock.Object);

            var response = _payslipService.GeneratePayslip(payslipRequest);
        }

        private PayslipRequest GetMockRequestData()
        {
            return new PayslipRequest()
            {
                FirstName = "John",
                LastName = "Smith",
                AnnualSalary = 120000,
                SuperRate = 8,
                PayStartPeriod = new DateTime(2015, 2, 2)
            };
        }

    }

}
