using AutoMapper;
using MediatR;
using ERP.BusinessModels.ParameterVM;
using ERP.Mediator.Mediator.Templates.Query;
using ERP.Repositories.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Templates.Handler
{
    public class GetTemplateByIdHandler : IRequestHandler<GetTemplateByIdQuery, GetTemplates>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTemplateByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetTemplates> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var tempalte = await unitOfWork.Repository<Entities.Models.Templates>().FindAsync(y => y.Id == request.Id);
            var _tempalte = mapper.Map<GetTemplates>(tempalte);
            return _tempalte;
        }
    }
}
