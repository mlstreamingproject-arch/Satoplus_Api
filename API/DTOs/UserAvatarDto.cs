using System;

namespace MeuProxySsl.DTOs
{
    public class UserAvatarDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class CreateUserAvatarDto
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

    public class UpdateUserAvatarDto
    {
        public string Name { get; set; }
        public byte[] BinaryData { get; set; }
        public bool? IsActive { get; set; }
        public string Description { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
