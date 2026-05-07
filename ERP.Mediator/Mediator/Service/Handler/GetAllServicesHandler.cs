using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Service.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Service.Handler
{
    public class GetAllServicesHandler : IRequestHandler<GetAllServicesQuery, IEnumerable<GetService>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllServicesHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<IEnumerable<GetService>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Service, bool>> predicate = x =>
                x.IsActive == true &&
                x.IsDelete == false &&
                x.CompanyId == this.sessionProvider.Session.CompanyId;

            if (request.DepartmentId.HasValue && request.DepartmentId.Value > 0)
            {
                predicate = x => x.IsActive == true &&
                    x.IsDelete == false &&
                    x.CompanyId == this.sessionProvider.Session.CompanyId &&
                    x.DepartmentId == request.DepartmentId.Value;
            }

            Expression<Func<Entities.Models.Service, object>>[] includes = {
                x => x.Department
            };

            var services = await unitOfWork.Repository<Entities.Models.Service>().GetWhereAsync(predicate, null, includes);

            var result = mapper.Map<IEnumerable<GetService>>(services.ToList());

            // Fill department names
            foreach (var item in result)
            {
                var entity = services.FirstOrDefault(x => x.Id == item.Id);
                if (entity != null && entity.Department != null)
                {
                    item.DepartmentName = entity.Department.Name;
                }
            }

            return result.OrderBy(x => x.Name);
        }
    }
}
