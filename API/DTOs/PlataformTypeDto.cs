namespace MeuProxySsl.DTOs
{
    public class PlataformTypeDto
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public int? Order { get; set; }
        public bool? IsActive { get; set; } = true;
    }

    public class CreatePlataformTypeDto
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public int? Order { get; set; }
        public bool? IsActive { get; set; } = true;
    }

    public class UpdatePlataformTypeDto
    {
        public string Label { get; set; }
        public int? Order { get; set; }
        public bool? IsActive { get; set; }
    }
}
