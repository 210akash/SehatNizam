using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class GetDispatchByCompanyHandler : IRequestHandler<GetDispatchByCompanyQuery, List<GetDispatch>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDispatchByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDispatch>> Handle(GetDispatchByCompanyQuery request, CancellationToken cancellationToken)
        {
            var Dispatch = await unitOfWork.Repository<Entities.Models.Dispatch>().GetAsync();
            var _Dispatch = mapper.Map<List<GetDispatch>>(Dispatch);
            return _Dispatch;
        }
    }
}
