using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.VoucherType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.VoucherType.Handler
{
    public class GetAllVoucherTypeHandler : IRequestHandler<GetAllVoucherTypeQuery, Tuple<IEnumerable<GetVoucherType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllVoucherTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetVoucherType>, long>> Handle(GetAllVoucherTypeQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.VoucherType, bool>> predicate;

            Expression<Func<Entities.Models.VoucherType, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant"))
            {
                predicate = x => x.IsActive == true
                &&(request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower().Trim()))
                && x.CompanyId == this.sessionProvider.Session.CompanyId;
            }
            else
            {
                predicate = x => x.IsActive == true
                && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower().Trim()));
            }

            Expression<Func<Entities.Models.VoucherType, object>> OrderBy = null;
            Expression<Func<Entities.Models.VoucherType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.VoucherType>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var VoucherType = mapper.Map<IEnumerable<GetVoucherType>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetVoucherType>, long>(VoucherType, entity.Item2);
        }
    }
}
