using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Row.Query;
using ERP.Mediator.Mediator.Section.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Section.Handler
{

    public class GetSectionByRowIdHandler : IRequestHandler<GetSectionByRowIdQuery, List<GetSection>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetSectionByRowIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }


        public async Task<List<GetSection>> Handle(GetSectionByRowIdQuery request, CancellationToken cancellationToken)
        {
            var _section = await unitOfWork.Repository<Entities.Models.Section>().GetAsync(y => y.RowId == request.Id);
            var _Section = mapper.Map<List<GetSection>>(_section);
            return _Section;
        }
    }
}
