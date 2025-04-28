using JobTrackingAPI.Enums;

namespace JobTrackingAPI.Contracts.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ErrorType? ErrorType { get; set; }
    public string? ErrorMessage { get; }

    protected Result(
        bool isSuccess, 
        ErrorType? errorType = null,
        string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorType = errorType;
        ErrorMessage = errorMessage;
    }

    public static Result Success() 
        => new Result(true);

    public static Result Failure(ErrorType errorType, string errorMessage) 
        => new Result(false, errorType, errorMessage);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(T data)
        : base(true)
    {
        Data = data;
    }

    private Result(
        ErrorType errorType,
        string errorMessage)
        : base(false, errorType, errorMessage)
    {
        Data = default;
    }

    public static Result<T> Success(T data)
        => new Result<T>(data);

    public static new Result<T> Failure(ErrorType errorType, string errorMessage)
        => new Result<T>(errorType, errorMessage);
}