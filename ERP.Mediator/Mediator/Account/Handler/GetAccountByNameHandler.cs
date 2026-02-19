using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Account.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Account.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetAccountByNameQuery, List<GetAccount>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }



        public async Task<List<GetAccount>> Handle(GetAccountByNameQuery request, CancellationToken cancellationToken)
        {
            var Account = await unitOfWork.Repository<Entities.Models.Account>().GetAsync(y =>
                (y.Name.ToLower().Contains(request.name.ToLower()) ||
                 y.Code.ToLower().Contains(request.name.ToLower()))
            , null, null, "AccountFlow,AccountType");

            List<Entities.Models.Account> _Accounts = new();

            if (request.accountFlow.Count != 0)
            {

                foreach (var item in Account)
                {
                    if (request.accountFlow.Any(af => item.AccountFlow.Name.Contains(af)))
                    {
                        _Accounts.Add(item);
                    }
                }
            }
            else
            {
                _Accounts = Account.ToList();
            }
            var _Account = mapper.Map<List<GetAccount>>(_Accounts);
            return _Account;
        }
    }
}
