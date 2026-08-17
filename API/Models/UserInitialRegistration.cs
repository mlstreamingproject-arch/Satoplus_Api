using System;

namespace MeuProxySsl.Models
{
    public class UserInitialRegistration
    {
        public long Id { get; set; }
        public bool? Status { get; set; }
        public string Email { get; set; }
        public string PlataformTypeId { get; set; }
        public string IP { get; set; }
        public string Token { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string V_OS { get; set; }
        public string V_Browser { get; set; }
        public string Deeplink { get; set; }
        public string Password { get; set; }
    }
}
