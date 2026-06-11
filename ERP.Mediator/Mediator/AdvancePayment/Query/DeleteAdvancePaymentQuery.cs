using MediatR;

namespace ERP.Mediator.Mediator.AdvancePayments.Query
{
    public class DeleteAdvancePaymentsQuery : IRequest<bool>
    {
        public DeleteAdvancePaymentsQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}