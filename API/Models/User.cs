using System;

namespace MeuProxySsl.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string External_Id { get; set; }
        public DateTime? Creation_Date { get; set; }
        public DateTime? Last_Login { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
