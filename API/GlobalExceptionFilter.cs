using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Filters;

namespace MeuProxySsl
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            string exceptionMessage = $"[{DateTime.UtcNow:o}] Exception: {context.Exception?.Message}\nStack: {context.Exception?.StackTrace}\n";
            
            // Try logging to file with multiple fallback paths
            TryLogToFile(exceptionMessage);

            try
            {
                // Return error response with exception details (for debugging)
                var errorContent = new
                {
                    Message = "An error has occurred.",
                    Exception = context.Exception?.Message ?? "Unknown error",
                    StackTrace = context.Exception?.StackTrace ?? "No stack trace",
                    Timestamp = DateTime.UtcNow.ToString("o")
                };
                context.Response = context.Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    errorContent
                );
            }
            catch
            {
                try
                {
                    context.Response = context.Request.CreateErrorResponse(
                        HttpStatusCode.InternalServerError,
                        "Internal server error"
                    );
                }
                catch { }
            }
        }

        private static void TryLogToFile(string message)
        {
            string[] logDirCandidates = new string[]
            {
                // Try 1: ~/logs
                SafeMapPath("~/logs"),
                // Try 2: BaseDirectory/logs
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "logs"),
                // Try 3: System temp (as last resort)
                Path.Combine(Path.GetTempPath(), "SatoPlusLogs")
            };

            foreach (var logDir in logDirCandidates)
            {
                if (string.IsNullOrEmpty(logDir)) continue;
                
                try
                {
                    if (!Directory.Exists(logDir))
                        Directory.CreateDirectory(logDir);

                    var logFile = Path.Combine(logDir, "global_exceptions.log");
                    File.AppendAllText(logFile, message + "\n" + new string('-', 80) + "\n");
                    return; // Success, stop trying
                }
                catch
                {
                    // Try next candidate
                    continue;
                }
            }
        }

        private static string SafeMapPath(string path)
        {
            try
            {
                return HostingEnvironment.MapPath(path);
            }
            catch
            {
                return null;
            }
        }
    }
}
