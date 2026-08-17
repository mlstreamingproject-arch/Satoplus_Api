using System;

namespace MeuProxySsl.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public int? User_Id { get; set; }
        public int? Role_Id { get; set; }
    }
}
