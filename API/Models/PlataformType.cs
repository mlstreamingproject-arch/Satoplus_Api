using System;

namespace MeuProxySsl.Models
{
    public class PlataformType
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public int? Order { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
