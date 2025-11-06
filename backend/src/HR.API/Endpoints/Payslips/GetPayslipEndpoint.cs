using FastEndpoints;
using HR.API.Models;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Endpoints.Payslips
{
  public class GetPayslipRequest
  {
    public Guid Id { get; set; }
  }

  public class PayslipDetailDto
  {
    public Guid PayslipId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
    public decimal TotalDeductions { get; set; }
    public List<DeductionItemDto> Deductions { get; set; } = new List<DeductionItemDto>();
    public PayslipStatus Status { get; set; }
    public DateTime IssueDate { get; set; }
  }

  public class DeductionItemDto
  {
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
  }

  public class GetPayslipEndpoint : Endpoint<GetPayslipRequest, PayslipDetailDto>
  {
    private readonly HRPayDezkDbContext _dbContext;

    public GetPayslipEndpoint(HRPayDezkDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public override void Configure()
    {
      Get("/api/payslips/{Id}");
      AllowAnonymous();
      Summary(s =>
      {
        s.Summary = "Get payslip details by ID";
        s.Description = "Returns detailed information for a specific payslip";
        s.Response<PayslipDetailDto>(200, "Payslip details returned successfully");
        s.Response(404, "Payslip not found");
      });
    }

    public override async Task HandleAsync(GetPayslipRequest req, CancellationToken ct)
    {
      var payslip = await _dbContext.Payslips
          .Include(p => p.Employee)
          .FirstOrDefaultAsync(p => p.PayslipId == req.Id, ct);

      if (payslip == null)
      {
        await SendNotFoundAsync(ct);
        return;
      }

      var payslipDto = new PayslipDetailDto
      {
        PayslipId = payslip.PayslipId,
        EmployeeId = payslip.EmployeeId,
        EmployeeName = $"{payslip.Employee?.FirstName} {payslip.Employee?.LastName}",
        PayPeriodStart = payslip.PayPeriod.StartDate,
        PayPeriodEnd = payslip.PayPeriod.EndDate,
        GrossPay = payslip.GrossPay,
        NetPay = payslip.NetPay,
        TotalDeductions = payslip.TotalDeductions,
        Status = payslip.Status,
        IssueDate = payslip.Created
        // Add deduction items if needed
      };

      await SendAsync(payslipDto, cancellation: ct);
    }
  }
}
