using System;

using System.Collections.Generic;

using System.Linq;

using System.Linq.Expressions;

using System.Threading;

using System.Threading.Tasks;

using AutoMapper;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.Fridge.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Fridge.Handler

{

    public class GetAllFridgeHandler : IRequestHandler<GetAllFridgeQuery, Tuple<IEnumerable<GetBloodFridge>, long>>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;



        public GetAllFridgeHandler(IUnitOfWork unitOfWork, IMapper mapper)

        {

            this.unitOfWork = unitOfWork;

            this.mapper = mapper;

        }



        public async Task<Tuple<IEnumerable<GetBloodFridge>, long>> Handle(GetAllFridgeQuery request, CancellationToken cancellationToken)

        {

            Expression<Func<Entities.Models.BloodFridge, bool>> predicate = x => x.IsActive == true && x.IsDelete == false

                && (request.Name == null || request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower().Trim()));



            Expression<Func<Entities.Models.BloodFridge, object>>[] includes = { x => x.CreatedBy };

            Expression<Func<Entities.Models.BloodFridge, object>> orderByDesc = x => x.Id;

            var entity = unitOfWork.Repository<Entities.Models.BloodFridge>()

                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);



            var result = mapper.Map<IEnumerable<GetBloodFridge>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetBloodFridge>, long>(result, entity.Item2);

        }

    }

}

