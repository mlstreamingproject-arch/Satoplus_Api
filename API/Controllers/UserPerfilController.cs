using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("userperfil")]
    public class UserPerfilController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/userperfil
        [HttpGet]
        [Route("getalluserperfil")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var perfis = _database.GetAllUserPerfis();
                return Ok(perfis);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar perfis: {ex.Message}");
            }
        }

        // GET: api/userperfil/{id}
        [HttpGet]
        [Route("getuserperfilbyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var perfil = _database.GetUserPerfilById(id);
                if (perfil == null)
                    return NotFound();
                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar perfil: {ex.Message}");
            }
        }

        // POST: api/userperfil
        [HttpPost]
        [Route("createuserperfil")]
        public IHttpActionResult Create([FromBody] CreateUserPerfilDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || dto.UserId <= 0 || string.IsNullOrEmpty(dto.Name))
                    return BadRequest("Id, UserId e Name são obrigatórios");

                var perfil = new UserPerfil
                {
                    Id = dto.Id,
                    UserId = dto.UserId,
                    IsActive = dto.IsActive ?? true,
                    Name = dto.Name,
                    UserAvatarId = dto.UserAvatarId,
                    IsChild = dto.IsChild ?? false,
                    IsMain = dto.IsMain ?? false,
                    CreatedOn = dto.CreatedOn ?? DateTime.Now,
                    DeletedOn = dto.DeletedOn
                };

                _database.CreateUserPerfil(perfil);
                return Created($"api/userperfil/{perfil.Id}", perfil);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar perfil: {ex.Message}");
            }
        }

        // PUT: api/userperfil/{id}
        [HttpPut]
        [Route("updateuserperfil/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdateUserPerfilDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var perfil = _database.GetUserPerfilById(id);
                if (perfil == null)
                    return NotFound();

                perfil.UserId = dto.UserId ?? perfil.UserId;
                perfil.IsActive = dto.IsActive ?? perfil.IsActive;
                perfil.Name = dto.Name ?? perfil.Name;
                perfil.UserAvatarId = dto.UserAvatarId ?? perfil.UserAvatarId;
                perfil.IsChild = dto.IsChild ?? perfil.IsChild;
                perfil.IsMain = dto.IsMain ?? perfil.IsMain;

                _database.UpdateUserPerfil(perfil);
                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar perfil: {ex.Message}");
            }
        }

        // DELETE: api/userperfil/{id}
        [HttpDelete]
        [Route("deleteuserperfil/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var perfil = _database.GetUserPerfilById(id);
                if (perfil == null)
                    return NotFound();

                _database.DeleteUserPerfil(id);
                return Ok(new { message = "Perfil deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar perfil: {ex.Message}");
            }
        }
    }
}
