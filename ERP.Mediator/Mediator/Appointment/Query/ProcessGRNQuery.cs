using MediatR;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class ProcessGRNQuery : IRequest<bool>
    {
        public ProcessGRNQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}