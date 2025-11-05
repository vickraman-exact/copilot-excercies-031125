using System;

namespace HR.Core.Entities
{
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }
        public string TerminationDate { get; set; }
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public Guid? ManagerId { get; set; }
        public string? EmergencyContact { get; set; }
        public string? BankDetails { get; set; } // Will need encryption when implemented
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public Department? Department { get; set; }
        public Position? Position { get; set; }
        public Employee? Manager { get; set; }
        public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();
        // Navigation property for User
        public User? User { get; set; }
    }
}
