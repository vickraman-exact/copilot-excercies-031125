using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HR.API.Controllers;
using HR.API.Models;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Tests;

public class SettingsControllerIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SettingsControllerIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                
                // Configure test connection string (not actually used since we use InMemory)
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefualtConnection"] = "Data Source=:memory:"
                    });
                });
                
                builder.ConfigureServices(services =>
                {
                    services.AddDbContext<HRPayDezkDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("SettingsTestDb_" + Guid.NewGuid());
                    });
                });
            });

        // Seed test data
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
            
            var department = new Department
            {
                DepartmentId = Guid.Parse("00000000-0000-0000-0000-000000000010"),
                Name = "Test Department",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            
            var position = new Position
            {
                PositionId = Guid.Parse("00000000-0000-0000-0000-000000000020"),
                Title = "Test Position",
                DepartmentId = department.DepartmentId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            
            var employee = new Employee
            {
                EmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                HireDate = DateTime.UtcNow.AddYears(-2),
                Status = EmployeeStatus.Active,
                DepartmentId = department.DepartmentId,
                PositionId = position.PositionId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
            
            db.Departments.Add(department);
            db.Positions.Add(position);
            db.Employees.Add(employee);
            db.SaveChanges();
        }

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task UpdateUserProfile_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new UpdateProfileRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Phone = "9876543210"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/settings/update-profile", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Message.Should().Be("Profile updated successfully");

        // Verify the data was actually updated in the database
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
        var updatedEmployee = await dbContext.Employees.FindAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        
        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.FirstName.Should().Be("Jane");
        updatedEmployee.LastName.Should().Be("Smith");
        updatedEmployee.Email.Should().Be("jane.smith@example.com");
        updatedEmployee.Phone.Should().Be("9876543210");
    }

    [Fact]
    public async Task UpdateUserProfile_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange - Create a factory with an empty database
        using var emptyFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<HRPayDezkDbContext>(options =>
                {
                    options.UseInMemoryDatabase("EmptyTestDb_" + Guid.NewGuid());
                });
            });
        });

        using var client = emptyFactory.CreateClient();
        var request = new UpdateProfileRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Phone = "9876543210"
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
    public async Task UpdateUserProfile_WithEmptyFields_UpdatesSuccessfully()
    {
        // Arrange
        var request = new UpdateProfileRequest
        {
            FirstName = "",
            LastName = "",
            Email = "",
            Phone = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/settings/update-profile", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();

        // Verify the data was updated with empty strings
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
        var updatedEmployee = await dbContext.Employees.FindAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        
        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.FirstName.Should().Be("");
        updatedEmployee.LastName.Should().Be("");
        updatedEmployee.Email.Should().Be("");
        updatedEmployee.Phone.Should().Be("");
    }

    [Fact]
    public async Task UpdateUserProfile_UpdatesModifiedTimestamp()
    {
        // Arrange
        // Get the original modified timestamp
        DateTime originalModifiedTime;
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
            var employee = await dbContext.Employees.FindAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            originalModifiedTime = employee!.Modified;
        }

        // Wait a small amount to ensure timestamp difference
        await Task.Delay(100);

        var request = new UpdateProfileRequest
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = "updated@example.com",
            Phone = "1111111111"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/settings/update-profile", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify the Modified timestamp was updated
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDbContext = verifyScope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
        var updatedEmployee = await verifyDbContext.Employees.FindAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        
        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.Modified.Should().BeAfter(originalModifiedTime);
    }

    [Fact]
    public async Task UpdateUserProfile_WithSpecialCharacters_UpdatesSuccessfully()
    {
        // Arrange
        var request = new UpdateProfileRequest
        {
            FirstName = "José",
            LastName = "O'Brien-Smith",
            Email = "jose.o'brien@example.com",
            Phone = "+1 (555) 123-4567"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/settings/update-profile", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();

        // Verify the data with special characters was saved correctly
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HRPayDezkDbContext>();
        var updatedEmployee = await dbContext.Employees.FindAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        
        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.FirstName.Should().Be("José");
        updatedEmployee.LastName.Should().Be("O'Brien-Smith");
        updatedEmployee.Email.Should().Be("jose.o'brien@example.com");
        updatedEmployee.Phone.Should().Be("+1 (555) 123-4567");
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}
