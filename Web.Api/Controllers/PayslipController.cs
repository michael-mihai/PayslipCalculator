using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    public class PayslipController : Controller
    {
        private readonly IPayslipService _payslipService;

        public PayslipController(IPayslipService payslipService)
        {
            _payslipService = payslipService;
        }

        [HttpPost]
        [HttpPost("CalculatePayslips")]
        public IActionResult CalculatePayslips([FromBody] List<PayslipRequest> payslipRequests)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                List<PayslipResponse> data = payslipRequests.Select(x => _payslipService.GeneratePayslip(x)).ToList();
                return Ok(data);
            }
        }
    }
}
