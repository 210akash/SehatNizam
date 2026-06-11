using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.CrossMatch.Query
{
    public class DeleteBloodCrossMatchQuery : IRequest<long>
    {
        public long Id { get; set; }
        public DeleteBloodCrossMatchQuery(long id) { Id = id; }
    }
}
