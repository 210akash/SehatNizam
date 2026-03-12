using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Area.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Area.Handler
{
    public class GetAreaByNameHandler : IRequestHandler<GetAreaByNameQuery, List<GetArea>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAreaByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetArea>> Handle(GetAreaByNameQuery request, CancellationToken cancellationToken)
        {
            var area = await unitOfWork.Repository<Entities.Models.Area>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _area = mapper.Map<List<GetArea>>(area);
            return _area;
        }
    }
}
