using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Entities.Models
{
    public class BiometricRequest : BaseEntity
    {
        public long PossessionsId { get; set; }
        public long? IntimationId { get; set; }
        public long? MemberId { get; set; }
        public string RequestToken { get; set; } 
        public bool IsVerified { get; set; } = false;
        public DateTime? VerifyOn { get; set; }

        public virtual Possessions Possessions { get; set; }
        public virtual Intimation Intimation { get; set; }
    }
}
