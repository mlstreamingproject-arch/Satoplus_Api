using System;
using System.Collections.Generic;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("positions")]
    public class PositionController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/positions
        [HttpGet]
        [Route("getallpositions")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var positions = _database.GetAllPositions();
                return Ok(positions);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar posições: {ex.Message}");
            }
        }

        // GET: api/positions/{id}
        [HttpGet]
        [Route("getpositionbyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var position = _database.GetPositionById(id);
                if (position == null)
                    return NotFound();
                return Ok(position);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar posição: {ex.Message}");
            }
        }

        // POST: api/positions
        [HttpPost]
        [Route("createposition")]
        public IHttpActionResult Create([FromBody] CreatePositionDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || string.IsNullOrEmpty(dto.Name))
                    return BadRequest("Id e Nome são obrigatórios");

                var position = new Position
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedOn = DateTime.Now,
                    CreatedBy = dto.CreatedBy,
                    IsActive = dto.IsActive ?? true
                };

                _database.CreatePosition(position);
                return Created($"api/positions/{position.Id}", position);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar posição: {ex.Message}");
            }
        }

        // PUT: api/positions/{id}
        [HttpPut]
        [Route("updateposition/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdatePositionDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var position = _database.GetPositionById(id);
                if (position == null)
                    return NotFound();

                position.Name = dto.Name ?? position.Name;
                position.Description = dto.Description ?? position.Description;
                position.UpdatedOn = DateTime.Now;
                position.UpdatedBy = dto.UpdatedBy ?? position.UpdatedBy;
                position.IsActive = dto.IsActive ?? position.IsActive;

                _database.UpdatePosition(position);
                return Ok(position);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar posição: {ex.Message}");
            }
        }

        // DELETE: api/positions/{id}
        [HttpDelete]
        [Route("deleteposition/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var position = _database.GetPositionById(id);
                if (position == null)
                    return NotFound();

                _database.DeletePosition(id);
                return Ok(new { message = "Posição deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar posição: {ex.Message}");
            }
        }
    }
}
