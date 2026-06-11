using System;

using System.Collections.Generic;

using System.Linq;

using System.Linq.Expressions;

using System.Threading;

using System.Threading.Tasks;

using AutoMapper;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.Donor.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Donor.Handler

{

    public class GetAllBloodDonorHandler : IRequestHandler<GetAllBloodDonorQuery, Tuple<IEnumerable<GetBloodDonor>, long>>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;



        public GetAllBloodDonorHandler(IUnitOfWork unitOfWork, IMapper mapper)

        {

            this.unitOfWork = unitOfWork;

            this.mapper = mapper;

        }



        public async Task<Tuple<IEnumerable<GetBloodDonor>, long>> Handle(GetAllBloodDonorQuery request, CancellationToken cancellationToken)

        {

            Expression<Func<Entities.Models.BloodDonor, bool>> predicate = x => x.IsActive == true && x.IsDelete == false

                && (request.Name == null || request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower().Trim()))

                && (request.CNIC == null || request.CNIC == "" || x.CNIC.ToLower().Contains(request.CNIC.ToLower().Trim()));



            Expression<Func<Entities.Models.BloodDonor, object>>[] includes = { x => x.CreatedBy, x => x.BloodGroupMaster };

            Expression<Func<Entities.Models.BloodDonor, object>> orderByDesc = x => x.Id;

            var entity = unitOfWork.Repository<Entities.Models.BloodDonor>()

                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);



            var result = mapper.Map<IEnumerable<GetBloodDonor>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetBloodDonor>, long>(result, entity.Item2);

        }

    }

}

