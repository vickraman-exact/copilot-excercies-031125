using System;

namespace HR.API.Models.Departments
{
    public class UpdateDepartmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ManagerId { get; set; }
        public Guid? ParentDepartmentId { get; set; }
    }
}
