using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAdmissionPackageMaster
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public GetUser CreatedBy { get; set; }
        public virtual List<GetAdmissionPackageDetail> AdmissionPackageDetail { get; set; }
    }
}
