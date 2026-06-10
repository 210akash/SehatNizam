using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class AdmissionPackageMaster : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<AdmissionPackageDetail> AdmissionPackageDetail { get; set; }
    }
}
