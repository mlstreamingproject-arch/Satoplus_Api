using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;

namespace MeuProxySsl
{
    public class SwaggerIgnoreFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, System.Web.Http.Description.IApiExplorer apiExplorer)
        {
            try
            {
                // Remove paths que possam causar problemas
                if (swaggerDoc.paths != null)
                {
                    var pathsToRemove = new List<string>();
                    
                    foreach (var path in swaggerDoc.paths)
                    {
                        // Remover caminhos com rotas aninhadas que possam causar conflitos
                        if (path.Key.Contains("{") && path.Key.Count(c => c == '{') > 2)
                        {
                            pathsToRemove.Add(path.Key);
                        }
                    }

                    foreach (var pathToRemove in pathsToRemove)
                    {
                        swaggerDoc.paths.Remove(pathToRemove);
                    }
                }

                // Log sucesso
                LogDebug("SwaggerIgnoreFilter applied successfully");
            }
            catch (System.Exception ex)
            {
                LogDebug($"SwaggerIgnoreFilter error: {ex.Message}");
            }
        }

        private static void LogDebug(string message)
        {
            try
            {
                var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                var logDir = System.IO.Path.Combine(baseDir, "logs");
                if (!System.IO.Directory.Exists(logDir))
                    System.IO.Directory.CreateDirectory(logDir);
                var logPath = System.IO.Path.Combine(logDir, "swagger_filter.log");
                System.IO.File.AppendAllText(logPath, $"[{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
            }
            catch { }
        }
    }
}
