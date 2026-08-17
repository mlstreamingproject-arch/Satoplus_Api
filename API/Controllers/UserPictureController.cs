using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("userpicture")]
    public class UserPictureController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/userpicture
        [HttpGet]
        [Route("getalluserpicture")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var pictures = _database.GetAllUserPictures();
                return Ok(pictures);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar pictures: {ex.Message}");
            }
        }

        // GET: api/userpicture/{id}
        [HttpGet]
        [Route("getuserpicturebyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var picture = _database.GetUserPictureById(id);
                if (picture == null)
                    return NotFound();
                return Ok(picture);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar picture: {ex.Message}");
            }
        }

        // POST: api/userpicture
        [HttpPost]
        [Route("createuserpicture")]
        public IHttpActionResult Create([FromBody] CreateUserPictureDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || dto.BinaryData == null)
                    return BadRequest("Id e BinaryData são obrigatórios");

                var picture = new UserPicture
                {
                    Id = dto.Id,
                    BinaryData = dto.BinaryData,
                    Name = dto.Name
                };

                _database.CreateUserPicture(picture);
                return Created($"api/userpicture/{picture.Id}", picture);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar picture: {ex.Message}");
            }
        }

        // PUT: api/userpicture/{id}
        [HttpPut]
        [Route("updateuserpicture/{id}")]
        public IHttpActionResult Update(int id, [FromBody] UpdateUserPictureDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var picture = _database.GetUserPictureById(id);
                if (picture == null)
                    return NotFound();

                picture.BinaryData = dto.BinaryData ?? picture.BinaryData;
                picture.Name = dto.Name ?? picture.Name;

                _database.UpdateUserPicture(picture);
                return Ok(picture);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar picture: {ex.Message}");
            }
        }

        // DELETE: api/userpicture/{id}
        [HttpDelete]
        [Route("deleteuserpicture/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var picture = _database.GetUserPictureById(id);
                if (picture == null)
                    return NotFound();

                _database.DeleteUserPicture(id);
                return Ok(new { message = "Picture deletada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar picture: {ex.Message}");
            }
        }
    }
}
