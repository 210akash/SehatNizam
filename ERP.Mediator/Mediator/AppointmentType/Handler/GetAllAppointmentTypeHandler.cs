using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AppointmentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AppointmentType.Handler
{
    public class GetAllAppointmentTypeHandler : IRequestHandler<GetAllAppointmentTypeQuery, Tuple<IEnumerable<GetAppointmentType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAppointmentTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAppointmentType>, long>> Handle(GetAllAppointmentTypeQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.AppointmentType, bool>> predicate;

            Expression<Func<Entities.Models.AppointmentType, object>>[] includes = {
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

            Expression<Func<Entities.Models.AppointmentType, object>> OrderBy = null;
            Expression<Func<Entities.Models.AppointmentType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.AppointmentType>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var AppointmentType = mapper.Map<IEnumerable<GetAppointmentType>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetAppointmentType>, long>(AppointmentType, entity.Item2);
        }
    }
}
