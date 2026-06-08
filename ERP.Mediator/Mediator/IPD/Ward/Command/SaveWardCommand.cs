using MediatR;
namespace ERP.Mediator.Mediator.IPD.Ward.Command
{
    public class SaveWardCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long DepartmentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
