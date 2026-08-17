using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("userroles")]
    public class UserRoleController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/userroles
        [HttpGet]
        [Route("getalluserroles")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var userRoles = _database.GetAllUserRoles();
                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar user roles: {ex.Message}");
            }
        }

        // GET: api/userroles/{id}
        [HttpGet]
        [Route("getuserrolebyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var userRole = _database.GetUserRoleById(id);
                if (userRole == null)
                    return NotFound();
                return Ok(userRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar user role: {ex.Message}");
            }
        }

        // POST: api/userroles
        [HttpPost]
        [Route("createuserrole")]
        public IHttpActionResult Create([FromBody] CreateUserRoleDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || dto.User_Id <= 0 || dto.Role_Id <= 0)
                    return BadRequest("Id, User_Id e Role_Id são obrigatórios");

                var userRole = new UserRole
                {
                    Id = dto.Id,
                    User_Id = dto.User_Id,
                    Role_Id = dto.Role_Id
                };

                _database.CreateUserRole(userRole);
                return Created($"api/userroles/{userRole.Id}", userRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar user role: {ex.Message}");
            }
        }

        // PUT: api/userroles/{id}
        [HttpPut]
        [Route("updateuserrole/{id}")]
        public IHttpActionResult Update(int id, [FromBody] UpdateUserRoleDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var userRole = _database.GetUserRoleById(id);
                if (userRole == null)
                    return NotFound();

                userRole.User_Id = dto.User_Id ?? userRole.User_Id;
                userRole.Role_Id = dto.Role_Id ?? userRole.Role_Id;

                _database.UpdateUserRole(userRole);
                return Ok(userRole);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar user role: {ex.Message}");
            }
        }

        // DELETE: api/userroles/{id}
        [HttpDelete]
        [Route("deleteuserrole/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var userRole = _database.GetUserRoleById(id);
                if (userRole == null)
                    return NotFound();

                _database.DeleteUserRole(id);
                return Ok(new { message = "User role deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar user role: {ex.Message}");
            }
        }
    }
}
