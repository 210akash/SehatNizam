using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.BloodGroup.Query
{
    public class DeleteBloodGroupMasterQuery : IRequest<bool>
    {
        public long Id { get; set; }
        public DeleteBloodGroupMasterQuery(long id) { Id = id; }
    }
}
