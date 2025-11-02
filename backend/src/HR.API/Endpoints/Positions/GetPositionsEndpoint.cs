using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Positions;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Positions
{
    public class GetPositionsEndpoint : Endpoint<PositionListRequest, PaginatedResponse<PositionDto>>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public GetPositionsEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Get("/api/positions");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Get all positions with optional filtering";
                s.Description = "Returns a paginated list of positions";
                s.Response<PaginatedResponse<PositionDto>>(200, "Positions returned successfully");
            });
        }

        public override async Task HandleAsync(PositionListRequest req, CancellationToken ct)
        {
            var query = _dbContext.Positions.AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(req.SearchTerm))
            {
                query = query.Where(p => p.Title.Contains(req.SearchTerm) ||
                                       (p.Description != null && p.Description.Contains(req.SearchTerm)));
            }

            // Filter by department if provided
            if (req.DepartmentId.HasValue)
            {
                query = query.Where(p => p.DepartmentId == req.DepartmentId.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync(ct);

            // Apply pagination
            var positions = await query
                .Include(p => p.Department)
                .OrderBy(p => p.Title)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(p => new PositionDto
                {
                    PositionId = p.PositionId,
                    Title = p.Title,
                    Description = p.Description,
                    MinSalary = p.MinSalary,
                    MaxSalary = p.MaxSalary,
                    DepartmentId = p.DepartmentId,
                    DepartmentName = p.Department.Name,
                    EmployeeCount = _dbContext.Employees.Count(e => e.PositionId == p.PositionId),
                    Created = p.Created,
                    Modified = p.Modified
                })
                .ToListAsync(ct);

            // Calculate total pages
            var totalPages = (int)Math.Ceiling(totalCount / (double)req.PageSize);            await SendAsync(new PaginatedResponse<PositionDto>
            {
                Success = true,
                Items = positions,
                PageNumber = req.Page,
                PageSize = req.PageSize,
                TotalCount = totalCount,
                Message = "Positions retrieved successfully"
            }, cancellation: ct);
        }
    }
}
