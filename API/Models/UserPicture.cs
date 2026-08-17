using System;

namespace MeuProxySsl.Models
{
    public class UserPicture
    {
        public int Id { get; set; }
        public byte[] BinaryData { get; set; }
        public string Name { get; set; }
    }
}
