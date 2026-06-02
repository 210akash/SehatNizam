using MediatR;
using System;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class ConfirmAppoinmentQuery : IRequest<Tuple<long, string>>
    {
        public long Id { get; set; }
        public long Discount { get; set; }
        public long PaymentModeId { get; set; }
    }
}