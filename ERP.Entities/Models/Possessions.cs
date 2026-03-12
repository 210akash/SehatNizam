using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Entities.Models
{
    public partial class Possessions : BaseEntity
    {
        public long FileId { get; set; }
        public long? MemberId { get; set; }
        public string RegNo { get; set; }
        public string plotNumber { get; set; }
        public string ExcessLand { get; set; }
        public string PrimeLocation { get; set; }
        public string PlotSizeSqYd { get; set; }
        public string TotalArea { get; set; }
        public int BookingType { get; set; }
        public string StreetNo { get; set; }
        public string PrimePercent { get; set; }
        public string SectorId { get; set; }
        public string ColApplied { get; set; }
        public string TillDateReceived { get; set; }
        public string PrimeLocationCharges { get; set; }
        public string ExcessLandCharges { get; set; }
        public string CostLandExcessLand { get; set; }
        public string Balance { get; set; }
        public string Mobile { get; set; }
        public string PlotType { get; set; }
        public string RegDate { get; set; }
        public string Cnic { get; set; }
        public string Address { get; set; }
        public string MemberName { get; set; }
        public string QrCode { get; set; }
        public string PossessionStatus { get; set; }
        public int? OldUserId { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string ImageUser { get; set; }
        [NotMapped]
        public string FileSource { get; set; }
        public long SocietyId { get; set; }
        public string PermCharg { get; set; }
        public string PayableDcharg { get; set; }
        public DateTime? DcDate { get; set; }
        public int IsComplete { get; set; }
        public int? ProcessId { get; set; }
        public DateTime? NoticeDevDate { get; set; }
        public int? PalRecv { get; set; }
        public int? RatePerSqYard { get; set; }
        public bool? IsHold { get; set; } = false;
        public long? StatusId { get; set; }
        public virtual Status Status { get; set; }
        public virtual AspNetUsers ModifiedNavigation { get; set; }
        public virtual ICollection<Process> Process { get; set; }
        public virtual PlotDimension PlotDimension { get; set; }
        public Society Society { get; set; }
        public virtual PossessionDateDetails PossessionDateDetails { get; set; }
        public virtual List<PossessionDocuments> PossessionDocuments { get; set; }
        //public virtual List<BiometricRequest> BiometricRequest { get; set; }
    }
}
