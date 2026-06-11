using System;

using System.Collections.Generic;

using System.Linq;

using System.Linq.Expressions;

using System.Threading;

using System.Threading.Tasks;

using AutoMapper;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.Donation.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Donation.Handler

{

    public class GetAllBloodDonationHandler : IRequestHandler<GetAllBloodDonationQuery, Tuple<IEnumerable<GetBloodDonation>, long>>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;



        public GetAllBloodDonationHandler(IUnitOfWork unitOfWork, IMapper mapper)

        {

            this.unitOfWork = unitOfWork;

            this.mapper = mapper;

        }



        public async Task<Tuple<IEnumerable<GetBloodDonation>, long>> Handle(GetAllBloodDonationQuery request, CancellationToken cancellationToken)

        {

            Expression<Func<Entities.Models.BloodDonation, bool>> predicate = x => x.IsActive == true && x.IsDelete == false

                && (!request.BloodDonorId.HasValue || request.BloodDonorId == 0 || x.BloodDonorId == request.BloodDonorId)

                && (request.DonorName == null || request.DonorName == "" || x.BloodDonor.Name.ToLower().Contains(request.DonorName.ToLower().Trim()))

                && (request.DonorCNIC == null || request.DonorCNIC == "" || x.BloodDonor.CNIC.ToLower().Contains(request.DonorCNIC.ToLower().Trim()));



            Expression<Func<Entities.Models.BloodDonation, object>>[] includes =

            {

                x => x.CreatedBy,

                x => x.BloodDonor,

                x => x.BloodDonor.BloodGroupMaster,

                x => x.BloodComponentType,

                x => x.BloodGroupMaster

            };

            Expression<Func<Entities.Models.BloodDonation, object>> orderByDesc = x => x.Id;

            var entity = unitOfWork.Repository<Entities.Models.BloodDonation>()

                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);



            var result = mapper.Map<IEnumerable<GetBloodDonation>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetBloodDonation>, long>(result, entity.Item2);

        }

    }

}

