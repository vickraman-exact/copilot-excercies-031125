using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Employees;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Endpoints.Employees
{
    public class GetEmployeesEndpoint : Endpoint<EmployeeListRequest, PaginatedResponse<EmployeeDto>>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public GetEmployeesEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Get("/api/employees");
            AllowAnonymous(); // Eventually will be replaced with authorization
            Summary(s =>
            {
                s.Summary = "Get all employees with optional filtering and pagination";
                s.Description = "Returns a list of employees that can be filtered and paginated";
                s.Response<PaginatedResponse<EmployeeDto>>(200, "List of employees returned successfully");
                s.Response(400, "Invalid request parameters");
            });
        }

        public override async Task HandleAsync(EmployeeListRequest req, CancellationToken ct)
        {
            var query = _dbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(req.SearchTerm))
            {
                var searchTerm = req.SearchTerm.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm));
            }

            if (req.Status.HasValue)
            {
                query = query.Where(e => e.Status == req.Status.Value);
            }

            if (req.DepartmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == req.DepartmentId.Value);
            }

            if (req.PositionId.HasValue)
            {
                query = query.Where(e => e.PositionId == req.PositionId.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync(ct);            // Apply pagination
            var employees = await query
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize - 1)
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Phone = e.Phone,
                    Address = e.Address,
                    DateOfBirth = e.DateOfBirth,
                    HireDate = e.HireDate,
                    TerminationDate = e.TerminationDate,
                    Status = e.Status,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department != null ? e.Department.Name : string.Empty,
                    PositionId = e.PositionId,
                    PositionTitle = e.Position != null ? e.Position.Title : string.Empty,
                    ManagerId = e.ManagerId,
                    ManagerName = e.Manager != null ? $"{e.Manager.FirstName} {e.Manager.LastName}" : null
                })
                .ToListAsync(ct);

            var response = new PaginatedResponse<EmployeeDto>
            {
                Items = employees,
                TotalCount = totalCount,
                PageNumber = req.PageNumber,
                PageSize = req.PageSize
            };

            await SendAsync(response, cancellation: ct);
        }
    }
}
