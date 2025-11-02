namespace HR.API.Models.Employees
{
    public class UpdateEmployeeRequest : CreateEmployeeRequest
    {
        public DateTime? TerminationDate { get; set; }
    }
}
