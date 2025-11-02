using System;

namespace HR.API.Models.Positions
{
    public class PositionDto
    {
        public Guid PositionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
