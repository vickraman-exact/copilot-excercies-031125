using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR.API.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();

            try
            {
                logger.LogInformation("Starting database seeding...");

                // Only seed if the database is empty (no departments)
                if (await dbContext.Departments.AnyAsync())
                {
                    logger.LogInformation("Database already contains data. Skipping seeding.");
                    return;
                }

                // Seed Departments
                logger.LogInformation("Seeding departments...");
                var hrDepartment = new Department
                {
                    DepartmentId = Guid.NewGuid(),
                    Name = "Human Resources",
                    Description = "Responsible for recruiting, employee relations, and benefits administration.",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };

                var itDepartment = new Department
                {
                    DepartmentId = Guid.NewGuid(),
                    Name = "Information Technology",
                    Description = "Responsible for maintaining IT infrastructure and software development.",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };

                var financeDepartment = new Department
                {
                    DepartmentId = Guid.NewGuid(),
                    Name = "Finance",
                    Description = "Responsible for financial planning, accounting, and payroll administration.",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };

                var marketingDepartment = new Department
                {
                    DepartmentId = Guid.NewGuid(),
                    Name = "Marketing",
                    Description = "Responsible for marketing strategies, advertising, and public relations.",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };

                var departments = new List<Department>
                {
                    hrDepartment, itDepartment, financeDepartment, marketingDepartment
                };

                await dbContext.Departments.AddRangeAsync(departments);
                await dbContext.SaveChangesAsync();

                // Seed Positions
                logger.LogInformation("Seeding positions...");
                var positions = new List<Position>
                {
                    new Position
                    {
                        PositionId = Guid.NewGuid(),
                        Title = "HR Manager",
                        Description = "Responsible for overseeing all HR functions.",
                        MinSalary = 80000,
                        MaxSalary = 120000,
                        DepartmentId = hrDepartment.DepartmentId,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Position
                    {
                        PositionId = Guid.NewGuid(),
                        Title = "HR Specialist",
                        Description = "Handles day-to-day HR operations.",
                        MinSalary = 50000,
                        MaxSalary = 75000,
                        DepartmentId = hrDepartment.DepartmentId,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Position
                    {
                        PositionId = Guid.NewGuid(),
                        Title = "IT Director",
                        Description = "Oversees all IT operations and strategy.",
                        MinSalary = 100000,
                        MaxSalary = 150000,
                        DepartmentId = itDepartment.DepartmentId,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Position
                    {
                        PositionId = Guid.NewGuid(),
                        Title = "Software Developer",
                        Description = "Develops and maintains software applications.",
                        MinSalary = 70000,
                        MaxSalary = 120000,
                        DepartmentId = itDepartment.DepartmentId,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Position
                    {
                        PositionId = Guid.NewGuid(),
                        Title = "Network Administrator",
                        Description = "Manages and maintains IT networks and infrastructure.",
                        MinSalary = 65000,
                        MaxSalary = 95000,
                        DepartmentId = itDepartment.DepartmentId,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Position
                    {
                        PositionId = Guid.NewGuid(),
                        Title = "Finance Manager",
                        Description = "Oversees financial operations and reporting.",
                        MinSalary = 90000,
                        MaxSalary = 130000,
                        DepartmentId = financeDepartment.DepartmentId,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Position
                    {
                        PositionId = Guid.NewGuid(),
                        Title = "Accountant",
                        Description = "Manages financial records and transactions.",
                        MinSalary = 60000,
                        MaxSalary = 85000,
                        DepartmentId = financeDepartment.DepartmentId,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Position
                    {
                        PositionId = Guid.NewGuid(),
                        Title = "Marketing Director",
                        Description = "Oversees marketing strategies and campaigns.",
                        MinSalary = 95000,
                        MaxSalary = 140000,
                        DepartmentId = marketingDepartment.DepartmentId,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Position
                    {
                        PositionId = Guid.NewGuid(),
                        Title = "Marketing Specialist",
                        Description = "Develops and implements marketing campaigns.",
                        MinSalary = 55000,
                        MaxSalary = 80000,
                        DepartmentId = marketingDepartment.DepartmentId,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    }
                };

                await dbContext.Positions.AddRangeAsync(positions);
                await dbContext.SaveChangesAsync();

                // Get positions for employee creation
                var hrManagerPosition = positions.First(p => p.Title == "HR Manager");
                var itDirectorPosition = positions.First(p => p.Title == "IT Director");
                var financeManagerPosition = positions.First(p => p.Title == "Finance Manager");
                var marketingDirectorPosition = positions.First(p => p.Title == "Marketing Director");
                var softwareDeveloperPosition = positions.First(p => p.Title == "Software Developer");

                // Seed Employees
                logger.LogInformation("Seeding employees...");
                var employees = new List<Employee>();

                // HR Manager
                var hrManager = new Employee
                {
                    EmployeeId = Guid.NewGuid(),
                    FirstName = "Jennifer",
                    LastName = "Adams",
                    Email = "jennifer.adams@example.com",
                    Phone = "555-123-4567",
                    Address = "123 Main St, Anytown, USA",
                    DateOfBirth = new DateTime(1980, 5, 15),
                    HireDate = new DateTime(2020, 3, 10),
                    Status = EmployeeStatus.Active,
                    DepartmentId = hrDepartment.DepartmentId,
                    PositionId = hrManagerPosition.PositionId,
                    EmergencyContact = "John Adams, 555-987-6543",
                    BankDetails = "Bank of America, Acc: XXXX1234",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };
                employees.Add(hrManager);

                // IT Director
                var itDirector = new Employee
                {
                    EmployeeId = Guid.NewGuid(),
                    FirstName = "Michael",
                    LastName = "Thompson",
                    Email = "michael.thompson@example.com",
                    Phone = "555-234-5678",
                    Address = "456 Oak Ave, Anytown, USA",
                    DateOfBirth = new DateTime(1975, 8, 22),
                    HireDate = new DateTime(2019, 6, 15),
                    Status = EmployeeStatus.Active,
                    DepartmentId = itDepartment.DepartmentId,
                    PositionId = itDirectorPosition.PositionId,
                    EmergencyContact = "Sarah Thompson, 555-876-5432",
                    BankDetails = "Chase Bank, Acc: XXXX5678",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };
                employees.Add(itDirector);

                // Finance Manager
                var financeManager = new Employee
                {
                    EmployeeId = Guid.NewGuid(),
                    FirstName = "Robert",
                    LastName = "Johnson",
                    Email = "robert.johnson@example.com",
                    Phone = "555-345-6789",
                    Address = "789 Maple Dr, Anytown, USA",
                    DateOfBirth = new DateTime(1978, 10, 10),
                    HireDate = new DateTime(2021, 1, 5),
                    Status = EmployeeStatus.Active,
                    DepartmentId = financeDepartment.DepartmentId,
                    PositionId = financeManagerPosition.PositionId,
                    EmergencyContact = "Mary Johnson, 555-765-4321",
                    BankDetails = "Wells Fargo, Acc: XXXX9012",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };
                employees.Add(financeManager);

                // Marketing Director
                var marketingDirector = new Employee
                {
                    EmployeeId = Guid.NewGuid(),
                    FirstName = "Lisa",
                    LastName = "Williams",
                    Email = "lisa.williams@example.com",
                    Phone = "555-456-7890",
                    Address = "101 Pine Ln, Anytown, USA",
                    DateOfBirth = new DateTime(1982, 3, 5),
                    HireDate = new DateTime(2022, 2, 20),
                    Status = EmployeeStatus.Active,
                    DepartmentId = marketingDepartment.DepartmentId,
                    PositionId = marketingDirectorPosition.PositionId,
                    EmergencyContact = "David Williams, 555-654-3210",
                    BankDetails = "Citibank, Acc: XXXX3456",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };
                employees.Add(marketingDirector);

                // Software Developer
                var softwareDeveloper = new Employee
                {
                    EmployeeId = Guid.NewGuid(),
                    FirstName = "Alex",
                    LastName = "Chen",
                    Email = "alex.chen@example.com",
                    Phone = "555-567-8901",
                    Address = "202 Cedar St, Anytown, USA",
                    DateOfBirth = new DateTime(1990, 7, 12),
                    HireDate = new DateTime(2023, 5, 10),
                    Status = EmployeeStatus.Active,
                    DepartmentId = itDepartment.DepartmentId,
                    PositionId = softwareDeveloperPosition.PositionId,
                    ManagerId = itDirector.EmployeeId,
                    EmergencyContact = "Emily Chen, 555-543-2109",
                    BankDetails = "Bank of America, Acc: XXXX7890",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };
                employees.Add(softwareDeveloper);

                // Set department managers
                hrDepartment.ManagerId = hrManager.EmployeeId;
                itDepartment.ManagerId = itDirector.EmployeeId;
                financeDepartment.ManagerId = financeManager.EmployeeId;
                marketingDepartment.ManagerId = marketingDirector.EmployeeId;

                // Update departments
                dbContext.Departments.UpdateRange(departments);
                await dbContext.Employees.AddRangeAsync(employees);
                await dbContext.SaveChangesAsync();

                // Seed salaries
                logger.LogInformation("Seeding salaries...");
                var salaries = new List<Salary>
                {
                    new Salary
                    {
                        SalaryId = Guid.NewGuid(),
                        EmployeeId = hrManager.EmployeeId,
                        Amount = 110000,
                        Currency = "USD",
                        PaymentType = PaymentType.Monthly,
                        EffectiveDate = hrManager.HireDate,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Salary
                    {
                        SalaryId = Guid.NewGuid(),
                        EmployeeId = itDirector.EmployeeId,
                        Amount = 135000,
                        Currency = "USD",
                        PaymentType = PaymentType.Monthly,
                        EffectiveDate = itDirector.HireDate,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Salary
                    {
                        SalaryId = Guid.NewGuid(),
                        EmployeeId = financeManager.EmployeeId,
                        Amount = 120000,
                        Currency = "USD",
                        PaymentType = PaymentType.Monthly,
                        EffectiveDate = financeManager.HireDate,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Salary
                    {
                        SalaryId = Guid.NewGuid(),
                        EmployeeId = marketingDirector.EmployeeId,
                        Amount = 125000,
                        Currency = "USD",
                        PaymentType = PaymentType.Monthly,
                        EffectiveDate = marketingDirector.HireDate,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    new Salary
                    {
                        SalaryId = Guid.NewGuid(),
                        EmployeeId = softwareDeveloper.EmployeeId,
                        Amount = 95000,
                        Currency = "USD",
                        PaymentType = PaymentType.Monthly,
                        EffectiveDate = softwareDeveloper.HireDate,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    }
                };

                await dbContext.Salaries.AddRangeAsync(salaries);
                await dbContext.SaveChangesAsync();

                // Seed Pay Periods
                logger.LogInformation("Seeding pay periods...");
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var payPeriods = new List<PayPeriod>();

                for (int i = 0; i < 3; i++)
                {
                    var startDate = currentDate.AddMonths(-i);
                    var endDate = startDate.AddDays(DateTime.DaysInMonth(startDate.Year, startDate.Month) - 1);
                    var paymentDate = endDate.AddDays(5); // 5 days after the end of the month

                    var payPeriod = new PayPeriod
                    {
                        PayPeriodId = Guid.NewGuid(),
                        StartDate = startDate,
                        EndDate = endDate,
                        PaymentDate = paymentDate,
                        Status = i == 0 ? PayPeriodStatus.Open : PayPeriodStatus.Closed,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    };

                    payPeriods.Add(payPeriod);
                }

                await dbContext.PayPeriods.AddRangeAsync(payPeriods);
                await dbContext.SaveChangesAsync();

                // Seed Users
                logger.LogInformation("Seeding users...");
                // Sample password hash for "password123" (for demo only, use a real hasher in production)
                string demoPasswordHash = "$2a$11$QWERTYUIOPASDFGHJKLZXCVBNM1234567890qwertyuiopasdfgh";

                // Admin user (not tied to employee)
                var adminUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = demoPasswordHash,
                    Role = "Admin",
                    EmployeeId = null
                };

                // Employee user (tied to first employee, e.g., HR Manager)
                var employeeUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = hrManager.FirstName.ToLower(),
                    Email = hrManager.Email,
                    PasswordHash = demoPasswordHash,
                    Role = "Employee",
                    EmployeeId = hrManager.EmployeeId
                };

                await dbContext.Users.AddRangeAsync(adminUser, employeeUser);
                await dbContext.SaveChangesAsync();

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
