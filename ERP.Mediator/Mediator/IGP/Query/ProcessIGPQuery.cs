using MediatR;

namespace ERP.Mediator.Mediator.IGP.Query
{
    public class ProcessIGPQuery : IRequest<bool>
    {
        public ProcessIGPQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}