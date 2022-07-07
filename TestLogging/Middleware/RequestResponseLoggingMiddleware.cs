using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;

namespace TestLogging.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private static string TraceId;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private static string GenerateRequestTraceId()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task Invoke(HttpContext context)
        {
            TraceId = GenerateRequestTraceId();
            
            _logger.Info("Request" + Environment.NewLine
                                             + "============================================" + Environment.NewLine
                                             + "Host: " + context.Request.Host + Environment.NewLine
                                             + "Path: " + context.Request.Path + Environment.NewLine
                                             + "Method: " + context.Request.Method + Environment.NewLine
                                             + "Protocol: " + context.Request.Protocol + Environment.NewLine
                                             + "TraceId: " + TraceId + Environment.NewLine
                                             + "============================================");

            await _next.Invoke(context);

            _logger.Info("Response" + Environment.NewLine
                                    + "============================================" + Environment.NewLine
                                    + "Host: " + context.Request.Host + Environment.NewLine
                                    + "Path: " + context.Request.Path + Environment.NewLine
                                    + "Method: " + context.Request.Method + Environment.NewLine
                                    + "Protocol: " + context.Request.Protocol + Environment.NewLine
                                    + "Status code: " + context.Response.StatusCode + Environment.NewLine
                                    + "TraceId: " + TraceId + Environment.NewLine
                                    + "============================================");
        }
    }
}