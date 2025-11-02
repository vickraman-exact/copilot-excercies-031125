using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Departments;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Departments
{
    public class GetDepartmentEndpoint : Endpoint<IdRequest, DepartmentDto>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public GetDepartmentEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Get("/api/departments/{Id}");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Get department by ID";
                s.Description = "Returns details of a specific department";
                s.Response<DepartmentDto>(200, "Department returned successfully");
                s.Response(404, "Department not found");
            });
        }

        public override async Task HandleAsync(IdRequest req, CancellationToken ct)
        {
            var department = await _dbContext.Departments
                .Include(d => d.Manager)
                .Include(d => d.ParentDepartment)
                .FirstOrDefaultAsync(d => d.DepartmentId == req.Id, ct);

            if (department == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Get employee count
            var employeeCount = await _dbContext.Employees
                .CountAsync(e => e.DepartmentId == department.DepartmentId, ct);

            // Get sub-departments if any
            var subDepartments = await _dbContext.Departments
                .Where(d => d.ParentDepartmentId == department.DepartmentId)
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Description = d.Description,
                    ManagerId = d.ManagerId,
                    ManagerName = d.Manager != null ? $"{d.Manager.FirstName} {d.Manager.LastName}" : null,
                    ParentDepartmentId = d.ParentDepartmentId,
                    ParentDepartmentName = department.Name,
                    EmployeeCount = _dbContext.Employees.Count(e => e.DepartmentId == d.DepartmentId)
                })
                .ToListAsync(ct);

            var departmentDto = new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Description = department.Description,
                ManagerId = department.ManagerId,
                ManagerName = department.Manager != null ? $"{department.Manager.FirstName} {department.Manager.LastName}" : null,
                ParentDepartmentId = department.ParentDepartmentId,
                ParentDepartmentName = department.ParentDepartment != null ? department.ParentDepartment.Name : null,
                SubDepartments = subDepartments.Count > 0 ? subDepartments : null,
                EmployeeCount = employeeCount,
                Created = department.Created,
                Modified = department.Modified
            };

            await SendAsync(departmentDto, cancellation: ct);
        }
    }
}
