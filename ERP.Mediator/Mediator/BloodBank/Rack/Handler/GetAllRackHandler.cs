using System;

using System.Collections.Generic;

using System.Linq;

using System.Linq.Expressions;

using System.Threading;

using System.Threading.Tasks;

using AutoMapper;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.Rack.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Rack.Handler

{

    public class GetAllRackHandler : IRequestHandler<GetAllRackQuery, Tuple<IEnumerable<GetBloodRack>, long>>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;



        public GetAllRackHandler(IUnitOfWork unitOfWork, IMapper mapper)

        {

            this.unitOfWork = unitOfWork;

            this.mapper = mapper;

        }



        public async Task<Tuple<IEnumerable<GetBloodRack>, long>> Handle(GetAllRackQuery request, CancellationToken cancellationToken)

        {

            Expression<Func<Entities.Models.BloodRack, bool>> predicate = x => x.IsActive == true && x.IsDelete == false

                && (request.Name == null || request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower().Trim()))

                && (!request.BloodFridgeId.HasValue || request.BloodFridgeId == 0 || x.BloodFridgeId == request.BloodFridgeId);



            Expression<Func<Entities.Models.BloodRack, object>>[] includes = { x => x.CreatedBy, x => x.BloodFridge };

            Expression<Func<Entities.Models.BloodRack, object>> orderByDesc = x => x.Id;

            var entity = unitOfWork.Repository<Entities.Models.BloodRack>()

                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);



            var result = mapper.Map<IEnumerable<GetBloodRack>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetBloodRack>, long>(result, entity.Item2);

        }

    }

}

