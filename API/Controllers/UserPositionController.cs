using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("userposition")]
    public class UserPositionController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/userposition
        [HttpGet]
        [Route("getalluserposition")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var userPositions = _database.GetAllUserPositions();
                return Ok(userPositions);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar user positions: {ex.Message}");
            }
        }

        // GET: api/userposition/{id}
        [HttpGet]
        [Route("getuserpositionbyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var userPosition = _database.GetUserPositionById(id);
                if (userPosition == null)
                    return NotFound();
                return Ok(userPosition);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar user position: {ex.Message}");
            }
        }

        // POST: api/userposition
        [HttpPost]
        [Route("createuserposition")]
        public IHttpActionResult Create([FromBody] CreateUserPositionDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || dto.UserId <= 0 || dto.PositionId <= 0)
                    return BadRequest("Id, UserId e PositionId são obrigatórios");

                var userPosition = new UserPosition
                {
                    Id = dto.Id,
                    UserId = dto.UserId,
                    PositionId = dto.PositionId,
                    CreatedOn = dto.CreatedOn ?? DateTime.Now,
                    CreatedBy = dto.CreatedBy,
                    UpdatedOn = dto.UpdatedOn,
                    UpdatedBy = dto.UpdatedBy
                };

                _database.CreateUserPosition(userPosition);
                return Created($"api/userposition/{userPosition.Id}", userPosition);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar user position: {ex.Message}");
            }
        }

        // PUT: api/userposition/{id}
        [HttpPut]
        [Route("updateuserposition/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdateUserPositionDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var userPosition = _database.GetUserPositionById(id);
                if (userPosition == null)
                    return NotFound();

                userPosition.UserId = dto.UserId ?? userPosition.UserId;
                userPosition.PositionId = dto.PositionId ?? userPosition.PositionId;
                userPosition.UpdatedOn = dto.UpdatedOn ?? DateTime.Now;
                userPosition.UpdatedBy = dto.UpdatedBy ?? userPosition.UpdatedBy;

                _database.UpdateUserPosition(userPosition);
                return Ok(userPosition);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar user position: {ex.Message}");
            }
        }

        // DELETE: api/userposition/{id}
        [HttpDelete]
        [Route("deleteuserposition/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var userPosition = _database.GetUserPositionById(id);
                if (userPosition == null)
                    return NotFound();

                _database.DeleteUserPosition(id);
                return Ok(new { message = "User position deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar user position: {ex.Message}");
            }
        }
    }
}
