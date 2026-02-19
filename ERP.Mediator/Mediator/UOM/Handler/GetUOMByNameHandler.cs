using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.UOM.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UOM.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetUOMByNameQuery, List<GetUOM>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetUOM>> Handle(GetUOMByNameQuery request, CancellationToken cancellationToken)
        {
            var UOM = await unitOfWork.Repository<Entities.Models.UOM>().GetAsync(y => y.Name == request.name);
            var _UOM = mapper.Map<List<GetUOM>>(UOM);
            return _UOM;
        }
    }
}
