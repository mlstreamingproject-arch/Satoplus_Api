using System;

namespace MeuProxySsl.DTOs
{
    public class UserAccessDto
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public long? UserPerfilId { get; set; }
        public string PlataformTypeId { get; set; }
        public string IP { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class CreateUserAccessDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public long? UserPerfilId { get; set; }
        public string PlataformTypeId { get; set; }
        public string IP { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class UpdateUserAccessDto
    {
        public int? UserId { get; set; }
        public long? UserPerfilId { get; set; }
        public string PlataformTypeId { get; set; }
        public string IP { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
