using MediatR;

namespace ERP.Mediator.Mediator.IGPType.Query
{
    public class DeleteIGPTypeQuery : IRequest<bool>
    {
        public DeleteIGPTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}