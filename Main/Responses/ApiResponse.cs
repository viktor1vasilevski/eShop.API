using Main.Enums;

namespace eShop.Main.Responses;

public class ApiResponse<T>  where T : class
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public NotificationType NotificationType { get; set; }
    public string? Location { get; set; }
    public int? TotalCount { get; set; }
}
