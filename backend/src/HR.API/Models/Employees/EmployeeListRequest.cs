using HR.Core.Entities;

namespace HR.API.Models.Employees
{
    public class EmployeeListRequest
    {
        public string? SearchTerm { get; set; }
        public EmployeeStatus? Status { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? PositionId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
