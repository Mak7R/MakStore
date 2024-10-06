using Microsoft.AspNetCore.Mvc;

namespace EmployeeWebClient.Services;

public class OperationResult : OperationResult<object>;

public class OperationResult<T>
{
    public bool IsSuccessful => StatusCode is >= 200 and < 300;
    public int StatusCode { get; set; }= 200;
    public T? Result { get; set; }
    public ValidationProblemDetails? ProblemDetails { get; set; }
}