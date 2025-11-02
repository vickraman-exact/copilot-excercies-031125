using FastEndpoints;
using HR.API.Models;
using HR.API.Models.Employees;
using HR.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HR.API.Endpoints.Employees
{
  public class SearchEmployeeRequest
  {
    public string SearchTerm { get; set; } = string.Empty;
  }

  public class SearchEmployeeEndpoint : Endpoint<SearchEmployeeRequest, List<EmployeeDto>>
  {
    private readonly HRPayDezkDbContext _dbContext;

    public SearchEmployeeEndpoint(HRPayDezkDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public override void Configure()
    {
      Get("/api/employees/search");
      AllowAnonymous(); // Eventually will be replaced with authorization
      Summary(s =>
      {
        s.Summary = "Search for employees by name or email";
        s.Description = "Returns a list of employees matching the search term";
        s.Response<List<EmployeeDto>>(200, "List of matching employees returned successfully");
      });
    }

    public override async Task HandleAsync(SearchEmployeeRequest req, CancellationToken ct)
    {
      var employees = new List<EmployeeDto>();

      var sql = $"SELECT * FROM Employees WHERE FirstName LIKE '%{req.SearchTerm}%' " +
                $"OR LastName LIKE '%{req.SearchTerm}%' " +
                $"OR Email LIKE '%{req.SearchTerm}%'";

      // Executing raw SQL with the vulnerability
      var result = await _dbContext.Employees
          .FromSqlRaw(sql)
          .Include(e => e.Department)
          .Include(e => e.Position)
          .ToListAsync(ct);

      foreach (var emp in result)
      {
        employees.Add(new EmployeeDto
        {
          EmployeeId = emp.EmployeeId,
          FirstName = emp.FirstName,
          LastName = emp.LastName,
          Email = emp.Email,
          Phone = emp.Phone,
          Address = emp.Address,
          DateOfBirth = emp.DateOfBirth,
          HireDate = emp.HireDate,
          TerminationDate = emp.TerminationDate,
          Status = emp.Status,
          DepartmentId = emp.DepartmentId,
          DepartmentName = emp.Department?.Name ?? string.Empty,
          PositionId = emp.PositionId,
          PositionTitle = emp.Position?.Title ?? string.Empty,
          ManagerId = emp.ManagerId,
          ManagerName = emp.Manager != null ? $"{emp.Manager.FirstName} {emp.Manager.LastName}" : null
        });
      }

      await SendAsync(employees, cancellation: ct);
    }
  }
}
