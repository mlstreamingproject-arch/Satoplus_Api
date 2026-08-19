using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("userdevice")]
    public class UserDeviceController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/userdevice
        [HttpGet]
        [Route("getalluserdevice")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var devices = _database.GetAllUserDevices();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar devices: {ex.Message}");
            }
        }

        // GET: api/userdevice/{id}
        [HttpGet]
        [Route("getuserdevicebyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var device = _database.GetUserDeviceById(id);
                if (device == null)
                    return NotFound();
                return Ok(device);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar device: {ex.Message}");
            }
        }

        // POST: api/userdevice
        [HttpPost]
        [Route("createuserdevice")]
        public IHttpActionResult Create([FromBody] CreateUserDeviceDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                    return BadRequest("Id é obrigatório");

                var device = new UserDevice
                {
                    Id = dto.Id,
                    Version = dto.Version,
                    UUID = dto.UUID,
                    Serial = dto.Serial,
                    Platform = dto.Platform,
                    Model = dto.Model,
                    Manufacturer = dto.Manufacturer,
                    IsVirtual = dto.IsVirtual,
                    GetCordova = dto.GetCordova,
                    DeviceType = dto.DeviceType,
                    UserId = dto.UserId,
                    UserInitialRegistrationToken = dto.UserInitialRegistrationToken
                };

                _database.CreateUserDevice(device);
                return Created($"api/userdevice/{device.Id}", device);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar device: {ex.Message}");
            }
        }

        // PUT: api/userdevice/{id}
        [HttpPut]
        [Route("updateuserdevice/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdateUserDeviceDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var device = _database.GetUserDeviceById(id);
                if (device == null)
                    return NotFound();

                device.Version = dto.Version;
                device.UUID = dto.UUID;
                device.Serial = dto.Serial;
                device.Platform = dto.Platform;
                device.Model = dto.Model;
                device.Manufacturer = dto.Manufacturer;
                device.IsVirtual = dto.IsVirtual;
                device.GetCordova = dto.GetCordova;
                device.DeviceType = dto.DeviceType;
                device.UserId = dto.UserId;
                device.UserInitialRegistrationToken = dto.UserInitialRegistrationToken;

                _database.UpdateUserDevice(device);
                return Ok(device);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar device: {ex.Message}");
            }
        }

        // DELETE: api/userdevice/{id}
        [HttpDelete]
        [Route("deleteuserdevice/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var device = _database.GetUserDeviceById(id);
                if (device == null)
                    return NotFound();

                _database.DeleteUserDevice(id);
                return Ok(new { message = "Device deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar device: {ex.Message}");
            }
        }
    }
}
