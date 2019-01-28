using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public class RequestSecurityMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestSecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var nonceQuery = context.Request.Query["Nonce"];
            var timestampQuery = context.Request.Query["Timestamp"];
            var signatureQuery = context.Request.Query["Signature"];

            var nonce = 0;

            if (string.IsNullOrWhiteSpace(nonceQuery) || !Int32.TryParse(nonceQuery, out nonce))
            {
                await context.Response.WriteAsync("Query string Nonce is required.");
                await _next(context);

                return;
            }

            var timestamp = 0;

            if (string.IsNullOrWhiteSpace(timestampQuery) || !Int32.TryParse(timestampQuery, out timestamp))
            {
                await context.Response.WriteAsync("Query string Timestamp is required.");
                await _next(context);

                return;
            }

            if (string.IsNullOrWhiteSpace(signatureQuery))
            {
                await context.Response.WriteAsync("Query string Signature is required.");
                await _next(context);

                return;
            }

            var requestData = new RequestData()
            {
                Nonce = nonce,
                Timestamp = timestamp,
                Signature = signatureQuery
            };

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }

    public static class RequestSecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestSecurity(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestSecurityMiddleware>();
        }
    }
}