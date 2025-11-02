using System;
using System.Collections.Generic;

namespace HR.API.Models.Departments
{
    public class DepartmentDto
    {
        public Guid DepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public Guid? ParentDepartmentId { get; set; }
        public string? ParentDepartmentName { get; set; }
        public List<DepartmentDto>? SubDepartments { get; set; }
        public int EmployeeCount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
