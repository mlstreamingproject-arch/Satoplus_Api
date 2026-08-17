using System.Configuration;
using System.Web.Http;
using Swashbuckle.Application;
using System;
using System.IO;

namespace MeuProxySsl
{
    public static class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            try
            {
                var enabled = bool.TryParse(ConfigurationManager.AppSettings["Swagger:Enabled"], out var value) && value;
                if (!enabled)
                {
                    return;
                }

                config.EnableSwagger("swagger/{apiVersion}", c =>
                {
                    c.SingleApiVersion("v1", "SatoPlus API");
                    c.IgnoreObsoleteActions();
                    c.IgnoreObsoleteProperties();
                    // Filter para ignorar controllers problemáticos
                    c.DocumentFilter<SwaggerIgnoreFilter>();
                    // c.ApiKey("Authorization")
                    //     .Description("JWT Bearer token. Exemplo: Bearer {token}")
                    //     .Name("Authorization")
                    //     .In("header");
                    // c.IncludeXmlComments(GetXmlCommentsPath());
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("SatoPlus API - Swagger");
                    // c.EnableApiKeySupport("Authorization", "header");
                });
                
                LogSwaggerDebug("Swagger registered successfully");
            }
            catch (Exception ex)
            {
                LogSwaggerDebug($"Swagger registration error: {ex.Message} | {ex.StackTrace}");
                throw;
            }
        }

        private static void LogSwaggerDebug(string message)
        {
            try
            {
                var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                var logDir = Path.Combine(baseDir, "logs");
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);
                var logPath = Path.Combine(logDir, "swagger_debug.log");
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
            }
            catch { }
        }

        private static string GetXmlCommentsPath()
        {
            var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var fileName = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".xml";
            return System.IO.Path.Combine(baseDirectory, fileName);
        }
    }
}
