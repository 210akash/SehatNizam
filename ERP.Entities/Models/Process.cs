using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Entities.Models
{
    public class Process : BaseEntity
    {
        public long? PossessionsId { get; set; }
        [ForeignKey("FromStatus")]
        public long? FromStatusId { get; set; }
        [ForeignKey("ToStatus")]
        public long? ToStatusId { get; set; }
        public long? IntimationId { get; set; }
        public string Comments { get; set; }
        public string RegNo { get; set; }
        public bool? isHold { get; set; } = false;

        public virtual Status FromStatus { get; set; }
        public virtual Status ToStatus { get; set; }
        public virtual Possessions Possessions { get; set; }
        public virtual AspNetUsers ModifiedNavigation { get; set; }
        public virtual Intimation Intimation { get; set; }
    }
}
