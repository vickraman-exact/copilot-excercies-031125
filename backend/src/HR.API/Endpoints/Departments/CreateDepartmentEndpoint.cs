using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Departments;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Departments
{
    public class CreateDepartmentEndpoint : Endpoint<CreateDepartmentRequest, BaseResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public CreateDepartmentEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Post("/api/departments");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Create a new department";
                s.Description = "Creates a new department with the provided information";
                s.Response<BaseResponse>(201, "Department created successfully");
                s.Response<BaseResponse>(400, "Invalid department data");
            });
        }

        public override async Task HandleAsync(CreateDepartmentRequest req, CancellationToken ct)
        {
            // Validation of parent department and manager IDs could be added here
            // For now, we'll keep it simple

            var department = new Department
            {
                DepartmentId = Guid.NewGuid(),
                Name = req.Name,
                Description = req.Description,
                ManagerId = req.ManagerId,
                ParentDepartmentId = req.ParentDepartmentId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            _dbContext.Departments.Add(department);
            await _dbContext.SaveChangesAsync(ct);

            var response = new BaseResponse
            {
                Success = true,
                Message = "Department created successfully",
                Id = department.DepartmentId
            };

            await SendCreatedAtAsync<GetDepartmentEndpoint>(
                new { Id = department.DepartmentId },
                response,
                generateAbsoluteUrl: true,
                cancellation: ct);
        }
    }
}
