using System;

namespace MeuProxySsl.DTOs
{
    public class UserStatusDto
    {
        public int Id { get; set; }
        public bool? IsOnLine { get; set; }
        public DateTime? UpdateOn { get; set; }
    }

    public class CreateUserStatusDto
    {
        public int Id { get; set; }
        public bool? IsOnLine { get; set; }
        public DateTime? UpdateOn { get; set; }
    }

    public class UpdateUserStatusDto
    {
        public bool? IsOnLine { get; set; }
        public DateTime? UpdateOn { get; set; }
    }
}
