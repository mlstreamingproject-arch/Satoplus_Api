using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Owin;
using Microsoft.Owin.Cors;
using System.Configuration;
using Swashbuckle.Application;

[assembly: Microsoft.Owin.OwinStartup(typeof(MeuProxySsl.Startup))]

namespace MeuProxySsl
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Log immediately to verify startup is being called
            WriteStartupLog("Startup.Configuration called at " + DateTime.UtcNow.ToString("o"));
            
            try
            {
                WriteStartupLog("Creating HttpConfiguration...");
                var config = new HttpConfiguration();
                WriteStartupLog("HttpConfiguration created");

                // Configure formatters - JSON only (remove XML)
                var jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
                config.Formatters.Clear();
                config.Formatters.Add(jsonFormatter);
                WriteStartupLog("JSON formatter configured as default");

                // Register global exception filter
                config.Filters.Add(new GlobalExceptionFilter());
                WriteStartupLog("GlobalExceptionFilter registered");

                var swaggerEnabled = bool.TryParse(ConfigurationManager.AppSettings["Swagger:Enabled"], out var value) && value;
                if (swaggerEnabled)
                {
                    SwaggerConfig.Register(config);
                }

                // Enable attribute routing and explicit proxy action route
                config.MapHttpAttributeRoutes();
                config.Routes.MapHttpRoute(
                    name: "ProxyActions",
                    routeTemplate: "proxy/{action}/{id}",
                    defaults: new { controller = "Proxy", id = RouteParameter.Optional }
                );
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                // Log each incoming HTTP request before routing
                app.Use(async (context, next) =>
                {
                    try
                    {
                        var requestPath = context.Request.Path.Value ?? string.Empty;
                        var method = context.Request.Method;
                        var query = context.Request.QueryString.Value ?? string.Empty;
                        var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
                        string logDir = null;
                        try { logDir = HostingEnvironment.MapPath("~/logs"); } catch { }
                        if (string.IsNullOrEmpty(logDir))
                            logDir = Path.Combine(baseDir, "logs");
                        if (!Directory.Exists(logDir))
                            Directory.CreateDirectory(logDir);
                        var path = Path.Combine(logDir, "request_pipeline.log");
                        File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {method} {requestPath} {query}\n");
                    }
                    catch { }

                    await next.Invoke();
                });

                // Allow CORS
                app.UseCors(CorsOptions.AllowAll);

                // Use Web API
                app.UseWebApi(config);

                try
                {
                    string logDir = null;
                    try
                    {
                        logDir = HostingEnvironment.MapPath("~/logs");
                    }
                    catch { }

                    if (string.IsNullOrEmpty(logDir))
                    {
                        var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
                        logDir = Path.Combine(baseDir, "logs");
                    }

                    if (!Directory.Exists(logDir))
                        Directory.CreateDirectory(logDir);

                    File.WriteAllText(Path.Combine(logDir, "startup_marker.txt"), $"Startup executed at {DateTime.UtcNow:o}");
                }
                catch { }
            }
            catch (Exception ex)
            {
                WriteStartupLog("STARTUP ERROR: " + ex.ToString());
                try
                {
                    string logDir = null;
                    try
                    {
                        logDir = HostingEnvironment.MapPath("~/logs");
                    }
                    catch { }

                    if (string.IsNullOrEmpty(logDir))
                    {
                        var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
                        logDir = Path.Combine(baseDir, "logs");
                    }

                    if (!Directory.Exists(logDir))
                        Directory.CreateDirectory(logDir);
                    File.WriteAllText(Path.Combine(logDir, "startup_error.txt"), ex.ToString());
                }
                catch { }
                throw;
            }
        }

        private static void WriteStartupLog(string message)
        {
            string[] logDirCandidates = new string[]
            {
                SafeMapPath("~/logs"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "logs"),
                Path.Combine(Path.GetTempPath(), "SatoPlusLogs")
            };

            foreach (var logDir in logDirCandidates)
            {
                if (string.IsNullOrEmpty(logDir)) continue;
                try
                {
                    if (!Directory.Exists(logDir))
                        Directory.CreateDirectory(logDir);
                    File.AppendAllText(Path.Combine(logDir, "startup.log"), $"[{DateTime.UtcNow:o}] {message}\n");
                    return;
                }
                catch { }
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
