using MediatR;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class DeleteGRNQuery : IRequest<bool>
    {
        public DeleteGRNQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}