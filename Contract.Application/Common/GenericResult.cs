namespace Contract.Application.Common;

public class GenericResult<T>
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public T? Value { get; }

    private GenericResult(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private GenericResult(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static GenericResult<T> Success(T value) => new GenericResult<T>(value);

    public static GenericResult<T> Failure(string error) => new GenericResult<T>(error);
}
}