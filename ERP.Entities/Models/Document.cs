using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Entities.Models
{
    public class Document : BaseEntity
    {
        public long? IndentRequestId { get; set; }
        public virtual IndentRequest IndentRequest { get; set; }

        public string Path { get; set; }
        [NotMapped]
        public string FileName { get; set; }
    }
}
