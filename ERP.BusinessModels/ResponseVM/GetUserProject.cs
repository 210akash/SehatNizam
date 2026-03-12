using System;
namespace ERP.BusinessModels.ResponseVM
{
    public class GetUserProject
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long ProjectId { get; set; }
        public virtual GetProject Project { get; set; }

        public Guid UserId { get; set; }
        public virtual GetUser User { get; set; }
    }
}
