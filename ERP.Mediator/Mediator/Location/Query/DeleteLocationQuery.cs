using MediatR;

namespace ERP.Mediator.Mediator.Location.Query
{
    public class DeleteLocationQuery : IRequest<bool>
    {
        public DeleteLocationQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}