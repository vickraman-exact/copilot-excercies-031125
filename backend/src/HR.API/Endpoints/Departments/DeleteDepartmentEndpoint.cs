using FastEndpoints;
using HR.API.Models;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Departments
{
    public class DeleteDepartmentEndpoint : Endpoint<IdRequest, BaseResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public DeleteDepartmentEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Delete("/api/departments/{Id}");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Delete a department";
                s.Description = "Deletes a department with the specified ID";
                s.Response<BaseResponse>(200, "Department deleted successfully");
                s.Response<BaseResponse>(404, "Department not found");
                s.Response<BaseResponse>(400, "Department cannot be deleted due to associations");
            });
        }

        public override async Task HandleAsync(IdRequest req, CancellationToken ct)
        {
            var department = await _dbContext.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == req.Id, ct);

            if (department == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Check if there are sub-departments
            var hasSubDepartments = await _dbContext.Departments
                .AnyAsync(d => d.ParentDepartmentId == department.DepartmentId, ct);

            // Check if there are employees in this department
            var hasEmployees = await _dbContext.Employees
                .AnyAsync(e => e.DepartmentId == department.DepartmentId, ct);

            // Check if there are positions in this department
            var hasPositions = await _dbContext.Positions
                .AnyAsync(p => p.DepartmentId == department.DepartmentId, ct);

            if (hasSubDepartments || hasEmployees || hasPositions)
            {
                await SendAsync(new BaseResponse
                {
                    Success = false,
                    Message = "Cannot delete department as it has associated sub-departments, employees, or positions"
                }, statusCode: 400, ct);
                return;
            }

            _dbContext.Departments.Remove(department);
            await _dbContext.SaveChangesAsync(ct);

            var response = new BaseResponse
            {
                Success = true,
                Message = "Department deleted successfully",
                Id = department.DepartmentId
            };

            await SendAsync(response, cancellation: ct);
        }
    }
}
