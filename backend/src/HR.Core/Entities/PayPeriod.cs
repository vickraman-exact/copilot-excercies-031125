using System;
using System.Collections.Generic;

namespace HR.Core.Entities
{
    public class PayPeriod
    {
        public Guid PayPeriodId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public PayPeriodStatus Status { get; set; } = PayPeriodStatus.Draft;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
    }
}
