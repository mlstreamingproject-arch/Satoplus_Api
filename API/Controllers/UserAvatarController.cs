using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("useravatar")]
    public class UserAvatarController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/useravatar
        [HttpGet]
        [Route("getalluseravatar")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var avatars = _database.GetAllUserAvatars();
                return Ok(avatars);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar avatars: {ex.Message}");
            }
        }

        // GET: api/useravatar/{id}
        [HttpGet]
        [Route("getuseravatarbyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var avatar = _database.GetUserAvatarById(id);
                if (avatar == null)
                    return NotFound();
                return Ok(avatar);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar avatar: {ex.Message}");
            }
        }

        // POST: api/useravatar
        [HttpPost]
        [Route("createuseravatar")]
        public IHttpActionResult Create([FromBody] CreateUserAvatarDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                    return BadRequest("Id é obrigatório");

                var avatar = new UserAvatar
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    BinaryData = dto.BinaryData,
                    IsActive = dto.IsActive ?? true,
                    Description = dto.Description,
                    CreatedOn = DateTime.Now,
                    CreatedBy = dto.CreatedBy
                };

                _database.CreateUserAvatar(avatar);
                return Created($"api/useravatar/{avatar.Id}", avatar);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar avatar: {ex.Message}");
            }
        }

        // PUT: api/useravatar/{id}
        [HttpPut]
        [Route("updateuseravatar/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdateUserAvatarDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var avatar = _database.GetUserAvatarById(id);
                if (avatar == null)
                    return NotFound();

                avatar.Name = dto.Name ?? avatar.Name;
                avatar.BinaryData = dto.BinaryData ?? avatar.BinaryData;
                avatar.IsActive = dto.IsActive ?? avatar.IsActive;
                avatar.Description = dto.Description ?? avatar.Description;
                avatar.UpdatedOn = DateTime.Now;
                avatar.UpdatedBy = dto.UpdatedBy ?? avatar.UpdatedBy;

                _database.UpdateUserAvatar(avatar);
                return Ok(avatar);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar avatar: {ex.Message}");
            }
        }

        // DELETE: api/useravatar/{id}
        [HttpDelete]
        [Route("deleteuseravatar/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var avatar = _database.GetUserAvatarById(id);
                if (avatar == null)
                    return NotFound();

                _database.DeleteUserAvatar(id);
                return Ok(new { message = "Avatar deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar avatar: {ex.Message}");
            }
        }
    }
}
