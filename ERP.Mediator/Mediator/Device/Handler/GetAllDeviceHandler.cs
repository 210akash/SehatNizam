using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Device.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Device.Handler
{
    public class GetAllDeviceHandler : IRequestHandler<GetAllDeviceQuery, Tuple<IEnumerable<GetDevice>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllDeviceHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetDevice>, long>> Handle(GetAllDeviceQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Device, bool>> predicate;

            Expression<Func<Entities.Models.Device, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("HR Manager") || roles.Contains("HR Exective"))
            {
                predicate = x => (request.IsActive == null || x.IsActive == request.IsActive)
                && (request.Name == "" || request.Name == null || x.Name == request.Name)
                && x.CompanyId == this.sessionProvider.Session.CompanyId;
            }
            else
            {
                predicate = x => (request.IsActive == null || x.IsActive == request.IsActive)
                  && (request.Name == "" || request.Name == null || x.Name == request.Name);
            }

            Expression<Func<Entities.Models.Device, object>> OrderBy = null;
            Expression<Func<Entities.Models.Device, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Device>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var Device = mapper.Map<IEnumerable<GetDevice>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetDevice>, long>(Device, entity.Item2);
        }
    }
}
