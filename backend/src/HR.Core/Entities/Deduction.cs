using System;

namespace HR.Core.Entities
{
    public class Deduction
    {
        public Guid DeductionId { get; set; }
        public Guid PayslipId { get; set; }
        public DeductionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public Payslip? Payslip { get; set; }
    }
}
