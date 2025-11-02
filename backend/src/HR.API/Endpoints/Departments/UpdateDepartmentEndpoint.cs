using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Departments;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Departments
{
    public class UpdateDepartmentRequestWithId : UpdateDepartmentRequest
    {
        public Guid Id { get; set; }
    }

    public class UpdateDepartmentEndpoint : Endpoint<UpdateDepartmentRequestWithId, BaseResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public UpdateDepartmentEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Put("/api/departments/{Id}");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Update an existing department";
                s.Description = "Updates a department's information";
                s.Response<BaseResponse>(200, "Department updated successfully");
                s.Response<BaseResponse>(404, "Department not found");
            });
        }

        public override async Task HandleAsync(UpdateDepartmentRequestWithId req, CancellationToken ct)
        {
            var department = await _dbContext.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == req.Id, ct);

            if (department == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Update properties
            department.Name = req.Name;
            department.Description = req.Description;
            department.ManagerId = req.ManagerId;
            department.ParentDepartmentId = req.ParentDepartmentId;
            department.Modified = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            var response = new BaseResponse
            {
                Success = true,
                Message = "Department updated successfully",
                Id = department.DepartmentId
            };

            await SendAsync(response, cancellation: ct);
        }
    }
}
