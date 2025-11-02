using System;

namespace HR.Core.Entities
{
    public class Salary
    {
        public Guid SalaryId { get; set; }
        public Guid EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentType PaymentType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public Employee? Employee { get; set; }
    }
}
