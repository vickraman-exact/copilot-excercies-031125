using System;

namespace HR.API.Models.Positions
{
    public class CreatePositionRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
