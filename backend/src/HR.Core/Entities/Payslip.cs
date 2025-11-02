using System;
using System.Collections.Generic;

namespace HR.Core.Entities
{
    public class Payslip
    {
        public Guid PayslipId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid PayPeriodId { get; set; }
        public decimal GrossPay { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }
        public DateTime GeneratedDate { get; set; }
        public PayslipStatus Status { get; set; } = PayslipStatus.Draft;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public Employee? Employee { get; set; }
        public PayPeriod? PayPeriod { get; set; }
        public ICollection<Deduction> Deductions { get; set; } = new List<Deduction>();
    }
}
