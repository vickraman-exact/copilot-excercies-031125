using FastEndpoints;
using HR.API.Models;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Positions
{
    public class DeletePositionEndpoint : Endpoint<IdRequest, BaseResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public DeletePositionEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Delete("/api/positions/{Id}");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Delete a position";
                s.Description = "Deletes a position with the specified ID";
                s.Response<BaseResponse>(200, "Position deleted successfully");
                s.Response<BaseResponse>(404, "Position not found");
                s.Response<BaseResponse>(400, "Position cannot be deleted as it's associated with employees");
            });
        }

        public override async Task HandleAsync(IdRequest req, CancellationToken ct)
        {
            var position = await _dbContext.Positions
                .FirstOrDefaultAsync(p => p.PositionId == req.Id, ct);

            if (position == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Check if there are employees using this position
            var hasEmployees = await _dbContext.Employees
                .AnyAsync(e => e.PositionId == position.PositionId, ct);

            if (hasEmployees)
            {
                await SendAsync(new BaseResponse
                {
                    Success = false,
                    Message = "Cannot delete position as it's associated with one or more employees"
                }, statusCode: 400, ct);
                return;
            }

            _dbContext.Positions.Remove(position);
            await _dbContext.SaveChangesAsync(ct);

            var response = new BaseResponse
            {
                Success = true,
                Message = "Position deleted successfully",
                Id = position.PositionId
            };

            await SendAsync(response, cancellation: ct);
        }
    }
}
