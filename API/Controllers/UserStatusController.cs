using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("userstatus")]
    public class UserStatusController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/userstatus
        [HttpGet]
        [Route("getalluserstatus")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var statuses = _database.GetAllUserStatuses();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar statuses: {ex.Message}");
            }
        }

        // GET: api/userstatus/{id}
        [HttpGet]
        [Route("getuserstatusbyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var status = _database.GetUserStatusById(id);
                if (status == null)
                    return NotFound();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar status: {ex.Message}");
            }
        }

        // POST: api/userstatus
        [HttpPost]
        [Route("createuserstatus")]
        public IHttpActionResult Create([FromBody] CreateUserStatusDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                    return BadRequest("Id é obrigatório");

                var status = new UserStatus
                {
                    Id = dto.Id,
                    IsOnLine = dto.IsOnLine ?? false,
                    UpdateOn = DateTime.Now
                };

                _database.CreateUserStatus(status);
                return Created($"api/userstatus/{status.Id}", status);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar status: {ex.Message}");
            }
        }

        // PUT: api/userstatus/{id}
        [HttpPut]
        [Route("updateuserstatus/{id}")]
        public IHttpActionResult Update(int id, [FromBody] UpdateUserStatusDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var status = _database.GetUserStatusById(id);
                if (status == null)
                    return NotFound();

                status.IsOnLine = dto.IsOnLine;
                status.UpdateOn = dto.UpdateOn;

                _database.UpdateUserStatus(status);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar status: {ex.Message}");
            }
        }

        // DELETE: api/userstatus/{id}
        [HttpDelete]
        [Route("deleteuserstatus/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var status = _database.GetUserStatusById(id);
                if (status == null)
                    return NotFound();

                _database.DeleteUserStatus(id);
                return Ok(new { message = "Status deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar status: {ex.Message}");
            }
        }
    }
}
