namespace Mi_Task_Api.Authentication
{
    public class MiddlewareHeader
    {
        private readonly RequestDelegate _next;
        public MiddlewareHeader(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            throw new NotImplementedException();
        }

    }
}
