using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.BloodGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.BloodGroup.Handler
{
    public class GetAllBloodGroupMasterHandler : IRequestHandler<GetAllBloodGroupMasterQuery, Tuple<IEnumerable<GetBloodGroupMaster>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllBloodGroupMasterHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetBloodGroupMaster>, long>> Handle(GetAllBloodGroupMasterQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.BloodGroupMaster, bool>> predicate = x => x.IsActive == true && x.IsDelete == false
                && (request.Name == null || request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower().Trim()));

            Expression<Func<Entities.Models.BloodGroupMaster, object>>[] includes = { x => x.CreatedBy };
            Expression<Func<Entities.Models.BloodGroupMaster, object>> orderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.BloodGroupMaster>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);

            var result = mapper.Map<IEnumerable<GetBloodGroupMaster>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetBloodGroupMaster>, long>(result, entity.Item2);
        }
    }
}
