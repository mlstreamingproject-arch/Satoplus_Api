using System;
using System.Web.Http;
using MeuProxySsl.DTOs;
using MeuProxySsl.Models;
using MeuProxySsl.Data;

namespace MeuProxySsl.Controllers
{
    [RoutePrefix("emailcontent")]
    public class EmailContentController : ApiController
    {
        private MySqlDatabase _database = new MySqlDatabase();

        // GET: api/emailcontent
        [HttpGet]
        [Route("getallemailcontent")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var emailContents = _database.GetAllEmailContents();
                return Ok(emailContents);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar conteúdos de email: {ex.Message}");
            }
        }

        // GET: api/emailcontent/{id}
        [HttpGet]
        [Route("getemailcontentbyid/{id}")]
        public IHttpActionResult GetById(long id)
        {
            try
            {
                var emailContent = _database.GetEmailContentById(id);
                if (emailContent == null)
                    return NotFound();
                return Ok(emailContent);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar conteúdo de email: {ex.Message}");
            }
        }

        // POST: api/emailcontent
        [HttpPost]
        [Route("createemailcontent")]
        public IHttpActionResult Create([FromBody] CreateEmailContentDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || string.IsNullOrEmpty(dto.Name))
                    return BadRequest("Id e Nome são obrigatórios");

                var emailContent = new EmailContent
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Tittle = dto.Tittle,
                    Greetings = dto.Greetings,
                    MainText = dto.MainText,
                    SecondaryText = dto.SecondaryText,
                    AuxiliarText = dto.AuxiliarText,
                    ButtonText = dto.ButtonText,
                    Link = dto.Link,
                    UpdateBy = dto.UpdateBy,
                    UpdateOn = dto.UpdateOn ?? DateTime.Now
                };

                _database.CreateEmailContent(emailContent);
                return Created($"api/emailcontent/{emailContent.Id}", emailContent);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar conteúdo de email: {ex.Message}");
            }
        }

        // PUT: api/emailcontent/{id}
        [HttpPut]
        [Route("updateemailcontent/{id}")]
        public IHttpActionResult Update(long id, [FromBody] UpdateEmailContentDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dados inválidos");

                var emailContent = _database.GetEmailContentById(id);
                if (emailContent == null)
                    return NotFound();

                emailContent.Name = dto.Name;
                emailContent.Tittle = dto.Tittle;
                emailContent.Greetings = dto.Greetings;
                emailContent.MainText = dto.MainText;
                emailContent.SecondaryText = dto.SecondaryText;
                emailContent.AuxiliarText = dto.AuxiliarText;
                emailContent.ButtonText = dto.ButtonText;
                emailContent.Link = dto.Link;
                emailContent.UpdateOn = dto.UpdateOn;
                emailContent.UpdateBy = dto.UpdateBy;

                _database.UpdateEmailContent(emailContent);
                return Ok(emailContent);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar conteúdo de email: {ex.Message}");
            }
        }

        // DELETE: api/emailcontent/{id}
        [HttpDelete]
        [Route("deleteemailcontent/{id}")]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                var emailContent = _database.GetEmailContentById(id);
                if (emailContent == null)
                    return NotFound();

                _database.DeleteEmailContent(id);
                return Ok(new { message = "Conteúdo de email deletado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar conteúdo de email: {ex.Message}");
            }
        }
    }
}
