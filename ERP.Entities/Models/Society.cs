using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Entities.Models
{
    public partial class Society : BaseEntity
    {
        public long SocietyId { get; set; }
        public string Title { get; set; }
        public string PrintName { get; set; }
        public string Image { get; set; }
        public string Abbrev { get; set; }
        public string SmallImage { get; set; }
        public ICollection<Possessions> Possessions { get; set; }
    }
}
