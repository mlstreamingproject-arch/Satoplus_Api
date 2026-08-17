namespace MeuProxySsl.DTOs
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int? User_Id { get; set; }
        public int? Role_Id { get; set; }
    }

    public class CreateUserRoleDto
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int Role_Id { get; set; }
    }

    public class UpdateUserRoleDto
    {
        public int? User_Id { get; set; }
        public int? Role_Id { get; set; }
    }
}
