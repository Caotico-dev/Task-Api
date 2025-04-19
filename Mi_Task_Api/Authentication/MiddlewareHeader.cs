using System.Net;

namespace Mi_Task_Api.Authentication
{
    public class MiddlewareHeader
    {
        private readonly RequestDelegate _next;
        private readonly ICheckBlackList _checkBlackList;
        public MiddlewareHeader(RequestDelegate next,ICheckBlackList checkBlackList)
        {
            _next = next;
            _checkBlackList = checkBlackList;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var authorization = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorization))
            {
                Console.WriteLine("[{0}]: Token detectado...", DateTime.Now);

                authorization = authorization.Replace("Bearer", "").Trim();

                if (_checkBlackList.IsBlackList(authorization))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    await context.Response.WriteAsync("Token inválido");

                    Console.WriteLine("[{0}]: Token inválido", DateTime.Now);

                    return;
                }

                Console.WriteLine("Count token: {0}", _checkBlackList.CountBlackList);
                Console.WriteLine("[{0}]: Token válido.", DateTime.Now);
            }

            await _next(context);
        }


    }
}
