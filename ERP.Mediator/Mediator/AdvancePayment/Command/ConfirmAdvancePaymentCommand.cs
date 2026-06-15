using MediatR;
using System;

namespace ERP.Mediator.Mediator.AdvancePayments.Command
{
    public class ConfirmAdvancePaymentCommand : IRequest<bool>
    {
        public ConfirmAdvancePaymentCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}