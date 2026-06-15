using System;

using System.Collections.Generic;

using System.Linq;

using System.Linq.Expressions;

using System.Threading;

using System.Threading.Tasks;

using AutoMapper;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.Request.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Request.Handler

{

    public class GetAllBloodRequestHandler : IRequestHandler<GetAllBloodRequestQuery, Tuple<IEnumerable<GetBloodRequest>, long>>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;



        public GetAllBloodRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)

        {

            this.unitOfWork = unitOfWork;

            this.mapper = mapper;

        }



        public async Task<Tuple<IEnumerable<GetBloodRequest>, long>> Handle(GetAllBloodRequestQuery request, CancellationToken cancellationToken)

        {

            Expression<Func<Entities.Models.BloodRequest, bool>> predicate = x => x.IsActive == true && x.IsDelete == false

                && (request.PatientCNIC == null || request.PatientCNIC == "" || (x.PatientCNIC ?? "").ToLower().Contains(request.PatientCNIC.ToLower().Trim()))

                && (!request.Status.HasValue || request.Status == 0 || x.Status == request.Status);



            Expression<Func<Entities.Models.BloodRequest, object>>[] includes =

            {

                x => x.CreatedBy,

                x => x.Admission,

                x => x.Appointment,

                x => x.BloodGroupMaster,

                x => x.BloodComponentType

            };

            Expression<Func<Entities.Models.BloodRequest, object>> orderByDesc = x => x.Id;

            var entity = unitOfWork.Repository<Entities.Models.BloodRequest>()

                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);



            var result = mapper.Map<IEnumerable<GetBloodRequest>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetBloodRequest>, long>(result, entity.Item2);

        }

    }

}

