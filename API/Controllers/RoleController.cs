using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("roles")]
    public class RoleController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/roles
        [HttpGet]
        [Route("getallroles")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var roles = _database.GetAllRoles();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar roles: {ex.Message}");
            }
        }

        // GET: api/roles/{id}
        [HttpGet]
        [Route("getrolebyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var role = _database.GetRoleById(id);
                if (role == null)
                    return NotFound();
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar role: {ex.Message}");
            }
        }

        // POST: api/roles
        [HttpPost]
        [Route("createrole")]
        public IHttpActionResult Create([FromBody] CreateRoleDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || string.IsNullOrEmpty(dto.Name))
                    return BadRequest("Id e Nome são obrigatórios");

                var role = new Role
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Persistent = dto.Persistent ?? true,
                    SS_Key = dto.SS_Key,
                    Espace_Id = dto.Espace_Id,
                    IsActive = dto.IsActive ?? true,
                    Description = dto.Description
                };

                _database.CreateRole(role);
                return Created($"api/roles/{role.Id}", role);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar role: {ex.Message}");
            }
        }

        // PUT: api/roles/{id}
        [HttpPut]
        [Route("updaterole/{id}")]
        public IHttpActionResult Update(int id, [FromBody] UpdateRoleDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var role = _database.GetRoleById(id);
                if (role == null)
                    return NotFound();

                role.Name = dto.Name;
                role.Persistent = dto.Persistent;
                role.SS_Key = dto.SS_Key;
                role.Espace_Id = dto.Espace_Id;
                role.IsActive = dto.IsActive;
                role.Description = dto.Description;

                _database.UpdateRole(role);
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar role: {ex.Message}");
            }
        }

        // DELETE: api/roles/{id}
        [HttpDelete]
        [Route("deleterole/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var role = _database.GetRoleById(id);
                if (role == null)
                    return NotFound();

                _database.DeleteRole(id);
                return Ok(new { message = "Role deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar role: {ex.Message}");
            }
        }
    }
}
