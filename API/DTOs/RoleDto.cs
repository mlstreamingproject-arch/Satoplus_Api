namespace MeuProxySsl.DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Persistent { get; set; }
        public string SS_Key { get; set; }
        public int? Espace_Id { get; set; }
        public bool? IsActive { get; set; }
        public string Description { get; set; }
    }

    public class CreateRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Persistent { get; set; } = true;
        public string SS_Key { get; set; }
        public int? Espace_Id { get; set; }
        public bool? IsActive { get; set; } = true;
        public string Description { get; set; }
    }

    public class UpdateRoleDto
    {
        public string Name { get; set; }
        public bool? Persistent { get; set; }
        public string SS_Key { get; set; }
        public int? Espace_Id { get; set; }
        public bool? IsActive { get; set; }
        public string Description { get; set; }
    }
}
