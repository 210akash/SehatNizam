using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Account.Query;
using ERP.Mediator.Mediator.AccountFlow.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Account.Handler
{
    internal class GetAccountByAccountFlowHandler : IRequestHandler<GetAccountByAccountFlowQuery, List<GetAccountByAccountFlow>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAccountByAccountFlowHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccountByAccountFlow>> Handle(GetAccountByAccountFlowQuery request, CancellationToken cancellationToken)
        {
            var Account = await unitOfWork.Repository<Entities.Models.Account>().GetAsync(y => y.IsActive == true &&  y.AccountFlowId == request.AccountFlowId);
            var _Account = mapper.Map<List<GetAccountByAccountFlow>>(Account);
            return _Account;
        }
    }
    
    }

