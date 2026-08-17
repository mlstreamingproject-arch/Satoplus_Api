using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace MeuProxySsl.Security
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        private const string Issuer = "MeuProxySsl";
        private const string DefaultAudience = "TokuPlusApp";

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                LogDebug("JwtAuthorizeAttribute.OnAuthorization called");
                
                if (actionContext == null)
                {
                    LogDebug("actionContext is null");
                    return;
                }

                var isAnonymous = actionContext.ActionDescriptor
                    .GetCustomAttributes<AllowAnonymousAttribute>()
                    .Any()
                    || actionContext.ControllerContext.ControllerDescriptor
                        .GetCustomAttributes<AllowAnonymousAttribute>()
                        .Any();

                if (isAnonymous)
                {
                    LogDebug("Endpoint is marked as anonymous");
                    return;
                }

                var auth = actionContext.Request?.Headers?.Authorization;
                if (auth == null || !string.Equals(auth.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                {
                    LogDebug("No Bearer token found");
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        "Authorization header Bearer token is required.");
                    return;
                }

                var token = auth.Parameter;
                if (string.IsNullOrWhiteSpace(token))
                {
                    LogDebug("Token parameter is empty");
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        "Token is required.");
                    return;
                }

                LogDebug("Validating token...");
                var principal = ValidateToken(token, DefaultAudience);
                if (principal == null)
                {
                    LogDebug("Token validation failed");
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        "Invalid or expired token.");
                    return;
                }

                LogDebug("Token validated successfully");
                actionContext.ControllerContext.RequestContext.Principal = principal;
                base.OnAuthorization(actionContext);
            }
            catch (Exception ex)
            {
                LogDebug("JwtAuthorizeAttribute exception: " + ex.ToString());
                throw;
            }
        }

        private static ClaimsPrincipal ValidateToken(string token, string validAudience)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

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
                    ValidIssuer = Issuer,
                    ValidAudience = validAudience,
                    IssuerSigningKey = key
                };

                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }

        private static void LogDebug(string message)
        {
            try
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
                        File.AppendAllText(Path.Combine(logDir, "jwt_debug.log"), $"[{DateTime.UtcNow:o}] {message}\n");
                        return;
                    }
                    catch { }
                }
            }
            catch { }
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
