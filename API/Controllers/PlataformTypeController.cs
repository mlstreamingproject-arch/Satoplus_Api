using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("plataformtypes")]
    public class PlataformTypeController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/plataformtypes
        [HttpGet]
        [Route("getallplataformtypes")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var plataforms = _database.GetAllPlataformTypes();
                return Ok(plataforms);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar plataformas: {ex.Message}");
            }
        }

        // GET: api/plataformtypes/{id}
        [HttpGet]
        [Route("getplataformtypebyid/{id}")]
        public IHttpActionResult GetById(string id)
        {
            try
            {
                var plataform = _database.GetPlataformTypeById(id);
                if (plataform == null)
                    return NotFound();
                return Ok(plataform);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar plataforma: {ex.Message}");
            }
        }

        // POST: api/plataformtypes
        [HttpPost]
        [Route("createplataformtype")]
        public IHttpActionResult Create([FromBody] CreatePlataformTypeDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var plataform = new PlataformType
                {
                    Id = dto.Id,
                    Label = dto.Label,
                    Order = dto.Order,
                    IsActive = dto.IsActive ?? true
                };

                _database.CreatePlataformType(plataform);
                return Created($"api/plataformtypes/{plataform.Id}", plataform);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar plataforma: {ex.Message}");
            }
        }

        // PUT: api/plataformtypes/{id}
        [HttpPut]
        [Route("updateplataformtype/{id}")]
        public IHttpActionResult Update(string id, [FromBody] UpdatePlataformTypeDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var plataform = _database.GetPlataformTypeById(id);
                if (plataform == null)
                    return NotFound();

                plataform.Label = dto.Label ?? plataform.Label;
                plataform.Order = dto.Order ?? plataform.Order;
                plataform.IsActive = dto.IsActive ?? plataform.IsActive;

                _database.UpdatePlataformType(plataform);
                return Ok(plataform);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar plataforma: {ex.Message}");
            }
        }

        // DELETE: api/plataformtypes/{id}
        [HttpDelete]
        [Route("deleteplataformtype/{id}")]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                var plataform = _database.GetPlataformTypeById(id);
                if (plataform == null)
                    return NotFound();

                _database.DeletePlataformType(id);
                return Ok(new { message = "Plataforma deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar plataforma: {ex.Message}");
            }
        }
    }
}
