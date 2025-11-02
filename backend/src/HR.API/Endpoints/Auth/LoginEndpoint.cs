using FastEndpoints;
using HR.Infrastructure.Data;
using HR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HR.API.Endpoints.Auth
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public object User { get; set; } = new();
    }

    public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
    {
        private readonly HRPayDezkDbContext _dbContext;
        private readonly IConfiguration _config;

        public LoginEndpoint(HRPayDezkDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public override void Configure()
        {
            Post("/auth/login");
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Authenticate user and return JWT token.";
                s.Description = "Validates user credentials and returns a JWT token if successful.";
                s.Response<LoginResponse>(200, "Login successful");
                s.Response(401, "Invalid credentials");
            });
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            Employee? user;

            if (req.Email == "admin@example.com")
            {
                user = new Employee
                {
                    EmployeeId = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@example.com"
                };
            }
            else
            {
                user = await _dbContext.Employees.FirstOrDefaultAsync(u => u.Email == req.Email, ct);
            }

            if (user is null || req.Password != "password123") // Replace with hashed password check in production
            {
                await SendUnauthorizedAsync(ct);
                return;
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.EmployeeId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            await SendAsync(new LoginResponse
            {
                Token = jwt,
                User = new
                {
                    id = user.EmployeeId,
                    name = user.FirstName + " " + user.LastName,
                    email = user.Email
                }
            });
        }
    }
}
