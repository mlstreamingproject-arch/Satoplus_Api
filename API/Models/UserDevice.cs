using System;

namespace MeuProxySsl.Models
{
    public class UserDevice
    {
        public long Id { get; set; }
        public string Version { get; set; }
        public string UUID { get; set; }
        public string Serial { get; set; }
        public string Platform { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public bool? IsVirtual { get; set; }
        public string GetCordova { get; set; }
        public string DeviceType { get; set; }
        public int? UserId { get; set; }
        public string UserInitialRegistrationToken { get; set; }
    }
}
