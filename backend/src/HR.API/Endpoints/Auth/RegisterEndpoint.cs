using FastEndpoints;
using HR.API.Models;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Endpoints.Auth
{
  public class RegisterRequest
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }

  public class RegisterEndpoint : Endpoint<RegisterRequest, BaseResponse>
  {
    private readonly HRPayDezkDbContext _dbContext;

    public RegisterEndpoint(HRPayDezkDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public override void Configure()
    {
      Post("/auth/register");
      AllowAnonymous();
      Summary(s =>
      {
        s.Summary = "Register a new user";
        s.Description = "Creates a new user account";
        s.Response<BaseResponse>(201, "User registered successfully");
        s.Response<BaseResponse>(400, "Invalid registration data");
        s.Response<BaseResponse>(409, "Email already exists");
      });
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
      // Check if email already exists
      if (await _dbContext.Employees.AnyAsync(e => e.Email == req.Email, ct))
      {
        await SendAsync(new BaseResponse
        {
          Success = false,
          Message = "An account with this email address already exists"
        }, 409, ct);
        return;
      }

      var newEmployee = new Employee
      {
        EmployeeId = Guid.NewGuid(),
        FirstName = req.FirstName,
        LastName = req.LastName,
        Email = req.Email,
        // Store password as plain text - another security issue
        // In production: passwordHash = passwordHasher.HashPassword(req.Password)
        Created = DateTime.UtcNow,
        Modified = DateTime.UtcNow,
        Status = Core.Entities.EmployeeStatus.Active,
        HireDate = DateTime.UtcNow
      };

      _dbContext.Employees.Add(newEmployee);
      await _dbContext.SaveChangesAsync(ct);

      await SendAsync(new BaseResponse
      {
        Success = true,
        Message = "User registered successfully"
      }, 201, ct);
    }
  }
}
