using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Store.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetStoreByCompanyQuery, List<GetStore>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetStore>> Handle(GetStoreByCompanyQuery request, CancellationToken cancellationToken)
        {
            // Determine the CompanyId to use: request.CompanyId or sessionProvider's CompanyId
            long companyId = request.CompanyId != 0 ? request.CompanyId : sessionProvider.Session.CompanyId;

            // Build the predicate based on FixedAsset condition
            Expression<Func<Entities.Models.Store, bool>> predicate = store =>
                store.Location.CompanyId == companyId &&
                (!request.FixedAsset || store.FixedAsset == request.FixedAsset);

            // Fetch the stores based on the predicate
            var stores = await unitOfWork.Repository<Entities.Models.Store>().GetAsync(predicate);

            // Map the result to the desired DTO
            return mapper.Map<List<GetStore>>(stores);
        }
    }
}
