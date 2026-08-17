using System;

namespace MeuProxySsl.DTOs
{
    public class UserPasswordRecoveryDto
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsValid { get; set; }
    }

    public class CreateUserPasswordRecoveryDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsValid { get; set; }
    }

    public class UpdateUserPasswordRecoveryDto
    {
        public int? UserId { get; set; }
        public bool? IsValid { get; set; }
    }
}
