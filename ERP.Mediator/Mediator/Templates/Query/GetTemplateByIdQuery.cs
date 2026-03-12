using MediatR;
using ERP.BusinessModels.ParameterVM;

namespace ERP.Mediator.Mediator.Templates.Query
{
    public class GetTemplateByIdQuery : IRequest<GetTemplates>
    {
        public GetTemplateByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}
