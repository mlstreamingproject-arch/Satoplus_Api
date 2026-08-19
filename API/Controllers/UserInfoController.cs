using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("userinfo")]
    public class UserInfoController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/userinfo
        [HttpGet]
        [Route("getalluserinfo")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var infos = _database.GetAllUserInfos();
                return Ok(infos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar infos: {ex.Message}");
            }
        }

        // GET: api/userinfo/{id}
        [HttpGet]
        [Route("getuserinfobyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var info = _database.GetUserInfoById(id);
                if (info == null)
                    return NotFound();
                return Ok(info);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar info: {ex.Message}");
            }
        }

        // POST: api/userinfo
        [HttpPost]
        [Route("createuserinfo")]
        public IHttpActionResult Create([FromBody] CreateUserInfoDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                    return BadRequest("Id é obrigatório");

                var info = new UserInfo
                {
                    Id = dto.Id,
                    Biography = dto.Biography,
                    CreatedOn = dto.CreatedOn ?? DateTime.Now,
                    CreatedBy = dto.CreatedBy,
                    UpdatedOn = dto.UpdatedOn,
                    UpdatedBy = dto.UpdatedBy,
                    IsStatusEmail = dto.IsStatusEmail,
                    HasStreamingAccount = dto.HasStreamingAccount,
                    IsCollaborator = dto.IsCollaborator,
                    BirthDate = dto.BirthDate,
                    Country = dto.Country,
                    CountryCode = dto.CountryCode,
                    Address = dto.Address
                };

                _database.CreateUserInfo(info);
                return Created($"api/userinfo/{info.Id}", info);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar info: {ex.Message}");
            }
        }

        // PUT: api/userinfo/{id}
        [HttpPut]
        [Route("updateuserinfo/{id}")]
        public IHttpActionResult Update(int id, [FromBody] UpdateUserInfoDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var info = _database.GetUserInfoById(id);
                if (info == null)
                    return NotFound();

                info.Biography = dto.Biography;
                info.CreatedOn = dto.CreatedOn;
                info.CreatedBy = dto.CreatedBy;
                info.UpdatedOn = dto.UpdatedOn;
                info.UpdatedBy = dto.UpdatedBy;
                info.IsStatusEmail = dto.IsStatusEmail;
                info.HasStreamingAccount = dto.HasStreamingAccount;
                info.IsCollaborator = dto.IsCollaborator;
                info.BirthDate = dto.BirthDate;
                info.Country = dto.Country;
                info.CountryCode = dto.CountryCode;
                info.Address = dto.Address;

                _database.UpdateUserInfo(info);
                return Ok(info);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar info: {ex.Message}");
            }
        }

        // DELETE: api/userinfo/{id}
        [HttpDelete]
        [Route("deleteuserinfo/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var info = _database.GetUserInfoById(id);
                if (info == null)
                    return NotFound();

                _database.DeleteUserInfo(id);
                return Ok(new { message = "Info deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar info: {ex.Message}");
            }
        }
    }
}
