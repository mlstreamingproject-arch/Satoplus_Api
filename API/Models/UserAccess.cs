using System;

namespace MeuProxySsl.Models
{
    public class UserAccess
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public long? UserPerfilId { get; set; }
        public string PlataformTypeId { get; set; }
        public string IP { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
