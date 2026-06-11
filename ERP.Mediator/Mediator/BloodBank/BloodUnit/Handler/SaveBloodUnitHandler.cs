using System;

using System.Threading;

using System.Threading.Tasks;

using ERP.Core.Provider;

using ERP.Mediator.Mediator.BloodBank.BloodUnit.Command;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.BloodUnit.Handler

{

    public class SaveBloodUnitHandler : IRequestHandler<SaveBloodUnitCommand, long>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly SessionProvider sessionProvider;



        public SaveBloodUnitHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)

        {

            this.unitOfWork = unitOfWork;

            this.sessionProvider = sessionProvider;

        }



        public async Task<long> Handle(SaveBloodUnitCommand request, CancellationToken cancellationToken)

        {

            var entity = await unitOfWork.Repository<Entities.Models.BloodUnit>()

                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);



            if (entity == null) return 404;



            entity.BloodFridgeId = request.BloodFridgeId;

            entity.BloodRackId = request.BloodRackId;

            entity.SlotNo = request.SlotNo;

            entity.Status = request.Status > 0 ? request.Status : entity.Status;

            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;

            entity.ModifiedDate = DateTime.Now;



            unitOfWork.Repository<Entities.Models.BloodUnit>().Update(entity);

            unitOfWork.SaveChanges();

            return 200;

        }

    }

}

