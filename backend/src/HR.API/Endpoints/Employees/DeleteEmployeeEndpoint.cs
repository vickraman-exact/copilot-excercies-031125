using FastEndpoints;
using HR.API.Models;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Endpoints.Employees
{
    public class DeleteEmployeeEndpoint : Endpoint<IdRequest, BaseResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public DeleteEmployeeEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Delete("/api/employees/{Id}");
            AllowAnonymous(); // Eventually will be replaced with authorization
            Summary(s =>
            {
                s.Summary = "Delete or deactivate an employee";
                s.Description = "Soft deletes an employee by marking them as Terminated";
                s.Response<BaseResponse>(200, "Employee deactivated successfully");
                s.Response<BaseResponse>(404, "Employee not found");
            });
        }

        public override async Task HandleAsync(IdRequest req, CancellationToken ct)
        {
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == req.Id, ct);

            if (employee == null)
            {                await SendNotFoundAsync(ct);
                return;
            }

            // Soft delete by changing status to Terminated
            employee.Status = EmployeeStatus.Terminated;
            employee.TerminationDate = DateTime.UtcNow;
            employee.Modified = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            await SendAsync(new BaseResponse
            {
                Success = true,
                Message = "Employee deactivated successfully"
            }, cancellation: ct);
        }
    }
}
