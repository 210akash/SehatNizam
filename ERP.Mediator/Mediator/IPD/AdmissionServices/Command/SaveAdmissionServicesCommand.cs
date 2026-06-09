using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionServices.Command
{
    public class SaveAdmissionServicesCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AdmissionId { get; set; }
        public long ServiceId { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPayable { get; set; }
        public long PaymentModeId { get; set; }
        public long PaymentStatusId { get; set; }
    }
}
