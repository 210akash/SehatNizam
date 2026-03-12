using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Entities.Models
{
    public class TransactionDocument : BaseEntity
    {
        public long TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }

        public string Path { get; set; }
        [NotMapped]
        public string FileName { get; set; }
    }
}
