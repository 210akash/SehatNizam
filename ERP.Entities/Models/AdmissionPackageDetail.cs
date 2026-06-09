namespace ERP.Entities.Models
{
    public class AdmissionPackageDetail : BaseEntity
    {
        public long AdmissionPackageMasterId { get; set; }
        public virtual AdmissionPackageMaster AdmissionPackageMaster { get; set; }

        public long ServiceId { get; set; }
        public virtual Service Service { get; set; }
    }
}
