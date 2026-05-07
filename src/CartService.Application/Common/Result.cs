namespace CartService.Application.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, T? value, string? error, int statusCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result<T> Ok(T value) => new(true, value, null, 200);
    public static Result<T> Fail(string error, int statusCode = 400) => new(false, default, error, statusCode);
}

public sealed class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, string? error, int statusCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result Ok() => new(true, null, 200);
    public static Result Fail(string error, int statusCode = 400) => new(false, error, statusCode);
}
