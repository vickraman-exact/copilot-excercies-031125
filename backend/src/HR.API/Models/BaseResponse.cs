namespace HR.API.Models
{
    public record BaseResponse
    {
        public bool Success { get; init; } = true;
        public string? Message { get; init; }
        public Guid? Id { get; init; }
    }
}
