using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("userinitialregistration")]
    public class UserInitialRegistrationController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/userinitialregistration
        [HttpGet]
        [Route("getalluserinitialregistration")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var registrations = _database.GetAllUserInitialRegistrations();
                return Ok(registrations);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar registrations: {ex.Message}");
            }
        }

        // GET: api/userinitialregistration/{id}
        [HttpGet]
        [Route("getuserinitialregistrationbyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var registration = _database.GetUserInitialRegistrationById(id);
                if (registration == null)
                    return NotFound();
                return Ok(registration);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar registration: {ex.Message}");
            }
        }

        // POST: api/userinitialregistration
        [HttpPost]
        [Route("createuserinitialregistration")]
        public IHttpActionResult Create([FromBody] CreateUserInitialRegistrationDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || string.IsNullOrEmpty(dto.Email))
                    return BadRequest("Id e Email são obrigatórios");

                var registration = new UserInitialRegistration
                {
                    Id = dto.Id,
                    Status = dto.Status,
                    Email = dto.Email,
                    PlataformTypeId = dto.PlataformTypeId,
                    IP = dto.IP,
                    Token = dto.Token,
                    CreatedOn = dto.CreatedOn ?? DateTime.Now,
                    UpdateOn = dto.UpdateOn,
                    RegionName = dto.RegionName,
                    City = dto.City,
                    Country = dto.Country,
                    V_OS = dto.V_OS,
                    V_Browser = dto.V_Browser,
                    Deeplink = dto.Deeplink,
                    Password = dto.Password
                };

                _database.CreateUserInitialRegistration(registration);
                return Created($"api/userinitialregistration/{registration.Id}", registration);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar registration: {ex.Message}");
            }
        }

        // PUT: api/userinitialregistration/{id}
        [HttpPut]
        [Route("updateuserinitialregistration/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdateUserInitialRegistrationDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var registration = _database.GetUserInitialRegistrationById(id);
                if (registration == null)
                    return NotFound();

                registration.Status = dto.Status ?? registration.Status;
                registration.Email = dto.Email ?? registration.Email;
                registration.PlataformTypeId = dto.PlataformTypeId ?? registration.PlataformTypeId;
                registration.IP = dto.IP ?? registration.IP;
                registration.Token = dto.Token ?? registration.Token;
                registration.UpdateOn = DateTime.Now;
                registration.RegionName = dto.RegionName ?? registration.RegionName;
                registration.City = dto.City ?? registration.City;
                registration.Country = dto.Country ?? registration.Country;
                registration.V_OS = dto.V_OS ?? registration.V_OS;
                registration.V_Browser = dto.V_Browser ?? registration.V_Browser;
                registration.Deeplink = dto.Deeplink ?? registration.Deeplink;
                registration.Password = dto.Password ?? registration.Password;

                _database.UpdateUserInitialRegistration(registration);
                return Ok(registration);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar registration: {ex.Message}");
            }
        }

        // DELETE: api/userinitialregistration/{id}
        [HttpDelete]
        [Route("deleteuserinitialregistration/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var registration = _database.GetUserInitialRegistrationById(id);
                if (registration == null)
                    return NotFound();

                _database.DeleteUserInitialRegistration(id);
                return Ok(new { message = "Registration deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar registration: {ex.Message}");
            }
        }
    }
}
