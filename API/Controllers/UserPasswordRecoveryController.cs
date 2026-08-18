using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("userpasswordrecovery")]
    public class UserPasswordRecoveryController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/userpasswordrecovery
        [HttpGet]
        [Route("getalluserpasswordrecovery")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var recoveries = _database.GetAllUserPasswordRecoveries();
                return Ok(recoveries);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar recoveries: {ex.Message}");
            }
        }

        // GET: api/userpasswordrecovery/{id}
        [HttpGet]
        [Route("getuserpasswordrecoverybyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var recovery = _database.GetUserPasswordRecoveryById(id);
                if (recovery == null)
                    return NotFound();
                return Ok(recovery);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar recovery: {ex.Message}");
            }
        }

        // POST: api/userpasswordrecovery
        [HttpPost]
        [Route("createuserpasswordrecovery")]
        public IHttpActionResult Create([FromBody] CreateUserPasswordRecoveryDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || dto.UserId <= 0)
                    return BadRequest("Id e UserId são obrigatórios");

                var recovery = new UserPasswordRecovery
                {
                    Id = dto.Id,
                    UserId = dto.UserId,
                    CreatedOn = dto.CreatedOn ?? DateTime.Now,
                    IsValid = dto.IsValid ?? true
                };

                _database.CreateUserPasswordRecovery(recovery);
                return Created($"api/userpasswordrecovery/{recovery.Id}", recovery);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar recovery: {ex.Message}");
            }
        }

        // PUT: api/userpasswordrecovery/{id}
        [HttpPut]
        [Route("updateuserpasswordrecovery/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdateUserPasswordRecoveryDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var recovery = _database.GetUserPasswordRecoveryById(id);
                if (recovery == null)
                    return NotFound();

                recovery.UserId = dto.UserId ?? recovery.UserId;
                recovery.IsValid = dto.IsValid ?? recovery.IsValid;

                _database.UpdateUserPasswordRecovery(recovery);
                return Ok(recovery);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar recovery: {ex.Message}");
            }
        }

        // DELETE: api/userpasswordrecovery/{id}
        [HttpDelete]
        [Route("deleteuserpasswordrecovery/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var recovery = _database.GetUserPasswordRecoveryById(id);
                if (recovery == null)
                    return NotFound();

                _database.DeleteUserPasswordRecovery(id);
                return Ok(new { message = "Recovery deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar recovery: {ex.Message}");
            }
        }
    }
}
