using System.Diagnostics;

namespace E_PharmaHub.Middleware
{
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            // Store start time to display in header
            context.Response.OnStarting(() =>
            {
                sw.Stop();
                var responseTimeMs = sw.ElapsedMilliseconds;
                
                // Add custom header to the response so the user can see it in Postman/Swagger
                if (!context.Response.Headers.ContainsKey("X-Response-Time-ms"))
                {
                    context.Response.Headers.Append("X-Response-Time-ms", responseTimeMs.ToString());
                }

                _logger.LogInformation($"Backend Performance: {context.Request.Method} {context.Request.Path} executed in {responseTimeMs}ms");
                
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
