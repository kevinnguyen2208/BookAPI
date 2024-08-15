namespace API.Middleware
{
    public class HeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Set response headers
            context.Response.Headers.AccessControlAllowOrigin = "*";
            context.Response.Headers.CacheControl = "public,max-age=0,must-revalidate";
            context.Response.Headers["Cf-Cache-Status"] = "DYNAMIC";
            context.Response.Headers["Cf-Ray"] = "8b35e472ad2e17cf-MEL";
            context.Response.Headers["Date"] = DateTime.UtcNow.ToString("r");
            context.Response.Headers["Nel"] = "{\"success_fraction\":0,\"report_to\":\"cf-nel\",\"max_age\":604800}";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Report-To"] = "{\"endpoints\":[{\"url\":\"https://a.nel.cloudflare.com/report/v4?s=smzjjVOIW9tn%2BlRGZXmBlyRDRcS6fJosOZywHhxMB8y8XKc5IgR6Aqoo5HwzixTSPdeQipI740QW3tPYm%2FrAQZH11nZtyZZouZsPsFWcXFKAHdAZxa%2FG9z6B52ac3W8cBonMCVeup5W3\"}],\"group\":\"cf-nel\",\"max_age\":604800}";
            context.Response.Headers["Server"] = "cloudflare";
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
            context.Response.Headers["Vary"] = "Accept-Encoding";
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            await _next(context); // Call the next middleware in the pipeline
        }
    }
}