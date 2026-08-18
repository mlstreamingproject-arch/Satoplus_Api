using System;

namespace MeuProxySsl.DTOs
{
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Biography { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsStatusEmail { get; set; }
        public bool? HasStreamingAccount { get; set; }
        public bool? IsCollaborator { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Address { get; set; }
    }

    public class CreateUserInfoDto
    {
        public int Id { get; set; }
        public string Biography { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsStatusEmail { get; set; }
        public bool? HasStreamingAccount { get; set; }
        public bool? IsCollaborator { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Address { get; set; }
    }

    public class UpdateUserInfoDto
    {
        public string Biography { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsStatusEmail { get; set; }
        public bool? HasStreamingAccount { get; set; }
        public bool? IsCollaborator { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Address { get; set; }
    }
}
