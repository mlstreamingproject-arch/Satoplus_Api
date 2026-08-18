using System;

namespace MeuProxySsl.DTOs
{
    public class UserPerfilDto
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public bool? IsActive { get; set; }
        public string Name { get; set; }
        public long? UserAvatarId { get; set; }
        public bool? IsChild { get; set; }
        public bool? IsMain { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }

    public class CreateUserPerfilDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public long? UserAvatarId { get; set; }
        public bool? IsChild { get; set; } = false;
        public bool? IsMain { get; set; } = false;
        public bool? IsActive { get; set; } = true;
        public DateTime? CreatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }

    public class UpdateUserPerfilDto
    {
        public int? UserId { get; set; }
        public bool? IsActive { get; set; }
        public string Name { get; set; }
        public long? UserAvatarId { get; set; }
        public bool? IsChild { get; set; }
        public bool? IsMain { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
