namespace SCM.Application.Common.Models;

public class ServiceResult<T>
{
    public bool    IsSuccess { get; private init; }
    public T?      Value     { get; private init; }
    public string? Error     { get; private init; }

    public static ServiceResult<T> Success(T value)   => new() { IsSuccess = true,  Value = value };
    public static ServiceResult<T> Failure(string err) => new() { IsSuccess = false, Error = err   };
}
