namespace TTSBot.Commands;

public static class HandlerResult
{
    public static HandlerResult<string> Success(string result = "") => HandlerResult<string>.Success(result);
    public static HandlerResult<string> Error(string errorMessage) => HandlerResult<string>.Error(errorMessage);
}

public record HandlerResult<T>(bool IsSuccess, T Result, string ErrorMessage = "")
{
    public static HandlerResult<T> Success(T result) => new(IsSuccess: true, result);
    public new static HandlerResult<T> Error(string errorMessage) => new(IsSuccess: false, Result: default!,  errorMessage);
}