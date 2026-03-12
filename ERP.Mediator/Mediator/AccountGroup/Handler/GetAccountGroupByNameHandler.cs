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
    public class GetAccountGroupByNameHandler : IRequestHandler<GetAccountGroupByNameQuery, List<GetAccountGroup>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAccountGroupByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccountGroup>> Handle(GetAccountGroupByNameQuery request, CancellationToken cancellationToken)
        {
            var Account = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetAsync(y =>
                (y.Name.ToLower().Contains(request.name.ToLower()) ||
                 y.Code.ToLower().Contains(request.name.ToLower())) &&
                 y.IsActive == true
            , null, null, null);

            List<Entities.Models.AccountGroup> _Accounts = new();

            //if (request.accountFlow.Count != 0)
            //{

            //    foreach (var item in Account)
            //    {
            //        if (request.accountFlow.Any(af => item.AccountFlow.Name.Contains(af)))
            //        {
            //            _Accounts.Add(item);
            //        }
            //    }
            //}
            //else
            //{
                _Accounts = Account.ToList();
           // }
            var _Account = mapper.Map<List<GetAccountGroup>>(_Accounts);
            return _Account;
        }
    }
}
