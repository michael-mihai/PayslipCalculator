using Domain.Models;

namespace Domain.Services
{
    public interface IPayslipService
    {
        PayslipResponse GeneratePayslip(PayslipRequest payslipRequest);        
    }
}
