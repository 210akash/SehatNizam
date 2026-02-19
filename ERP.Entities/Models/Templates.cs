using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Entities.Models
{
    //public partial class Templates : BaseEntity
    public partial class Templates 
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        //public string Description { get; set; }
        public string Content { get; set; }
        //public int Society_id { get; set; }
        //public int Code { get; set; }
        //public int Type { get; set; }
    }
}
