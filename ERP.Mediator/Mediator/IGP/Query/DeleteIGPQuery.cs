using MediatR;

namespace ERP.Mediator.Mediator.IGP.Query
{
    public class DeleteIGPQuery : IRequest<bool>
    {
        public DeleteIGPQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}