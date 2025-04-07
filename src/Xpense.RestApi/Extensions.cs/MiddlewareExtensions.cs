using Microsoft.AspNetCore.Builder;
using Xpense.RestApi.Middlewares;

namespace Xpense.RestApi.Extensions.cs;

public static class MiddlewareExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}