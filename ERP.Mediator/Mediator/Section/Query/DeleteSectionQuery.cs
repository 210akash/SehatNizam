using MediatR;

namespace ERP.Mediator.Mediator.Section.Query
{
    public class DeleteSectionQuery : IRequest<long>
    {
        public DeleteSectionQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}