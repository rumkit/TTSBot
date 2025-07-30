namespace TTSBot.Commands;

public record HandlerResult(bool IsSuccess, string ErrorMessage = "")
{
    public static HandlerResult Success() => new(IsSuccess: true);
    public static HandlerResult Error(string errorMessage) => new(IsSuccess: false, errorMessage);
}

public record HandlerResult<T>(bool IsSuccess, T Result, string ErrorMessage = "")
    : HandlerResult(IsSuccess, ErrorMessage)
{
    public static HandlerResult<T> Success(T result) => new(IsSuccess: true, result);
    public new static HandlerResult<T> Error(string errorMessage) => new(IsSuccess: false, Result: default!,  errorMessage);
}