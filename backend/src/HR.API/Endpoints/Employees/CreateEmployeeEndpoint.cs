using FastEndpoints;
using FluentValidation;
using HR.API.Models;
using HR.API.Models.Employees;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.API.Endpoints.Employees
{
    public class CreateEmployeeEndpoint : Endpoint<CreateEmployeeRequest, BaseResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;

        public CreateEmployeeEndpoint(HRPayDezkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Post("/api/employees");
            AllowAnonymous(); // Eventually will be replaced with authorization
            Summary(s =>
            {
                s.Summary = "Create a new employee";
                s.Description = "Creates a new employee record in the system";
                s.Response<BaseResponse>(201, "Employee created successfully");
                s.Response<BaseResponse>(400, "Invalid employee data");
                s.Response<BaseResponse>(409, "Email already exists");
            });
        }

        public class CreateEmployeeValidator : Validator<CreateEmployeeRequest>
        {
            public CreateEmployeeValidator()
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("First name is required")
                    .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Last name is required")
                    .MaximumLength(100).WithMessage("Last name must not exceed 100 characters"); RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required")
                    .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

                RuleFor(x => x.DateOfBirth)
                    .LessThan(DateTime.UtcNow.AddYears(-16)).WithMessage("Employee must be at least 16 years old");

                RuleFor(x => x.HireDate)
                    .NotEmpty().WithMessage("Hire date is required")
                    .GreaterThan(DateTime.UtcNow.AddYears(-100)).WithMessage("Hire date cannot be more than 100 years in the past")
                    .LessThanOrEqualTo(DateTime.UtcNow.AddMonths(1)).WithMessage("Hire date cannot be more than 1 month in the future");

                RuleFor(x => x.DepartmentId)
                    .NotEmpty().WithMessage("Department is required");

                RuleFor(x => x.PositionId)
                    .NotEmpty().WithMessage("Position is required");
            }
        }

        public override async Task HandleAsync(CreateEmployeeRequest req, CancellationToken ct)
        {
            // Check for duplicate email
            if (await _dbContext.Employees.AnyAsync(e => e.Email == req.Email, ct))
            {
                await SendAsync(new BaseResponse
                {
                    Success = false,
                    Message = "An employee with this email address already exists"
                }, 409, ct);
                return;
            }

            var employee = new Employee
            {
                EmployeeId = Guid.NewGuid(),
                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                Phone = req.Phone,
                Address = req.Address,
                DateOfBirth = req.DateOfBirth,
                HireDate = req.HireDate,
                Status = req.Status,
                DepartmentId = req.DepartmentId,
                PositionId = req.PositionId,
                ManagerId = req.ManagerId,
                EmergencyContact = req.EmergencyContact,
                BankDetails = req.BankDetails,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync(ct);

            var response = new BaseResponse
            {
                Success = true,
                Message = "Employee created successfully"
            };

            await SendCreatedAtAsync<GetEmployeeEndpoint>(
                new { Id = employee.EmployeeId },
                response,
                generateAbsoluteUrl: true,
                cancellation: ct
            );
        }
    }
}
