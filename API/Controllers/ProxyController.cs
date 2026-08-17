using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MeuProxySsl.Data;
using MeuProxySsl.Security;


namespace MeuProxySsl.Controllers
{
    public class ProxyController : ApiController
    {
        private static readonly HttpClient _httpClient;
        private static readonly string _baseUrl;

        static ProxyController()
        {
            try
            {
                WriteDebugLog("controller_init.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ProxyController static constructor started\n", out var _);
                
                _baseUrl = ConfigurationManager.AppSettings["Upstream:BaseUrl"] ?? string.Empty;

                WriteDebugLog("controller_init.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] BaseUrl: {!string.IsNullOrEmpty(_baseUrl)}\n", out var _);

                // Set TLS versions globally
                try { ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; } catch { }

                // For .NET Framework 4.8: do not disable certificate validation — keep default behavior

                var handler = new HttpClientHandler();
                handler.AllowAutoRedirect = true;
                handler.MaxAutomaticRedirections = 10;
                
                try { handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls; } catch { }

                // Do not override handler.ServerCertificateCustomValidationCallback — enforce certificate validation

                if (!string.IsNullOrEmpty(_baseUrl))
                {
                    _httpClient = new HttpClient(handler)
                    {
                        BaseAddress = new Uri(_baseUrl, UriKind.Absolute),
                        Timeout = TimeSpan.FromSeconds(30)
                    };
                }
                else
                {
                    _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
                }

                WriteDebugLog("controller_init.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ProxyController static constructor completed\n", out var _);
            }
            catch (Exception ex)
            {
                WriteDebugLog("controller_init.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ProxyController static constructor failed: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n", out var _);
                throw;
            }
        }

        private async Task<IHttpActionResult> ForwardRequest(string endpoint, string queryString = "")
        {
            var baseTrim = _baseUrl?.TrimEnd('/') ?? string.Empty;
            var targetUri = string.IsNullOrEmpty(baseTrim) ? endpoint : baseTrim + "/" + endpoint;

            // If running under HttpContext (IIS) use that; otherwise use ApiController.Request (self-host)
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                if (!string.IsNullOrEmpty(queryString))
                {
                    if (targetUri.Contains("?"))
                        targetUri += "&" + queryString;
                    else
                        targetUri += "?" + queryString;
                }
                else if (!targetUri.Contains("?") && !string.IsNullOrEmpty(request.QueryString.ToString()))
                {
                    var qs = request.QueryString.ToString();
                    targetUri += "?" + qs;
                }

                var message = new HttpRequestMessage(new HttpMethod(request.HttpMethod), targetUri);

                // Copy headers, excluding problematic ones
                var headersToExclude = new[] { "Host", "Connection", "Content-Length", "Transfer-Encoding", "Expect" };
                foreach (var key in request.Headers.AllKeys)
                {
                    if (headersToExclude.Contains(key, StringComparer.OrdinalIgnoreCase)) continue;
                    
                    var values = request.Headers.GetValues(key);
                    if (!message.Headers.TryAddWithoutValidation(key, values))
                    {
                        if (message.Content == null) message.Content = new StringContent(string.Empty);
                        message.Content.Headers.TryAddWithoutValidation(key, values);
                    }
                }

                if (request.InputStream != null && request.ContentLength > 0)
                {
                    request.InputStream.Position = 0;
                    var content = new StreamContent(request.InputStream);
                    if (!string.IsNullOrEmpty(request.ContentType))
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.ContentType);
                    message.Content = content;
                }

                using (var resp = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead))
                {
                    var responseContent = await resp.Content.ReadAsByteArrayAsync();
                    var responseText = Encoding.UTF8.GetString(responseContent);
                    
                    // Log response details
                    try
                    {
                        var logPath = GetLogFilePath("proxy_response.log");
                        File.AppendAllText(logPath, $"\n[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {endpoint}\n");
                        File.AppendAllText(logPath, $"URL: {targetUri}\n");
                        File.AppendAllText(logPath, $"Status: {resp.StatusCode}\n");
                        File.AppendAllText(logPath, $"Content-Length: {responseContent.Length}\n");
                        File.AppendAllText(logPath, $"Response: {responseText.Substring(0, Math.Min(2000, responseText.Length))}\n");
                    }
                    catch { }
                    
                    var result = new System.Net.Http.HttpResponseMessage(resp.StatusCode)
                    {
                        Content = new ByteArrayContent(responseContent)
                    };
                    foreach (var header in resp.Headers)
                        result.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    foreach (var header in resp.Content.Headers)
                        result.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);

