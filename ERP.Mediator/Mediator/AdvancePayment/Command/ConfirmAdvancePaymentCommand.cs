using MediatR;
using System;

namespace ERP.Mediator.Mediator.AdvancePayments.Command
{
    public class ConfirmAdvancePaymentCommand : IRequest<Tuple<long, string>>
    {
        public long Id { get; set; }
    }
}