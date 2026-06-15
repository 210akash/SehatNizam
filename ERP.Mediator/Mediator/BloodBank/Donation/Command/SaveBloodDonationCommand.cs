using System;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Donation.Command
{
    public class SaveBloodDonationCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long? AppointmentId { get; set; }
        public long BloodDonorId { get; set; }
        public long BloodComponentTypeId { get; set; }
        public long? BloodGroupMasterId { get; set; }
        public DateTime DonationDate { get; set; }
        public decimal Volume { get; set; }
        public int ScreeningStatus { get; set; }
        public string Remarks { get; set; }
    }
}
