using MediatR;

namespace ERP.Mediator.Mediator.Templates.Query
{
    public class GetPrintTemplateQuery : IRequest<string>
    {
        public GetPrintTemplateQuery(long OrderId, long TemplateId, long DispatchId)
        {
            this.OrderId = OrderId;
            this.TemplateId = TemplateId;
            this.DispatchId = DispatchId;
        }

        public long OrderId { get; set; }
        public long TemplateId { get; set; }
        public long DispatchId { get; set; }
    }
}
