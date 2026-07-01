using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Service.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Services.Handler
{
    public class GetAllServicesHandler : IRequestHandler<GetAllServicesQuery, Tuple<IEnumerable<GetService>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllServicesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetService>, long>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Service, bool>> predicate = x => x.IsActive == true
            && (request.DepartmentId  == null || x.DepartmentId  == request.DepartmentId)
            && (request.ServiceTypeId == null || x.ServiceTypeId == request.ServiceTypeId)
            && (request.IsSurgical == null || x.IsSurgical == request.IsSurgical)
            && (string.IsNullOrEmpty(request.Name) || x.Name.ToLower().Contains(request.Name.ToLower()));

            Expression<Func<Entities.Models.Service, object>>[] includes = {
                x => x.Department,
                x => x.ServiceType,
            };

            Expression<Func<Entities.Models.Service, object>> OrderBy = null;
            Expression<Func<Entities.Models.Service, object>> OrderByDescending = x => x.ModifiedDate ?? x.CreatedDate;
            var entity = unitOfWork.Repository<Entities.Models.Service>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDescending, null, includes);
            var Services = mapper.Map<IEnumerable<GetService>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetService>, long>(Services, entity.Item2);
        }
    }
}
