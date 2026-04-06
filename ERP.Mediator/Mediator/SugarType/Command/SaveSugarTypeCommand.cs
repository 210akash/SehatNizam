using MediatR;

namespace ERP.Mediator.Mediator.SugarType.Command
{
    public class SaveSugarTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
    }
}