                    return ResponseMessage(result);
                }
            }

            // Self-host / OWIN environment: use ApiController.Request
            if (!string.IsNullOrEmpty(queryString))
            {
                if (targetUri.Contains("?"))
                    targetUri += "&" + queryString;
                else
                    targetUri += "?" + queryString;
            }
            else if (!targetUri.Contains("?") && Request != null && Request.RequestUri != null && !string.IsNullOrEmpty(Request.RequestUri.Query))
            {
                var qs = Request.RequestUri.Query.TrimStart('?');
                targetUri += "?" + qs;
            }

            var apiReq = this.Request;
            HttpRequestMessage msg;
            if (apiReq == null)
            {
                msg = new HttpRequestMessage(HttpMethod.Get, targetUri);
            }
            else
            {
                msg = new HttpRequestMessage(apiReq.Method, targetUri);

                if (apiReq.Headers != null)
                {
                    var headersToExclude = new[] { "Host", "Connection", "Content-Length", "Transfer-Encoding", "Expect" };
                    foreach (var header in apiReq.Headers)
                    {
                        if (headersToExclude.Contains(header.Key, StringComparer.OrdinalIgnoreCase)) continue;
                        
                        if (!msg.Headers.TryAddWithoutValidation(header.Key, header.Value))
                        {
                            if (msg.Content == null) msg.Content = new StringContent(string.Empty);
                            msg.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }
                }

                if (apiReq.Content != null)
                {
                    var bytes = await apiReq.Content.ReadAsByteArrayAsync();
                    if (bytes != null && bytes.Length > 0)
                    {
                        msg.Content = new ByteArrayContent(bytes);
                        if (apiReq.Content.Headers != null)
                            foreach (var h in apiReq.Content.Headers)
                                msg.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
                    }
                }
            }

            using (var resp = await _httpClient.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead))
            {
                var responseContent = await resp.Content.ReadAsByteArrayAsync();
                var responseText = Encoding.UTF8.GetString(responseContent);
                
                // Log response details
                try
                {
                    var logPath = GetLogFilePath("proxy_response.log");
                    File.AppendAllText(logPath, $"\n[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {endpoint} (OWIN)\n");
                    File.AppendAllText(logPath, $"URL: {targetUri}\n");
                    File.AppendAllText(logPath, $"Status: {resp.StatusCode}\n");
                    File.AppendAllText(logPath, $"Content-Length: {responseContent.Length}\n");
                    File.AppendAllText(logPath, $"Response: {responseText.Substring(0, Math.Min(2000, responseText.Length))}\n");
                }
                catch { }
                
                var result = new System.Net.Http.HttpResponseMessage(resp.StatusCode)
                {
                    Content = new ByteArrayContent(responseContent)
                };
                foreach (var header in resp.Headers)
                    result.Headers.TryAddWithoutValidation(header.Key, header.Value);
                foreach (var header in resp.Content.Headers)
                    result.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);

                return ResponseMessage(result);
            }
        }

        private static string GetLogFilePath(string fileName)
        {
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
                return Path.Combine(logDir, fileName);
            }
            catch
            {
                return Path.Combine("C:\\temp", fileName);
            }
        }

        private static bool WriteDebugLog(string fileName, string message, out string error)
        {
            error = null;
            try
            {
                var logPath = GetLogFilePath(fileName);
                File.AppendAllText(logPath, message);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    var fallbackPath = Path.Combine("C:\\temp", fileName);
                    File.AppendAllText(fallbackPath, message);
                    error = $"Primary log write failed, fallback wrote to {fallbackPath}: {ex.GetType().Name}: {ex.Message}";
                    return false;
                }
                catch (Exception fallbackEx)
                {
                    error = $"Primary log write failed: {ex.GetType().Name}: {ex.Message}; fallback failed: {fallbackEx.GetType().Name}: {fallbackEx.Message}";
                    return false;
                }
            }
        }

        private static void SafeLog(string fileName, string message)
        {
            // Try multiple log locations in order
            string[] logPaths = new string[]
            {
                GetLogFilePath(fileName),
                Path.Combine("C:\\temp", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? "", fileName),
                Path.Combine(Path.GetTempPath(), fileName)
            };

            foreach (var logPath in logPaths)
            {
                try
                {
                    if (!string.IsNullOrEmpty(logPath))
                    {
                        var dir = Path.GetDirectoryName(logPath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        
                        File.AppendAllText(logPath, message);
                        return; // Success
                    }
                }
                catch
                {
                    // Try next location
                }
            }
        }

        // Método auxiliar para validar JWT e extrair claims
        private static TimeSpan GetAccessTokenLifetime()
        {
            var accessTokenHours = 1;
            var raw = ConfigurationManager.AppSettings["JwtAccessTokenExpirationHours"];
            if (!string.IsNullOrWhiteSpace(raw) && int.TryParse(raw, out var parsed) && parsed > 0)
                accessTokenHours = parsed;
            return TimeSpan.FromHours(accessTokenHours);
        }

        private static TimeSpan GetRefreshTokenLifetime()
        {
            var refreshTokenDays = 7;
            var raw = ConfigurationManager.AppSettings["JwtRefreshTokenExpirationDays"];
            if (!string.IsNullOrWhiteSpace(raw) && int.TryParse(raw, out var parsed) && parsed > 0)
                refreshTokenDays = parsed;
            return TimeSpan.FromDays(refreshTokenDays);
        }

        private static string CreateJwtToken(IEnumerable<Claim> claims, string audience, TimeSpan validFor)
        {
            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"] ?? "defaultSecretKey";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: "MeuProxySsl",
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(validFor),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static bool IsRefreshToken(ClaimsPrincipal principal)
        {
            return principal?.FindFirst("tokenType")?.Value == "refresh";
        }

        private ClaimsPrincipal ValidateJwtToken(string token, string validAudience = "TokuPlusApp")
        {
            if (string.IsNullOrEmpty(token)) return null;
            
            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"] ?? "defaultSecretKey";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "MeuProxySsl",
                    ValidAudience = validAudience,
                    IssuerSigningKey = key
                };
                
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                try
                {
                    var logPath = GetLogFilePath("jwt_validation.log");
                    var safeToken = token.Length > 50 ? token.Substring(0, 50) + "..." : token;
                    File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ValidateJwtToken failed for audience={validAudience} tokenStart={safeToken} exception={ex.GetType().Name}: {ex.Message}\n");
                }
                catch { }
                return null; // Token inválido
            }
        }

        private readonly MySqlDatabase _db = new MySqlDatabase();

        [HttpGet]
        [Route("~/test")]
        public IHttpActionResult DatabaseTest()
        {
            if (!_db.IsConfigured)
            {
                return Content(HttpStatusCode.ServiceUnavailable, new
                {
                    Message = "MySQL não configurado",
                    ConnectionStringConfigured = false,
                    Hint = "Configure 'MySqlConnection' ou 'MySql:ConnectionString' no Web.config"
                });
            }

            try
            {
                var version = _db.ExecuteScalar("SELECT VERSION()")?.ToString();
                var databaseName = _db.ExecuteScalar("SELECT DATABASE()")?.ToString();

                return Ok(new
                {
                    Message = "MySQL conectado com sucesso",
                    ConnectionStringConfigured = true,
                    Database = databaseName,
                    Version = version,
                    Timestamp = DateTime.Now.ToString("o")
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Falha ao conectar no MySQL",
                    ConnectionStringConfigured = true,
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message,
                    Timestamp = DateTime.Now.ToString("o")
                });
            }
        }

        [HttpGet]
        [Route("~/table")]
        public IHttpActionResult GetTableData([FromUri] string tableName, [FromUri] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return BadRequest("O parâmetro 'tableName' é obrigatório.");
            }

            var sanitizedTable = tableName.Trim();
            if (!Regex.IsMatch(sanitizedTable, "^[A-Za-z0-9_]+$"))
            {
                return Content(HttpStatusCode.BadRequest, new
                {
                    Message = "Nome da tabela inválido.",
                    tableName = sanitizedTable,
                    AllowedPattern = "^[A-Za-z0-9_]+$"
                });
            }

            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = $"SELECT * FROM `{sanitizedTable}` LIMIT @limit";
                var rows = _db.Query(sql, new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                return Ok(new
                {
                    tableName = sanitizedTable,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela do MySQL.",
                    tableName = sanitizedTable,
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userdevice")]
        public IHttpActionResult GetUserDeviceData([FromUri] int? id = null, [FromUri] int? userId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userdevice`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `UserId` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userdevice",
                    id = id,
                    userId = userId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userdevice.",
                    tableName = "userdevice",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userinfo")]
        public IHttpActionResult GetUserInfoData([FromUri] int? id = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userinfo`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userinfo",
                    id = id,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userinfo.",
                    tableName = "userinfo",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/useraccess")]
        public IHttpActionResult GetUserAccessData([FromUri] long? id = null, [FromUri] int? userId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `useraccess`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `UserId` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "useraccess",
                    id = id,
                    userId = userId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela useraccess.",
                    tableName = "useraccess",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/position")]
        public IHttpActionResult GetPositionData([FromUri] long? id = null, [FromUri] int? createdBy = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `position`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (createdBy.HasValue)
                {
                    sql.Append(" WHERE `CreatedBy` = @createdBy");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@createdBy", createdBy.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "position",
                    id = id,
                    createdBy = createdBy,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela position.",
                    tableName = "position",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/plataformtype")]
        public IHttpActionResult GetPlataformTypeData([FromUri] string id = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `plataformtype`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(id))
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id));
                }

                sql.Append(" ORDER BY `Order` ASC, `Label` ASC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "plataformtype",
                    id = id,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela plataformtype.",
                    tableName = "plataformtype",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userperfil")]
        public IHttpActionResult GetUserPerfilData([FromUri] long? id = null, [FromUri] int? userId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userperfil`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `UserId` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userperfil",
                    id = id,
                    userId = userId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userperfil.",
                    tableName = "userperfil",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/useravatar")]
        public IHttpActionResult GetUserAvatarData([FromUri] long? id = null, [FromUri] int? createdBy = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `useravatar_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (createdBy.HasValue)
                {
                    sql.Append(" WHERE `CreatedBy` = @createdBy");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@createdBy", createdBy.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "useravatar_backup",
                    id = id,
                    createdBy = createdBy,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela useravatar_backup.",
                    tableName = "useravatar_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userdevicebackup")]
        public IHttpActionResult GetUserDeviceBackupData([FromUri] long? id = null, [FromUri] int? userId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userdevice_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `UserId` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userdevice_backup",
                    id = id,
                    userId = userId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userdevice_backup.",
                    tableName = "userdevice_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userinfobackup")]
        public IHttpActionResult GetUserInfoBackupData([FromUri] int? id = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userinfo_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userinfo_backup",
                    id = id,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userinfo_backup.",
                    tableName = "userinfo_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userinitialregistration")]
        public IHttpActionResult GetUserInitialRegistrationData([FromUri] long? id = null, [FromUri] int? userId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userinitialregistration_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `UserId` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userinitialregistration_backup",
                    id = id,
                    userId = userId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userinitialregistration_backup.",
                    tableName = "userinitialregistration_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userpasswordrecovery")]
        public IHttpActionResult GetUserPasswordRecoveryData([FromUri] long? id = null, [FromUri] int? userId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userpasswordrecovery_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `UserId` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userpasswordrecovery_backup",
                    id = id,
                    userId = userId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userpasswordrecovery_backup.",
                    tableName = "userpasswordrecovery_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userperfilbackup")]
        public IHttpActionResult GetUserPerfilBackupData([FromUri] long? id = null, [FromUri] int? userId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userperfil_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `UserId` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userperfil_backup",
                    id = id,
                    userId = userId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userperfil_backup.",
                    tableName = "userperfil_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userpicture")]
        [Route("~/userpicturebackup")]
        public IHttpActionResult GetUserPictureData([FromUri] long? id = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userpicture_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userpicture_backup",
                    id = id,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userpicture_backup.",
                    tableName = "userpicture_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userposition")]
        [Route("~/userpositionbackup")]
        public IHttpActionResult GetUserPositionData([FromUri] long? id = null, [FromUri] int? userId = null, [FromUri] long? positionId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userposition_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `UserId` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }
                else if (positionId.HasValue)
                {
                    sql.Append(" WHERE `PositionId` = @positionId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@positionId", positionId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userposition_backup",
                    id = id,
                    userId = userId,
                    positionId = positionId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userposition_backup.",
                    tableName = "userposition_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userstatus")]
        [Route("~/userstatusbackup")]
        public IHttpActionResult GetUserStatusData([FromUri] int? id = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `userstatus_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "userstatus_backup",
                    id = id,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela userstatus_backup.",
                    tableName = "userstatus_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/configurations")]
        [Route("~/configurationsbackup")]
        public IHttpActionResult GetConfigurationsData([FromUri] long? id = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `configurations_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "configurations_backup",
                    id = id,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela configurations_backup.",
                    tableName = "configurations_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/emailcontent")]
        [Route("~/emailcontentbackup")]
        public IHttpActionResult GetEmailContentData([FromUri] long? id = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `emailcontent_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "emailcontent_backup",
                    id = id,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela emailcontent_backup.",
                    tableName = "emailcontent_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/user")]
        public IHttpActionResult GetUserData([FromUri] int? id = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `user_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "user_backup",
                    id = id,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela user_backup.",
                    tableName = "user_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/userrole")]
        public IHttpActionResult GetUserRoleData([FromUri] int? id = null, [FromUri] int? userId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `user_role_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `User_Id` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "user_role_backup",
                    id = id,
                    userId = userId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela user_role_backup.",
                    tableName = "user_role_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/useraccessbackup")]
        public IHttpActionResult GetUserAccessBackupData([FromUri] long? id = null, [FromUri] int? userId = null, [FromUri] int limit = 20)
        {
            if (limit <= 0 || limit > 500)
                limit = 50;

            try
            {
                var sql = new StringBuilder("SELECT * FROM `useraccess_backup`");
                var parameters = new List<MySql.Data.MySqlClient.MySqlParameter>();

                if (id.HasValue)
                {
                    sql.Append(" WHERE `Id` = @id");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@id", id.Value));
                }
                else if (userId.HasValue)
                {
                    sql.Append(" WHERE `UserId` = @userId");
                    parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@userId", userId.Value));
                }

                sql.Append(" ORDER BY `Id` DESC LIMIT @limit");
                parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@limit", limit));

                var rows = _db.Query(sql.ToString(), parameters.ToArray());

                return Ok(new
                {
                    tableName = "useraccess_backup",
                    id = id,
                    userId = userId,
                    limit = limit,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Erro ao consultar a tabela useraccess_backup.",
                    tableName = "useraccess_backup",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("~/ping")]
        public IHttpActionResult Ping()
        {
            // Global endpoint call log
            try
            {
                WriteDebugLog("global_endpoint_calls.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Ping called\n", out var _);
            }
            catch { }
            
            string resolvedLogPath = null;
            string logWriteError = null;
            bool logWriteSuccess = false;

            try
            {
                // Log entry point
                resolvedLogPath = GetLogFilePath("ping_debug.log");
                WriteDebugLog("ping_debug.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Ping started\n", out var _);

                var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
                WriteDebugLog("ping_debug.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] JwtSecretKey read: {!string.IsNullOrEmpty(secretKey)}\n", out var _);

                var baseUrl = ConfigurationManager.AppSettings["Upstream:BaseUrl"];
                WriteDebugLog("ping_debug.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Upstream:BaseUrl read: {!string.IsNullOrEmpty(baseUrl)}\n", out var _);

                var cfgVersion = typeof(ProxyController).Assembly.GetName().Version?.ToString() ?? "unknown";
                WriteDebugLog("ping_debug.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Assembly version: {cfgVersion}\n", out var _);

                var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Ping executed. JwtSecretKeyConfigured={!string.IsNullOrEmpty(secretKey)}, UpstreamBaseUrlConfigured={!string.IsNullOrEmpty(baseUrl)}, AssemblyVersion={cfgVersion}, ResolvedLogPath={resolvedLogPath}\n";
                logWriteSuccess = WriteDebugLog("ping_debug.log", logLine, out logWriteError);
                return Ok(new
                {
                    Message = "pong",
                    JwtSecretKeyConfigured = !string.IsNullOrEmpty(secretKey),
                    UpstreamBaseUrlConfigured = !string.IsNullOrEmpty(baseUrl),
                    AssemblyVersion = cfgVersion,
                    ResolvedLogPath = resolvedLogPath,
                    LogWriteSuccess = logWriteSuccess,
                    LogWriteError = logWriteError,
                    BaseDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    CurrentDirectory = Environment.CurrentDirectory,
                    HostingEnvironmentIsHosted = HostingEnvironment.IsHosted,
                    Timestamp = DateTime.Now.ToString("o")
                });
            }
            catch (Exception ex)
            {
                try
                {
                    if (resolvedLogPath == null)
                        resolvedLogPath = GetLogFilePath("ping_debug.log");
                    WriteDebugLog("ping_debug.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Ping exception: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n", out var exceptionLogError);
                    logWriteError = logWriteError ?? exceptionLogError;
                }
                catch { }

                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Ping failed",
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message,
                    LogWriteSuccess = logWriteSuccess,
                    LogWriteError = logWriteError,
                    ResolvedLogPath = resolvedLogPath,
                    BaseDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    CurrentDirectory = Environment.CurrentDirectory,
                    HostingEnvironmentIsHosted = HostingEnvironment.IsHosted
                });
            }
        }

        [HttpGet]
        [Route("logpath")]
        public IHttpActionResult LogPath()
        {
            string resolvedLogPath = GetLogFilePath("ping_debug.log");
            return Ok(new
            {
                Message = "logpath",
                LogDirectory = Path.GetDirectoryName(resolvedLogPath),
                ResolvedLogPath = resolvedLogPath,
                BaseDirectory = AppDomain.CurrentDomain.BaseDirectory,
                CurrentDirectory = Environment.CurrentDirectory,
                HostingEnvironmentIsHosted = HostingEnvironment.IsHosted
            });
        }

        [HttpGet]
        [Route("logtest")]
        public IHttpActionResult LogTest()
        {
            string resolvedLogPath = GetLogFilePath("logtest_debug.log");
            var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] LogTest executed. ResolvedLogPath={resolvedLogPath}\n";
            var success = WriteDebugLog("logtest_debug.log", logLine, out var error);
            return Ok(new
            {
                Message = "logtest",
                LogDirectory = Path.GetDirectoryName(resolvedLogPath),
                ResolvedLogPath = resolvedLogPath,
                LogWriteSuccess = success,
                LogWriteError = error,
                BaseDirectory = AppDomain.CurrentDomain.BaseDirectory,
                CurrentDirectory = Environment.CurrentDirectory,
                HostingEnvironmentIsHosted = HostingEnvironment.IsHosted
            });
        }

        [HttpGet]
        [Route("getBuildInfo")]
        public IHttpActionResult GetBuildInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "unknown";
            var buildDateUtc = File.Exists(assembly.Location)
                ? File.GetLastWriteTimeUtc(assembly.Location).ToString("o")
                : "unknown";

            return Ok(new
            {
                Assembly = assembly.GetName().Name,
                Version = version,
                BuildDateUtc = buildDateUtc
            });
        }

        [HttpGet]
        [Route("createOrDeleteFavorites")]
        public async Task<IHttpActionResult> CreateOrDeleteFavorites(long userId, long serieId)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            return await ForwardRequest($"createOrDeleteFavorites?UserId={userId}&SerieId={serieId}");
        }

        [HttpGet]
        [Route("createOrUpdateUserTimeProgress")]
        public async Task<IHttpActionResult> CreateOrUpdateUserTimeProgress(long userId, int serieId, int episodioId, string time, string typeScreen, string status, string totalTime)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            var qs = $"UserId={userId}&SerieId={serieId}&EpisodioId={episodioId}&Time={Uri.EscapeDataString(time)}&TypeScreen={Uri.EscapeDataString(typeScreen)}&Status={Uri.EscapeDataString(status)}&TotalTime={Uri.EscapeDataString(totalTime)}";
            return await ForwardRequest("createOrUpdateUserTimeProgress", qs);
        }

        [HttpGet]
        [Route("createOrUpdateTokenTV")]
        public async Task<IHttpActionResult> CreateOrUpdateTokenTV(long userId, string token)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            if (string.IsNullOrEmpty(token)) return BadRequest("Token is required");
            var qs = $"UserId={userId}&Token={Uri.EscapeDataString(token)}";
            return await ForwardRequest("createOrUpdateTokenTV", qs);
        }

        [HttpGet]
        [Route("getContinuousEpisodesUser")]
        public async Task<IHttpActionResult> GetContinuousEpisodesUser(long userId, long serieId)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            var qs = $"UserId={userId}&SerieId={serieId}";
            return await ForwardRequest("getContinuousEpisodesUser", qs);
        }

        [HttpGet]
        [Route("getContinuousEpisodes")]
        public async Task<IHttpActionResult> GetContinuousEpisodes(long userId, int maxRecord, int filter)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            var qs = $"UserId={userId}&MaxRecord={maxRecord}&Filter={filter}";
            return await ForwardRequest("getContinuousEpisodes", qs);
        }

        [HttpGet]
        [Route("getEpisodes")]
        public async Task<IHttpActionResult> GetEpisodes(long id, long serieId, string status, long userId, bool? isMovie = null)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            // Log the received parameters for debugging
            try
            {
                var logPath = GetLogFilePath("proxy_request.log");
                File.AppendAllText(logPath, $"\n[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] GetEpisodes called: Id={id}, SerieId={serieId}, Status={status}, UserId={userId}, IsMovie={(isMovie.HasValue ? isMovie.Value.ToString().ToLowerInvariant() : "null")}\n");
            }
            catch { }

            var qs = $"Id={id}&SerieId={serieId}&Status={Uri.EscapeDataString(status)}&UserId={userId}";
            if (isMovie.HasValue)
                qs += $"&IsMovie={isMovie.Value.ToString().ToLowerInvariant()}";
            return await ForwardRequest("getEpisodes", qs);
        }

        [HttpGet]
        [Route("getFavorites")]
        public async Task<IHttpActionResult> GetFavorites(long userId)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            var qs = $"UserId={userId}";
            return await ForwardRequest("getFavorites", qs);
        }
  
        [HttpGet]
        [Route("getReleases")]
        public async Task<IHttpActionResult> GetReleases(long userId, int filter)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            var qs = $"UserId={userId}&Filter={filter}";
            return await ForwardRequest("getReleases", qs);
        }

        [HttpGet]
        [Route("getInfoInicialTV")]
        public async Task<IHttpActionResult> GetInfoInicialTV(long userId, long seriesId, long episodeId)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            var qs = $"UserId={userId}&SeriesId={seriesId}&EpisodeId={episodeId}";
            return await ForwardRequest("getInfoInicialTV", qs);
        }

        [HttpGet]
        [Route("getSerieCatalogo")]
        public async Task<IHttpActionResult> GetSerieCatalogo(int maxRegisters, long userId, long genreId, int filter)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            var qs = $"MaxRegisters={maxRegisters}&UserId={userId}&GenreId={genreId}&Filter={filter}";
            return await ForwardRequest("getSerieCatalogo", qs);
        }

        [HttpGet]
        [Route("getSeriesHigh")]
        public async Task<IHttpActionResult> GetSeriesHigh(int number, long userId)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            var qs = $"Number={number}&UserId={userId}";
            return await ForwardRequest("getSeriesHigh", qs);
        }

        [HttpGet]
        [Route("getTvChannels")]
        public async Task<IHttpActionResult> GetTvChannels()
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            return await ForwardRequest("getTvChannels");
        }

        [HttpGet]
        [Route("GetChannelsYoutube")]
        public async Task<IHttpActionResult> GetChannelsYoutube()
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            return await ForwardRequest("GetChannelsYoutube");
        }

        [HttpGet]
        [Route("GetChannelsYoutubeVideosList")]
        public async Task<IHttpActionResult> GetChannelsYoutubeVideosList(string youtubeChannelId, string pageToken = "")
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            if (string.IsNullOrEmpty(youtubeChannelId)) return BadRequest("YoutubeChannelId is required");
            
            var qs = $"YoutubeChannelId={Uri.EscapeDataString(youtubeChannelId)}";
            if (!string.IsNullOrEmpty(pageToken))
                qs += $"&pageToken={Uri.EscapeDataString(pageToken)}";
            
            return await ForwardRequest("GetChannelsYoutubeVideosList", qs);
        }

        [HttpGet]
        [Route("getChannelsYoutubeVideoDetail")]
        public async Task<IHttpActionResult> GetChannelsYoutubeVideoDetail(string videoYoutubeId)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            if (string.IsNullOrEmpty(videoYoutubeId)) return BadRequest("VideoYoutubeId is required");
            
            var qs = $"VideoYoutubeId={Uri.EscapeDataString(videoYoutubeId)}";
            
            return await ForwardRequest("getChannelsYoutubeVideoDetail", qs);
        }

        [HttpGet]
        [Route("SearchCatalogo")]
        public async Task<IHttpActionResult> SearchCatalogo(long userId, string text)
        {
            var authHeader = Request?.Headers?.Authorization;
            if (authHeader == null || !string.Equals(authHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            var principal = ValidateJwtToken(authHeader.Parameter);
            if (principal == null)
                return Unauthorized();

            if (string.IsNullOrEmpty(text)) return BadRequest("Text is required");
            var qs = $"UserId={userId}&Text={Uri.EscapeDataString(text)}";
            return await ForwardRequest("SearchCatalogo", qs);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("validateTVBoxToken")]
        public async Task<IHttpActionResult> validateTVBoxTokenTemp(string token)
        {
            // Global endpoint call log
            try
            {
                WriteDebugLog("global_endpoint_calls.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] validateTVBoxTokenTemp called with token='{token ?? "null"}'\n", out var _);
            }
            catch { }
            
            // Log entry point immediately - before anything else
            try
            {
                var immediateLogPath = GetLogFilePath("validateTVBoxTokenTemp_debug.log");
                File.AppendAllText(immediateLogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Method validateTVBoxTokenTemp ENTERED. Token parameter: '{token ?? "null"}'\n");
            }
            catch (Exception immediateEx)
            {
                try
                {
                    var fallbackImmediate = Path.Combine("C:\\temp", "validateTVBoxTokenTemp_debug.log");
                    File.AppendAllText(fallbackImmediate, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Method validateTVBoxTokenTemp ENTERED but immediate log failed: {immediateEx.Message}. Token parameter: '{token ?? "null"}'\n");
                }
                catch { }
            }
            
            var debugLogPath = GetLogFilePath("validateTVBoxTokenTemp_debug.log");
            
            // Log entry point immediately
            try
            {
                File.AppendAllText(debugLogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Method entered. Token present: {!string.IsNullOrEmpty(token)}\n");
            }
            catch (Exception entryEx)
            {
                // If even this fails, try fallback
                try
                {
                    var fallbackPath = Path.Combine("C:\\temp", "validateTVBoxTokenTemp_debug.log");
                    File.AppendAllText(fallbackPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Method entered but primary log failed: {entryEx.Message}. Token present: {!string.IsNullOrEmpty(token)}\n");
                }
                catch { }
            }
            
            try
            {
                if (string.IsNullOrEmpty(token)) return BadRequest("Token is required");
                var qs = $"Token={Uri.EscapeDataString(token)}";

                var baseTrim = _baseUrl?.TrimEnd('/') ?? string.Empty;
                try
                {
                    File.AppendAllText(debugLogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] _baseUrl check: '{_baseUrl}', baseTrim: '{baseTrim}'\n");
                }
                catch { }
                
                if (string.IsNullOrEmpty(baseTrim))
                    return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Upstream:BaseUrl is not configured", Encoding.UTF8, "application/json")
                    });

                var targetUri = baseTrim + "/validateTVBoxToken";
                if (!string.IsNullOrEmpty(qs)) targetUri += "?" + qs;

                try
                {
                    File.AppendAllText(debugLogPath,
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Request targetUri={targetUri} token={token}\n");
                }
                catch { }

                using (var resp = await _httpClient.GetAsync(targetUri))
                {
                    var responseBytes = await resp.Content.ReadAsByteArrayAsync();
                    var responseText = Encoding.UTF8.GetString(responseBytes);

                    try
                    {
                        File.AppendAllText(debugLogPath,
                            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Response status={(int)resp.StatusCode} responseText={responseText}\n");
                    }
                    catch { }

                    if (!resp.IsSuccessStatusCode)
                    {
                        var failContent = new ByteArrayContent(responseBytes);
                        foreach (var header in resp.Content.Headers)
                            failContent.Headers.TryAddWithoutValidation(header.Key, header.Value);

                        return ResponseMessage(new System.Net.Http.HttpResponseMessage(resp.StatusCode)
                        {
                            Content = failContent
                        });
                    }

                    JObject responseJson;
                    try
                    {
                        responseJson = JObject.Parse(responseText);
                    }
                    catch (JsonException jsonEx)
                    {
                        try
                        {
                            File.AppendAllText(debugLogPath,
                                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] JSON parse error: {jsonEx.Message}\n");
                        }
                        catch { }

                        var parseFailContent = new ByteArrayContent(responseBytes);
                        foreach (var header in resp.Content.Headers)
                            parseFailContent.Headers.TryAddWithoutValidation(header.Key, header.Value);

                        return ResponseMessage(new System.Net.Http.HttpResponseMessage(resp.StatusCode)
                        {
                            Content = parseFailContent
                        });
                    }

                    long userId = responseJson["UserId"]?.ToObject<long>() ?? 0;
                    string userName = responseJson["UserName"]?.ToString() ?? string.Empty;
                    string validate = responseJson["Validate"]?.ToString() ?? string.Empty;

                    if (userId != 0)
                    {
                        try
                        {
                            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"] ?? "defaultSecretKey";
                            try
                            {
                                File.AppendAllText(debugLogPath,
                                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] JwtSecretKey length={secretKey?.Length ?? 0}\n");
                            }
                            catch { }
                            var claims = new[]
                            {
                                new Claim("userId", userId.ToString()),
                                new Claim("userName", userName),
                                new Claim("validate", validate),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                            };
                            var accessToken = CreateJwtToken(claims, "TokuPlusApp", GetAccessTokenLifetime());
                            var refreshToken = CreateJwtToken(claims.Concat(new[] { new Claim("tokenType", "refresh") }), "TokuPlusRefresh", GetRefreshTokenLifetime());
                            responseJson["AccessToken"] = accessToken;
                            responseJson["RefreshToken"] = refreshToken;
                        }
                        catch (Exception jwtEx)
                        {
                            try
                            {
                                File.AppendAllText(debugLogPath,
                                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] JWT error: {jwtEx.GetType().Name}: {jwtEx.Message}\n{jwtEx.StackTrace}\n");
                            }
                            catch { }
                            responseJson["AccessToken"] = $"fallback_token_{userId}_{Guid.NewGuid()}";
                        }
                    }

                    return Ok(responseJson);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Main exception in validateTVBoxTokenTemp: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n";
                
                // Try to log to debug file
                try
                {
                    File.AppendAllText(debugLogPath, errorMessage);
                }
                catch { }
                
                // Ensure error is logged using robust method
                SafeLog("validateTVBoxTokenTemp_error.log", errorMessage);
                
                // Also log via global exception reporting
                SafeLog("global_exceptions.log", errorMessage);

                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal error: {ex.Message}", Encoding.UTF8, "application/json")
                });
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("refreshToken")]
        public IHttpActionResult RefreshToken(string refreshToken)
        {
            // Refresh token deve ser passado via query string ?refreshToken=<refresh_token>.
            // Este endpoint não exige token JWT no cabeçalho Authorization.
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var token = refreshToken;

            var principal = ValidateJwtToken(token, "TokuPlusRefresh");
            if (principal == null || !IsRefreshToken(principal))
            {
                return Unauthorized();
            }

            var userId = principal.FindFirst("userId")?.Value;
            var userName = principal.FindFirst("userName")?.Value ?? string.Empty;
            var validate = principal.FindFirst("validate")?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var claims = new[]
            {
                new Claim("userId", userId),
                new Claim("userName", userName),
                new Claim("validate", validate),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var accessToken = CreateJwtToken(claims, "TokuPlusApp", GetAccessTokenLifetime());
            var newRefreshToken = CreateJwtToken(claims.Concat(new[] { new Claim("tokenType", "refresh") }), "TokuPlusRefresh", GetRefreshTokenLifetime());

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresInSeconds = (int)GetAccessTokenLifetime().TotalSeconds
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ensureValidToken")]
        public IHttpActionResult EnsureValidToken([FromBody] Newtonsoft.Json.Linq.JObject body)
        {
            try
            {
                var accessToken = body?[("accessToken")]?.ToString();
                var refreshToken = body?[("refreshToken")]?.ToString();

                // Se o access token for válido, devolve-o imediatamente
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var principal = ValidateJwtToken(accessToken, "TokuPlusApp");
                    if (principal != null)
                    {
                        try
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var jwt = handler.ReadJwtToken(accessToken);
                            var seconds = (int)Math.Max(0, (jwt.ValidTo - DateTime.UtcNow).TotalSeconds);
                            return Ok(new { AccessToken = accessToken, Refreshed = false, ExpiresInSeconds = seconds });
                        }
                        catch
                        {
                            return Ok(new { AccessToken = accessToken, Refreshed = false, ExpiresInSeconds = (int)GetAccessTokenLifetime().TotalSeconds });
                        }
                    }
                }

                // Se o access token inválido/ausente, tenta usar o refresh token
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var principal = ValidateJwtToken(refreshToken, "TokuPlusRefresh");
                    if (principal != null && IsRefreshToken(principal))
                    {
                        var userId = principal.FindFirst("userId")?.Value;
                        var userName = principal.FindFirst("userName")?.Value ?? string.Empty;
                        var validate = principal.FindFirst("validate")?.Value ?? string.Empty;

                        if (!string.IsNullOrEmpty(userId))
                        {
                            var claims = new[]
                            {
                                new Claim("userId", userId),
                                new Claim("userName", userName),
                                new Claim("validate", validate),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                            };

                            var newAccess = CreateJwtToken(claims, "TokuPlusApp", GetAccessTokenLifetime());
                            var newRefresh = CreateJwtToken(claims.Concat(new[] { new Claim("tokenType", "refresh") }), "TokuPlusRefresh", GetRefreshTokenLifetime());

                            return Ok(new
                            {
                                AccessToken = newAccess,
                                RefreshToken = newRefresh,
                                ExpiresInSeconds = (int)GetAccessTokenLifetime().TotalSeconds,
                                Refreshed = true
                            });
                        }
                    }
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                SafeLog("ensureValidToken_error.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ensureValidToken error: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n");
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Internal error: {ex.Message}", Encoding.UTF8, "application/json")
                });
            }
        }

        [HttpPost]
        [Route("putCreateUserAccess")]
        public async Task<IHttpActionResult> PutCreateUserAccess(long userId, string plataform, string ip)
        {
            if (string.IsNullOrEmpty(plataform)) return BadRequest("Plataform is required");
            if (string.IsNullOrEmpty(ip)) return BadRequest("IP is required");
            
            var baseTrim = _baseUrl?.TrimEnd('/') ?? string.Empty;
            if (string.IsNullOrEmpty(baseTrim))
                return ResponseMessage(new System.Net.Http.HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Upstream:BaseUrl is not configured", Encoding.UTF8, "application/json")
                });
            // Upstream expects parameters in the URL; forward as query string.
            var qs = $"UserId={userId}&Plataform={Uri.EscapeDataString(plataform)}&IP={Uri.EscapeDataString(ip)}";
            var targetUri = baseTrim + "/putCreateUserAccess" + "?" + qs;

            var request = new HttpRequestMessage(HttpMethod.Put, targetUri);

            using (var resp = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var responseContent = await resp.Content.ReadAsByteArrayAsync();
                var result = new System.Net.Http.HttpResponseMessage(resp.StatusCode)
                {
                    Content = new ByteArrayContent(responseContent)
                };
                foreach (var header in resp.Headers)
                    result.Headers.TryAddWithoutValidation(header.Key, header.Value);
                foreach (var header in resp.Content.Headers)
                    result.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);

                return ResponseMessage(result);
            }
        }

        [HttpGet]
        [Route("getCreateUserAccess")]
        public async Task<IHttpActionResult> GetCreateUserAccess(long userId, string plataform, string ip)
        {
            if (string.IsNullOrEmpty(plataform)) return BadRequest("Plataform is required");
            if (string.IsNullOrEmpty(ip)) return BadRequest("IP is required");

            var qs = $"UserId={userId}&Plataform={Uri.EscapeDataString(plataform)}&IP={Uri.EscapeDataString(ip)}";
            return await ForwardRequest("getCreateUserAccess", qs);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getAppConfig")]
        public async Task<IHttpActionResult> GetAppConfig()
        {
            return await ForwardRequest("getAppConfig");
        }

        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [HttpPatch]
        [HttpOptions]
        [Route("~/api/{*path}")]
        public async Task<IHttpActionResult> ProxyAll(string path = "")
        {
            if (HttpContext.Current == null)
                return await ForwardRequest(path, string.Empty);

            var request = HttpContext.Current.Request;

            var targetUri = _baseUrl.TrimEnd('/') + "/" + path;
            if (!string.IsNullOrEmpty(request.QueryString.ToString()))
                targetUri += "?" + request.QueryString.ToString();

            var message = new HttpRequestMessage(new HttpMethod(request.HttpMethod), targetUri);

            foreach (var key in request.Headers.AllKeys)
            {
                var values = request.Headers.GetValues(key);
                if (!message.Headers.TryAddWithoutValidation(key, values))
                {
                    if (message.Content == null) message.Content = new StringContent(string.Empty);
                    message.Content.Headers.TryAddWithoutValidation(key, values);
                }
            }

            if (request.InputStream != null && request.ContentLength > 0)
            {
                request.InputStream.Position = 0;
                var content = new StreamContent(request.InputStream);
                if (!string.IsNullOrEmpty(request.ContentType))
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.ContentType);
                message.Content = content;
            }

            using (var resp = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead))
            {
                var responseContent = await resp.Content.ReadAsByteArrayAsync();
                var result = new System.Net.Http.HttpResponseMessage(resp.StatusCode)
                {
                    Content = new ByteArrayContent(responseContent)
                };
                foreach (var header in resp.Headers)
                    result.Headers.TryAddWithoutValidation(header.Key, header.Value);
                foreach (var header in resp.Content.Headers)
                    result.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);

                return ResponseMessage(result);
            }
        }
    }
}
