using System;

namespace MeuProxySsl.Models
{
    public class UserAvatar
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte[] BinaryData { get; set; }
        public bool? IsActive { get; set; } = true;
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
