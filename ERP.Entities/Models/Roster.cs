using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Roster : BaseEntityHistory
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public long StatusId { get; set; }
        public virtual Status Status { get; set; }
        public string Remarks { get; set; }
        public List<RosterDetail> RosterDetail { get; set; }
    }
}
