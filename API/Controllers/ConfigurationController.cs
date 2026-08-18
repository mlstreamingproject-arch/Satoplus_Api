using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("configurations")]
    public class ConfigurationController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/configurations
        [HttpGet]
        [Route("getallconfigurations")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var configurations = _database.GetAllConfigurations();
                return Ok(configurations);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar configurações: {ex.Message}");
            }
        }

        // GET: api/configurations/{id}
        [HttpGet]
        [Route("getconfigurationsbyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var configuration = _database.GetConfigurationById(id);
                if (configuration == null)
                    return NotFound();
                return Ok(configuration);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar configuração: {ex.Message}");
            }
        }

        // POST: api/configurations
        [HttpPost]
        [Route("createconfiguration")]
        public IHttpActionResult Create([FromBody] CreateConfigurationDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || string.IsNullOrEmpty(dto.Name))
                    return BadRequest("Id e Nome são obrigatórios");

                var configuration = new Configuration
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    Value = dto.Value,
                    CreatedOn = dto.CreatedOn ?? DateTime.Now,
                    CreatedBy = dto.CreatedBy,
                    UpdateOn = dto.UpdateOn,
                    UpdateBy = dto.UpdateBy
                };

                _database.CreateConfiguration(configuration);
                return Created($"api/configurations/{configuration.Id}", configuration);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar configuração: {ex.Message}");
            }
        }

        // PUT: api/configurations/{id}
        [HttpPut]
        [Route("updateconfiguration/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdateConfigurationDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var configuration = _database.GetConfigurationById(id);
                if (configuration == null)
                    return NotFound();

                configuration.Name = dto.Name ?? configuration.Name;
                configuration.Description = dto.Description ?? configuration.Description;
                configuration.Value = dto.Value ?? configuration.Value;
                configuration.UpdateOn = DateTime.Now;
                configuration.UpdateBy = dto.UpdateBy ?? configuration.UpdateBy;

                _database.UpdateConfiguration(configuration);
                return Ok(configuration);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar configuração: {ex.Message}");
            }
        }

        // DELETE: api/configurations/{id}
        [HttpDelete]
        [Route("deleteconfiguration/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var configuration = _database.GetConfigurationById(id);
                if (configuration == null)
                    return NotFound();

                _database.DeleteConfiguration(id);
                return Ok(new { message = "Configuração deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar configuração: {ex.Message}");
            }
        }
    }
}
