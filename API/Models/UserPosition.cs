using System;

namespace MeuProxySsl.Models
{
    public class UserPosition
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public long? PositionId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
