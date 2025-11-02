using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Departments;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Departments
{
    public class GetDepartmentsEndpoint : Endpoint<DepartmentListRequest, PaginatedResponse<DepartmentDto>>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public GetDepartmentsEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Get("/api/departments");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Get all departments with optional filtering";
                s.Description = "Returns a paginated list of departments";
                s.Response<PaginatedResponse<DepartmentDto>>(200, "Departments returned successfully");
            });
        }

        public override async Task HandleAsync(DepartmentListRequest req, CancellationToken ct)
        {
            var query = _dbContext.Departments.AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(req.SearchTerm))
            {
                query = query.Where(d => d.Name.Contains(req.SearchTerm) ||
                                        (d.Description != null && d.Description.Contains(req.SearchTerm)));
            }

            // If not including sub-departments, only get top-level ones
            if (!req.IncludeSubDepartments)
            {
                query = query.Where(d => d.ParentDepartmentId == null);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync(ct);

            // Apply pagination
            var departments = await query
                .Include(d => d.Manager)
                .Include(d => d.ParentDepartment)
                .OrderBy(d => d.Name)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Description = d.Description,
                    ManagerId = d.ManagerId,
                    ManagerName = d.Manager != null ? $"{d.Manager.FirstName} {d.Manager.LastName}" : null,
                    ParentDepartmentId = d.ParentDepartmentId,
                    ParentDepartmentName = d.ParentDepartment != null ? d.ParentDepartment.Name : null,
                    EmployeeCount = _dbContext.Employees.Count(e => e.DepartmentId == d.DepartmentId),
                    Created = d.Created,
                    Modified = d.Modified
                })
                .ToListAsync(ct);

            // Calculate total pages
            var totalPages = (int)Math.Ceiling(totalCount / (double)req.PageSize);            await SendAsync(new PaginatedResponse<DepartmentDto>
            {
                Success = true,
                Items = departments,
                PageNumber = req.Page,
                PageSize = req.PageSize,
                TotalCount = totalCount,
                Message = "Departments retrieved successfully"
            }, cancellation: ct);
        }
    }
}
