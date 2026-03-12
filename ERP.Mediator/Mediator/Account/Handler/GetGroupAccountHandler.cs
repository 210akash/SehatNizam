using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Account.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Account.Handler
{
    public class GetGroupAccountHandler : IRequestHandler<GetGroupAccountQuery, List<GetAccount>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetGroupAccountHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccount>> Handle(GetGroupAccountQuery request, CancellationToken cancellationToken)
        {
            var Account = await unitOfWork.Repository<Entities.Models.Account>().GetAsync(y =>
                 y.IsGroup == true &&
                 y.IsActive == true
            , null,null, "AccountFlow");
            var _Account = mapper.Map<List<GetAccount>>(Account);
            return _Account;
        }
    }
}
