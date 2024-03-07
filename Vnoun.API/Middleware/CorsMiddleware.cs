namespace Vnoun.API.Middleware;

public class CorsMiddleware
{
    private readonly RequestDelegate _next;
    public CorsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        httpContext.Response.Headers["Access-Control-Allow-Origin"] = "http://127.0.0.1:3333";
        httpContext.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        httpContext.Response.Headers["Access-Control-Allow-Methods"] = "HEAD,GET,PUT,POST,DELETE";

        return _next(httpContext);
    }
}