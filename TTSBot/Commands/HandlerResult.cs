public record HandlerResult(bool IsSuccess, string ErrorMessage = "")
{
    public static HandlerResult Success() => new(IsSuccess: true);
    public static HandlerResult Error(string errorMessage) => new(IsSuccess: false, errorMessage);
}