using System;

namespace MeuProxySsl.DTOs
{
    public class UserPositionDto
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public long? PositionId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class CreateUserPositionDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public long PositionId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class UpdateUserPositionDto
    {
        public int? UserId { get; set; }
        public long? PositionId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
