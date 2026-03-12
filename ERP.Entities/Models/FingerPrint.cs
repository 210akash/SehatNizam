using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Entities.Models
{
    public partial class FingerPrint : BaseEntity
    {
        public string Title { get; set; }
        public long? MemberId { get; set; }
        public long? FileId { get; set; }
        public string Thumb { get; set; }
        public string ThumbImage { get; set; }
    }
}
