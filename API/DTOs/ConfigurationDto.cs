using System;

namespace MeuProxySsl.DTOs
{
    public class ConfigurationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdateOn { get; set; }
        public int? UpdateBy { get; set; }
    }

    public class CreateConfigurationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdateOn { get; set; }
        public int? UpdateBy { get; set; }
    }

    public class UpdateConfigurationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public int? UpdateBy { get; set; }
    }
}
