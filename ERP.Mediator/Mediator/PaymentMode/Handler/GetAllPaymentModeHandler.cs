using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PaymentMode.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Handler
{
    public class GetAllPaymentModeHandler : IRequestHandler<GetAllPaymentModeQuery, Tuple<IEnumerable<GetPaymentMode>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllPaymentModeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetPaymentMode>, long>> Handle(GetAllPaymentModeQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.PaymentMode, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.PaymentMode, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager") || roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId;
            }

            Expression<Func<Entities.Models.PaymentMode, object>> OrderBy = null;
            Expression<Func<Entities.Models.PaymentMode, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.PaymentMode>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var PaymentMode = mapper.Map<IEnumerable<GetPaymentMode>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetPaymentMode>, long>(PaymentMode, entity.Item2);
        }
    }
}
