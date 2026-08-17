using System;

namespace MeuProxySsl.Models
{
    public class UserPasswordRecovery
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsValid { get; set; }
    }
}
