using MediatR;

namespace ERP.Mediator.Mediator.EmployeeWorkSiteType.Command
{
    public class SaveEmployeeWorkSiteTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
