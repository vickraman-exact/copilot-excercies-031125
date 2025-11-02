namespace HR.API.Models.Positions
{
    public class PositionListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
