using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ClassLib.Middlewares
{
    public class TokenExpiredMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenExpiredMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            // check error 401 unauthorized and header Token-Expired
            if(context.Response.StatusCode == 401 && context.Response.Headers.ContainsKey("Token-Expired"))
            {
                var response = new
                {
                    message = "Token Expired. Please refresh token"
                };

                context.Response.ContentType = " apllication/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }

            
        }
    }
}
