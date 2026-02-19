using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountHead.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountHead.Handler
{
    public class GetAllAccountHeadHandler : IRequestHandler<GetAllAccountHeadQuery, Tuple<IEnumerable<GetAccountHead>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAccountHeadHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAccountHead>, long>> Handle(GetAllAccountHeadQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.AccountHead, bool>> predicate;

            Expression<Func<Entities.Models.AccountHead, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant"))
            {
                predicate = x => x.IsActive == true
                &&(request.Name == "" || request.Name == null || x.Name == request.Name)
                && x.CompanyId == this.sessionProvider.Session.CompanyId;
            }
            else
            {
                predicate = x => x.IsActive == true
                  && (request.Name == "" || request.Name == null || x.Name == request.Name);
            }

            Expression<Func<Entities.Models.AccountHead, object>> OrderBy = null;
            Expression<Func<Entities.Models.AccountHead, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.AccountHead>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var AccountHead = mapper.Map<IEnumerable<GetAccountHead>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetAccountHead>, long>(AccountHead, entity.Item2);
        }
    }
}
