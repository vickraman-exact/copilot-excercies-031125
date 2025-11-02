using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Positions;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Positions
{
    public class UpdatePositionRequestWithId : UpdatePositionRequest
    {
        public Guid Id { get; set; }
    }

    public class UpdatePositionEndpoint : Endpoint<UpdatePositionRequestWithId, BaseResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public UpdatePositionEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Put("/api/positions/{Id}");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Update an existing position";
                s.Description = "Updates a position's information";
                s.Response<BaseResponse>(200, "Position updated successfully");
                s.Response<BaseResponse>(404, "Position not found");
            });
        }

        public override async Task HandleAsync(UpdatePositionRequestWithId req, CancellationToken ct)
        {
            var position = await _dbContext.Positions
                .FirstOrDefaultAsync(p => p.PositionId == req.Id, ct);

            if (position == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Update properties
            position.Title = req.Title;
            position.Description = req.Description;
            position.MinSalary = req.MinSalary;
            position.MaxSalary = req.MaxSalary;
            position.DepartmentId = req.DepartmentId;
            position.Modified = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            var response = new BaseResponse
            {
                Success = true,
                Message = "Position updated successfully",
                Id = position.PositionId
            };

            await SendAsync(response, cancellation: ct);
        }
    }
}
