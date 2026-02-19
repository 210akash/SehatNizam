using MediatR;

namespace ERP.Mediator.Mediator.UOM.Query
{
    public class DeleteUOMQuery : IRequest<bool>
    {
        public DeleteUOMQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}