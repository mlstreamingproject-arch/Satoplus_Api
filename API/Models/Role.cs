using System;

namespace MeuProxySsl.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Persistent { get; set; } = true;
        public string SS_Key { get; set; }
        public int? Espace_Id { get; set; }
        public bool? IsActive { get; set; } = true;
        public string Description { get; set; }
    }
}
