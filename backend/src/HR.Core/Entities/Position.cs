using System;
using System.Collections.Generic;

namespace HR.Core.Entities
{
    public class Position
    {
        public Guid PositionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public Guid DepartmentId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public Department? Department { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
