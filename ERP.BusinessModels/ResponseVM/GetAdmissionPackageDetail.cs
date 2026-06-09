namespace ERP.BusinessModels.ResponseVM
{
    public class GetAdmissionPackageDetail
    {
        public long Id { get; set; }
        public long AdmissionPackageMasterId { get; set; }
        public long ServiceId { get; set; }
        public GetService Service { get; set; }
    }
}
