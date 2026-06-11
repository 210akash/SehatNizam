using System;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Donor.Command
{
    public class SaveBloodDonorCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string DonorCode { get; set; }
        public string Name { get; set; }
        public string CNIC { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long? BloodGroupMasterId { get; set; }
        public long? PatientMasterId { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public bool IsDeferred { get; set; }
        public string DeferralReason { get; set; }
    }
}
