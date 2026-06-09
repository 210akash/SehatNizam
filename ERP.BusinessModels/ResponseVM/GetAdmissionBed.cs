using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAdmissionBed
    {
        public long Id { get; set; }
        public long AdmissionId { get; set; }
        public GetAdmission Admission { get; set; }
        public long BedId { get; set; }
        public GetBed Bed { get; set; }
        public DateTime? CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
