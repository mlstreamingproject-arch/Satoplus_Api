using System;

namespace MeuProxySsl.Models
{
    public class UserStatus
    {
        public int Id { get; set; }
        public bool? IsOnLine { get; set; }
        public DateTime? UpdateOn { get; set; }
    }
}
