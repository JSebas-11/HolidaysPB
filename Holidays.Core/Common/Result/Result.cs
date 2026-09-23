namespace HolidaysPB.Core.Common.Result;

public class Result {
    private readonly bool _success;

    public bool IsSuccess => _success;
    public bool IsFailure => !_success;
    public AppError? Error { get; }

    protected Result(bool success, AppError? error = null) {
        Error = error;
        _success = success;
    }

    public static Result Ok() => new (true);
    public static Result Fail(AppError error) => new (false, error);
}

public sealed class Result<T> : Result {
    private readonly T? _value;

    public T? Value => IsSuccess ? _value 
        : throw new InvalidOperationException("Cannot access Value of a failed Result.");

    private Result(T value) : base(true) => _value = value;
    private Result(AppError error) : base(false, error) {}

    public static Result<T> Ok(T value) => new (value);
    public static new Result<T> Fail(AppError error) => new (error);
}