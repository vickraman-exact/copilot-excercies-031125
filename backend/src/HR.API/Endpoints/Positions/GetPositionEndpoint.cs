using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Positions;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Positions
{
    public class GetPositionEndpoint : Endpoint<IdRequest, PositionDto>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public GetPositionEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Get("/api/positions/{Id}");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Get position by ID";
                s.Description = "Returns details of a specific position";
                s.Response<PositionDto>(200, "Position returned successfully");
                s.Response(404, "Position not found");
            });
        }

        public override async Task HandleAsync(IdRequest req, CancellationToken ct)
        {
            var position = await _dbContext.Positions
                .Include(p => p.Department)
                .FirstOrDefaultAsync(p => p.PositionId == req.Id, ct);

            if (position == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Get employee count
            var employeeCount = await _dbContext.Employees
                .CountAsync(e => e.PositionId == position.PositionId, ct);

            var positionDto = new PositionDto
            {
                PositionId = position.PositionId,
                Title = position.Title,
                Description = position.Description,
                MinSalary = position.MinSalary,
                MaxSalary = position.MaxSalary,
                DepartmentId = position.DepartmentId,
                DepartmentName = position.Department?.Name ?? string.Empty,
                EmployeeCount = employeeCount,
                Created = position.Created,
                Modified = position.Modified
            };

            await SendAsync(positionDto, cancellation: ct);
        }
    }
}
