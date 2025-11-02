using Microsoft.AspNetCore.Mvc;
using HR.Core.Entities;
using HR.Infrastructure.Data;
using HR.API.Models;

namespace HR.API.Controllers
{
  [ApiController]
  [Route("api/settings")]
  public class SettingsController : ControllerBase
  {
    private readonly HRPayDezkDbContext _dbContext;

    public SettingsController(HRPayDezkDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    [HttpPost("update-profile")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileRequest request)
    {
      var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

      var user = await _dbContext.Employees.FindAsync(userId);

      if (user == null)
      {
        return NotFound(new BaseResponse
        {
          Success = false,
          Message = "User not found"
        });
      }

      // Update user profile without CSRF protection
      user.FirstName = request.FirstName;
      user.LastName = request.LastName;
      user.Email = request.Email; // Note: This could be exploited to take over accounts
      user.Phone = request.Phone;
      user.Modified = DateTime.UtcNow;

      await _dbContext.SaveChangesAsync();

      return Ok(new BaseResponse
      {
        Success = true,
        Message = "Profile updated successfully"
      });
    }
  }

  public class UpdateProfileRequest
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
  }
}
