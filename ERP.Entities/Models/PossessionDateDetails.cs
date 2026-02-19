using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Entities.Models
{
    public class PossessionDateDetails : BaseEntity
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public long PossessionsId { get; set; }
        public virtual Possessions Possessions { get; set; }
    }
}
