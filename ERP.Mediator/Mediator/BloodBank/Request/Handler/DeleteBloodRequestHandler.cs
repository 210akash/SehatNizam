using System;

using System.Threading;

using System.Threading.Tasks;

using ERP.Core.Provider;

using ERP.Mediator.Mediator.BloodBank.Request.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Request.Handler

{

    public class DeleteBloodRequestHandler : IRequestHandler<DeleteBloodRequestQuery, bool>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly SessionProvider sessionProvider;



        public DeleteBloodRequestHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)

        {

            this.unitOfWork = unitOfWork;

            this.sessionProvider = sessionProvider;

        }



        public async Task<bool> Handle(DeleteBloodRequestQuery request, CancellationToken cancellationToken)

        {

            var entity = await unitOfWork.Repository<Entities.Models.BloodRequest>()

                .GetFirstAsNoTrackingAsync(y => y.Id == request.Id);

            if (entity == null) return false;



            entity.IsDelete = true;

            entity.IsActive = false;

            entity.DeleteDate = DateTime.Now;

            entity.ModifiedDate = DateTime.Now;

            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Repository<Entities.Models.BloodRequest>().Update(entity);

            await unitOfWork.SaveChangesAsync();

            return true;

        }

    }

}

