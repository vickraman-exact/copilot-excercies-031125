using System;

namespace HR.Core.Entities
{
  public class User
  {
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // e.g., Admin, Employee
    public Guid? EmployeeId { get; set; } // Nullable for Admins not tied to Employee
    public Employee? Employee { get; set; }
  }
}
