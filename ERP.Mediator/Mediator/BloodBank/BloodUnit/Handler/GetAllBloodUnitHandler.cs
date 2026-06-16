using System;

using System.Collections.Generic;

using System.Linq;

using System.Linq.Expressions;

using System.Threading;

using System.Threading.Tasks;

using AutoMapper;

using ERP.BusinessModels.Enums;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.BloodUnit.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.BloodUnit.Handler

{

    public class GetAllBloodUnitHandler : IRequestHandler<GetAllBloodUnitQuery, Tuple<IEnumerable<GetBloodUnit>, long>>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;



        public GetAllBloodUnitHandler(IUnitOfWork unitOfWork, IMapper mapper)

        {

            this.unitOfWork = unitOfWork;

            this.mapper = mapper;

        }



        public async Task<Tuple<IEnumerable<GetBloodUnit>, long>> Handle(GetAllBloodUnitQuery request, CancellationToken cancellationToken)

        {

            Expression<Func<Entities.Models.BloodUnit, bool>> predicate = x => x.IsActive == true && x.IsDelete == false

                && (request.UnitNo == null || request.UnitNo == "" || x.UnitNo.ToLower().Contains(request.UnitNo.ToLower().Trim()))

                && (request.ComponentTypeName == null || request.ComponentTypeName == "" || x.BloodComponentType.Name.ToLower().Contains(request.ComponentTypeName.ToLower().Trim()))

                && (!request.BloodGroupMasterId.HasValue || request.BloodGroupMasterId == 0 || x.BloodGroupMasterId == request.BloodGroupMasterId)

                && (!request.Status.HasValue || request.Status == 0 || x.Status == request.Status)

                && (!request.StorageAssigned.HasValue || request.StorageAssigned == 0

                    || (request.StorageAssigned == 1 && x.BloodFridgeId.HasValue && x.BloodRackId.HasValue)

                    || (request.StorageAssigned == 2 && (!x.BloodFridgeId.HasValue || !x.BloodRackId.HasValue)))

                && (!request.ExcludeIssued.HasValue || !request.ExcludeIssued.Value || x.Status != (int)BloodUnitStatus.Issued);



            Expression<Func<Entities.Models.BloodUnit, object>>[] includes =

            {

                x => x.CreatedBy,

                x => x.BloodComponentType,

                x => x.BloodGroupMaster,

                x => x.BloodFridge,

                x => x.BloodRack,

                x => x.BloodDonation

            };

            Expression<Func<Entities.Models.BloodUnit, object>> orderByDesc = x => x.Id;

            var entity = unitOfWork.Repository<Entities.Models.BloodUnit>()

                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);



            var result = mapper.Map<IEnumerable<GetBloodUnit>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetBloodUnit>, long>(result, entity.Item2);

        }

    }

}

