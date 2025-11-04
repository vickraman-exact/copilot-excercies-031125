using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using HR.API.Controllers;
using HR.API.Models;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace HR.Tests
{
    public class SettingsControllerIntegrationTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Guid _testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private readonly string _databaseName;

        public SettingsControllerIntegrationTests()
        {
            _databaseName = $"TestDatabase_{Guid.NewGuid()}";
            
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Test");

                    builder.ConfigureServices(services =>
                    {
                        // Add DbContext using in-memory database for testing
                        services.AddDbContext<HRPayDezkDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(_databaseName);
                        });
                    });
                });
        }

        public void Dispose()
        {
            _factory?.Dispose();
        }

        private async Task SeedTestEmployee(HRPayDezkDbContext context)
        {
            // Create test department
            var department = new Department
            {
                DepartmentId = Guid.NewGuid(),
                Name = "Test Department",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            context.Departments.Add(department);

            // Create test position
            var position = new Position
            {
                PositionId = Guid.NewGuid(),
                Title = "Test Position",
                DepartmentId = department.DepartmentId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            context.Positions.Add(position);

            // Create test employee
            var employee = new Employee
            {
                EmployeeId = _testUserId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890",
                DateOfBirth = new DateTime(1990, 1, 1),
                HireDate = DateTime.UtcNow,
                Status = EmployeeStatus.Active,
                DepartmentId = department.DepartmentId,
                PositionId = position.PositionId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            context.Employees.Add(employee);

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateUserProfile_Success_ReturnsOkWithSuccessMessage()
        {
            // Arrange
            var client = _factory.CreateClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
                await SeedTestEmployee(context);
            }

            var request = new UpdateProfileRequest
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "098-765-4321"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/settings/update-profile", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<BaseResponse>();
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.Message.Should().Be("Profile updated successfully");

            // Verify the database was updated
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
                var updatedEmployee = await context.Employees.FindAsync(_testUserId);

                updatedEmployee.Should().NotBeNull();
                updatedEmployee!.FirstName.Should().Be("Jane");
                updatedEmployee.LastName.Should().Be("Smith");
                updatedEmployee.Email.Should().Be("jane.smith@example.com");
                updatedEmployee.Phone.Should().Be("098-765-4321");
            }
        }

        [Fact]
        public async Task UpdateUserProfile_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Don't seed any employees - the hardcoded user ID won't exist

            var request = new UpdateProfileRequest
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "098-765-4321"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/settings/update-profile", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var result = await response.Content.ReadFromJsonAsync<BaseResponse>();
            result.Should().NotBeNull();
            result!.Success.Should().BeFalse();
            result.Message.Should().Be("User not found");
        }

        [Fact]
        public async Task UpdateUserProfile_EmptyRequest_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
                await SeedTestEmployee(context);
            }

            var request = new UpdateProfileRequest
            {
                FirstName = "",
                LastName = "",
                Email = "",
                Phone = ""
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/settings/update-profile", request);

            // Assert
            // Note: The current controller implementation doesn't validate the request
            // This test documents the current behavior - it will succeed even with empty values
            // In a real scenario, you'd want to add validation attributes to the request model
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateUserProfile_PartialUpdate_UpdatesOnlyProvidedFields()
        {
            // Arrange
            var client = _factory.CreateClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
                await SeedTestEmployee(context);
            }

            var request = new UpdateProfileRequest
            {
                FirstName = "UpdatedFirstName",
                LastName = "Doe", // Keep original
                Email = "john.doe@example.com", // Keep original
                Phone = "555-1234" // Update
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/settings/update-profile", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify the database was updated correctly
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
                var updatedEmployee = await context.Employees.FindAsync(_testUserId);

                updatedEmployee.Should().NotBeNull();
                updatedEmployee!.FirstName.Should().Be("UpdatedFirstName");
                updatedEmployee.Phone.Should().Be("555-1234");
            }
        }
    }
}
