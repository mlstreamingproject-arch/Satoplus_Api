using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("useraccess")]
    public class UserAccessController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/useraccess
        [HttpGet]
        [Route("getalluseraccess")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var userAccess = _database.GetAllUserAccess();
                return Ok(userAccess);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar user access: {ex.Message}");
            }
        }

        // GET: api/useraccess/{id}
        [HttpGet]
        [Route("getuseraccessbyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var access = _database.GetUserAccessById(id);
                if (access == null)
                    return NotFound();
                return Ok(access);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar user access: {ex.Message}");
            }
        }

        // POST: api/useraccess
        [HttpPost]
        [Route("createuseraccess")]
        public IHttpActionResult Create([FromBody] CreateUserAccessDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || dto.UserId <= 0)
                    return BadRequest("Id e UserId são obrigatórios");

                var userAccess = new UserAccess
                {
                    Id = dto.Id,
                    UserId = dto.UserId,
                    UserPerfilId = dto.UserPerfilId,
                    PlataformTypeId = dto.PlataformTypeId,
                    IP = dto.IP,
                    CreatedOn = DateTime.Now
                };

                _database.CreateUserAccess(userAccess);
                return Created($"api/useraccess/{userAccess.Id}", userAccess);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar user access: {ex.Message}");
            }
        }

        // PUT: api/useraccess/{id}
        [HttpPut]
        [Route("updateuseraccess/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdateUserAccessDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var userAccess = _database.GetUserAccessById(id);
                if (userAccess == null)
                    return NotFound();

                userAccess.UserId = dto.UserId ?? userAccess.UserId;
                userAccess.UserPerfilId = dto.UserPerfilId ?? userAccess.UserPerfilId;
                userAccess.PlataformTypeId = dto.PlataformTypeId ?? userAccess.PlataformTypeId;
                userAccess.IP = dto.IP ?? userAccess.IP;

                _database.UpdateUserAccess(userAccess);
                return Ok(userAccess);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar user access: {ex.Message}");
            }
        }

        // DELETE: api/useraccess/{id}
        [HttpDelete]
        [Route("deleteuseraccess/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var userAccess = _database.GetUserAccessById(id);
                if (userAccess == null)
                    return NotFound();

                _database.DeleteUserAccess(id);
                return Ok(new { message = "User access deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar user access: {ex.Message}");
            }
        }
    }
}
