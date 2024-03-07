namespace Vnoun.API.Exceptions;

public class AppException : Exception
{
    public AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
        Status = statusCode.ToString().StartsWith("4") ? "fail" : "error";
        IsOperational = true;
    }

    public int StatusCode { get; set; }
    public string Status { get; set; }
    public bool IsOperational { get; set; }
}