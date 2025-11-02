using HR.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace HR.API.Models.Employees
{
    public class CreateEmployeeRequest
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        public Guid PositionId { get; set; }

        public Guid? ManagerId { get; set; }

        public string? EmergencyContact { get; set; }

        public string? BankDetails { get; set; }
    }
}
