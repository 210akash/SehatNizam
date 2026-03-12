using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class PriceGroup : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public virtual List<PriceGroupDetails> PriceGroupDetails {get; set;}
    }
}
