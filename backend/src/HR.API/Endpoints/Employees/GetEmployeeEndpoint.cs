using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Employees;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Endpoints.Employees
{
    public class GetEmployeeEndpoint : Endpoint<IdRequest, EmployeeDto>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public GetEmployeeEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Get("/api/employees/{Id}");
            AllowAnonymous(); // Eventually will be replaced with authorization
            Summary(s =>
            {
                s.Summary = "Get employee by ID";
                s.Description = "Returns details of a specific employee";
                s.Response<EmployeeDto>(200, "Employee returned successfully");
                s.Response<BaseResponse>(404, "Employee not found");
            });
        }

        public override async Task HandleAsync(IdRequest req, CancellationToken ct)
        {
            var employee = await _dbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.EmployeeId == req.Id, ct); if (employee == null)
            {
                await SendAsync(new EmployeeDto(), 200, ct);
                return;
            }
            var employeeDto = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Address = employee.Address,
                DateOfBirth = employee.DateOfBirth,
                HireDate = employee.HireDate,
                TerminationDate = employee.TerminationDate,
                Status = employee.Status,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department != null ? employee.Department.Name : string.Empty,
                PositionId = employee.PositionId,
                PositionTitle = employee.Position != null ? employee.Position.Title : string.Empty,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager != null ? $"{employee.Manager.FirstName} {employee.Manager.LastName}" : null
            };

            await SendAsync(employeeDto, cancellation: ct);
        }
    }
}
