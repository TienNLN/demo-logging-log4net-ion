using System;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;

namespace TestLogging.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        
        private readonly ILog _requestResponseLogger = LogManager.GetLogger("RequestResponseLogger");
        private readonly ILog _exceptionLogger = LogManager.GetLogger("ExceptionLogger");

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
            var TraceId = GenerateRequestTraceId();

            LogicalThreadContext.Properties["TraceId"] = TraceId;
            
            _requestResponseLogger.Info("Request" + Environment.NewLine
                                             + "============================================" + Environment.NewLine
                                             + "Host: " + context.Request.Host + Environment.NewLine
                                             + "Path: " + context.Request.Path + Environment.NewLine
                                             + "Method: " + context.Request.Method + Environment.NewLine
                                             + "Protocol: " + context.Request.Protocol + Environment.NewLine
                                             + "TraceId: " + TraceId + Environment.NewLine
                                             + "============================================");

            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(ex, context, TraceId);
            }

            _requestResponseLogger.Info("Response" + Environment.NewLine
                                                   + "============================================" + Environment.NewLine
                                                   + "Host: " + context.Request.Host + Environment.NewLine
                                                   + "Path: " + context.Request.Path + Environment.NewLine
                                                   + "Method: " + context.Request.Method + Environment.NewLine
                                                   + "Protocol: " + context.Request.Protocol + Environment.NewLine
                                                   + "Status code: " + context.Response.StatusCode + Environment.NewLine
                                                   + "TraceId: " + TraceId + Environment.NewLine
                                                   + "============================================");
        }
        
        private Task HandleExceptionAsync(Exception ex, HttpContext context, string TraceId)
        {
            var errorMessage = "Exception" + Environment.NewLine
                                           + "============================================" + Environment.NewLine
                                           + "Data: " + ex.Data + Environment.NewLine
                                           + "InnerException: " + ex.InnerException + Environment.NewLine
                                           + "StackTrace: " + ex.StackTrace + Environment.NewLine
                                           + "Source: " + ex.Source + Environment.NewLine
                                           + "HelpLink: " + ex.HelpLink + Environment.NewLine
                                           + "TraceId: " + TraceId + Environment.NewLine
                                           + "============================================";
            
            _exceptionLogger.Error(errorMessage);
            
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(errorMessage);
        }
    }
}