namespace MeuProxySsl.DTOs
{
    public class UserPictureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateUserPictureDto
    {
        public int Id { get; set; }
        public byte[] BinaryData { get; set; }
        public string Name { get; set; }
    }

    public class UpdateUserPictureDto
    {
        public byte[] BinaryData { get; set; }
        public string Name { get; set; }
    }
}
