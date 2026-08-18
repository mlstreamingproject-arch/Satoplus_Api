using System;
using System.IO;
using System.Web.Http;
using System.Web.Hosting;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("users")]
    [AllowAnonymous]
    public class UserController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/users/test - Endpoint de teste (sem banco de dados)
        [HttpGet]
        [Route("test")]
        public IHttpActionResult TestEndpoint()
        {
            return Ok(new { status = "OK", message = "API funcionando", timestamp = DateTime.UtcNow });
        }

        // GET: api/users
        [HttpGet]
        [Route("getallusers")]
        public IHttpActionResult GetAll()
        {
            LogDebug("UserController.GetAll() called");
            try
            {
                LogDebug("Creating MySqlDatabase instance...");
                LogDebug("Calling _database.GetAllUsers()...");
                var users = _database.GetAllUsers();
                LogDebug($"GetAllUsers returned {(users?.Count ?? 0)} items");
                return Ok(users);
            }
            catch (Exception ex)
            {
                LogDebug($"Exception in GetAll: {ex.GetType().Name}: {ex.Message}\nStack: {ex.StackTrace}");
                return BadRequest($"Erro ao buscar usuários: {ex.Message} | {ex.InnerException?.Message}");
            }
        }

        // GET: api/users/{id}
        [HttpGet]
        [Route("getuserbyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var user = _database.GetUserById(id);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar usuário: {ex.Message}");
            }
        }

        // POST: api/users
        [HttpPost]
        [Route("createuser")]
        public IHttpActionResult Create([FromBody] CreateUserDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                    return BadRequest("Id, Username e Password são obrigatórios");

                var user = new User
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Username = dto.Username,
                    Password = dto.Password,
                    Email = dto.Email,
                    MobilePhone = dto.MobilePhone,
                    External_Id = dto.External_Id,
                    Creation_Date = dto.Creation_Date ?? DateTime.Now,
                    Last_Login = dto.Last_Login,
                    IsActive = dto.IsActive ?? true
                };

                _database.CreateUser(user);
                return Created($"api/users/{user.Id}", user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar usuário: {ex.Message}");
            }
        }

        // PUT: api/users/{id}
        [HttpPut]
        [Route("updateuser/{id}")]
        public IHttpActionResult Update(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var user = _database.GetUserById(id);
                if (user == null)
                    return NotFound();

                user.Name = dto.Name ?? user.Name;
                user.Username = dto.Username ?? user.Username;
                user.Password = dto.Password ?? user.Password;
                user.Email = dto.Email ?? user.Email;
                user.MobilePhone = dto.MobilePhone ?? user.MobilePhone;
                user.External_Id = dto.External_Id ?? user.External_Id;
                user.IsActive = dto.IsActive ?? user.IsActive;

                _database.UpdateUser(user);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete]
        [Route("deleteuser/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var user = _database.GetUserById(id);
                if (user == null)
                    return NotFound();

                _database.DeleteUser(id);
                return Ok(new { message = "Usuário deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar usuário: {ex.Message}");
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
                        File.AppendAllText(Path.Combine(logDir, "controller_debug.log"), $"[{DateTime.UtcNow:o}] {message}\n");
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
