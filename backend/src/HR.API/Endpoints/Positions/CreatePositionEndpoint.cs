using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Positions;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HR.API.Endpoints.Positions
{
    public class CreatePositionEndpoint : Endpoint<CreatePositionRequest, BaseResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public CreatePositionEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Post("/api/positions");
            AllowAnonymous(); // Will be replaced with proper authorization
            Summary(s =>
            {
                s.Summary = "Create a new position";
                s.Description = "Creates a new position with the provided information";
                s.Response<BaseResponse>(201, "Position created successfully");
                s.Response<BaseResponse>(400, "Invalid position data");
            });
        }

        public override async Task HandleAsync(CreatePositionRequest req, CancellationToken ct)
        {
            // Validation could be added here
            // For now, we'll keep it simple

            var position = new Position
            {
                PositionId = Guid.NewGuid(),
                Title = req.Title,
                Description = req.Description,
                MinSalary = req.MinSalary,
                MaxSalary = req.MaxSalary,
                DepartmentId = req.DepartmentId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            _dbContext.Positions.Add(position);
            await _dbContext.SaveChangesAsync(ct);

            var response = new BaseResponse
            {
                Success = true,
                Message = "Position created successfully",
                Id = position.PositionId
            };

            await SendCreatedAtAsync<GetPositionEndpoint>(
                new { Id = position.PositionId },
                response,
                generateAbsoluteUrl: true,
                cancellation: ct);
        }
    }
}
