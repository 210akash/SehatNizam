using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Section.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Section.Handler
{
    public class GetSectionByNameHandler : IRequestHandler<GetSectionByNameQuery, List<GetSection>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetSectionByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetSection>> Handle(GetSectionByNameQuery request, CancellationToken cancellationToken)
        {
            var Section = await unitOfWork.Repository<Entities.Models.Section>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _Section = mapper.Map<List<GetSection>>(Section);
            return _Section;
        }
    }
}
