using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Employees;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Endpoints.Employees
{
    public class UpdateEmployeeRequestWithId : UpdateEmployeeRequest
    {
        public Guid Id { get; set; }
    }

    public class UpdateEmployeeEndpoint : Endpoint<UpdateEmployeeRequestWithId, BaseResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public UpdateEmployeeEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Put("/api/employees/{Id}");
            AllowAnonymous(); // Eventually will be replaced with authorization
            Summary(s =>
            {
                s.Summary = "Update an existing employee";
                s.Description = "Updates an employee's information";
                s.Response<BaseResponse>(200, "Employee updated successfully");
                s.Response<BaseResponse>(400, "Invalid employee data");
                s.Response<BaseResponse>(404, "Employee not found");
                s.Response<BaseResponse>(409, "Email already exists");
            });
        }

        public override async Task HandleAsync(UpdateEmployeeRequestWithId req, CancellationToken ct)
        {
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == req.Id, ct);

            if (employee == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Check for duplicate email
            if (await _dbContext.Employees.AnyAsync(e => e.Email == req.Email && e.EmployeeId != req.Id, ct))
            {
                await SendAsync(new BaseResponse
                {
                    Success = false,
                    Message = "An employee with this email address already exists"
                }, 409, ct);
                return;
            }            // Update employee properties
            employee.FirstName = req.FirstName;
            employee.LastName = req.LastName;
            employee.Email = req.Email;
            employee.Phone = req.Phone;
            employee.Address = req.Address;
            employee.DateOfBirth = req.DateOfBirth;
            employee.HireDate = req.HireDate;
            employee.Status = req.Status;
            employee.DepartmentId = req.DepartmentId;
            employee.PositionId = req.PositionId;
            employee.EmergencyContact = req.EmergencyContact;
            employee.BankDetails = req.BankDetails;
            employee.Modified = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            await SendAsync(new BaseResponse
            {
                Success = true,
                Message = "Employee updated successfully"
            }, cancellation: ct);
        }
    }
}
