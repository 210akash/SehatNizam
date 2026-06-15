using System.Threading;

using System.Threading.Tasks;

using AutoMapper;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.BloodUnit.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.BloodUnit.Handler

{

    public class GetBloodUnitByIdHandler : IRequestHandler<GetBloodUnitByIdQuery, GetBloodUnit>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;



        public GetBloodUnitByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)

        {

            this.unitOfWork = unitOfWork;

            this.mapper = mapper;

        }



        public async Task<GetBloodUnit> Handle(GetBloodUnitByIdQuery request, CancellationToken cancellationToken)

        {

            var unit = await unitOfWork.Repository<Entities.Models.BloodUnit>()

                .GetFirstAsNoTrackingAsync(

                    x => x.Id == request.Id && x.IsActive == true,

                    null,

                    null,

                    "BloodComponentType,BloodGroupMaster,BloodFridge,BloodRack,BloodDonation,CreatedBy");



            return mapper.Map<GetBloodUnit>(unit);

        }

    }

}

