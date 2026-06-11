using System;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Request.Command
{
    public class SaveBloodRequestCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long? AdmissionId { get; set; }
        public string PatientName { get; set; }
        public string PatientCNIC { get; set; }
        public long BloodGroupMasterId { get; set; }
        public long BloodComponentTypeId { get; set; }
        public int Quantity { get; set; }
        public DateTime RequestDate { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
    }
}
