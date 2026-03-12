using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Entities.Models
{
    public partial class PossessionDocuments : BaseEntity
    {
        public long PossessionId { get; set; }
        public string DocumentName { get; set; }
        public virtual Possessions Possession { get; set; }

        [NotMapped]
        public string FileName { get; set; }
    }
}
