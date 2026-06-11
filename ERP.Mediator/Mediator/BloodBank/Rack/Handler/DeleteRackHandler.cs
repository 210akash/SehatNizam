using System;

using System.Threading;

using System.Threading.Tasks;

using ERP.Core.Provider;

using ERP.Mediator.Mediator.BloodBank.Rack.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.Rack.Handler

{

    public class DeleteRackHandler : IRequestHandler<DeleteRackQuery, bool>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly SessionProvider sessionProvider;



        public DeleteRackHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)

        {

            this.unitOfWork = unitOfWork;

            this.sessionProvider = sessionProvider;

        }



        public async Task<bool> Handle(DeleteRackQuery request, CancellationToken cancellationToken)

        {

            var entity = await unitOfWork.Repository<Entities.Models.BloodRack>()

                .GetFirstAsNoTrackingAsync(y => y.Id == request.Id);

            if (entity == null) return false;



            entity.IsDelete = true;

            entity.IsActive = false;

            entity.DeleteDate = DateTime.Now;

            entity.ModifiedDate = DateTime.Now;

            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Repository<Entities.Models.BloodRack>().Update(entity);

            await unitOfWork.SaveChangesAsync();

            return true;

        }

    }

}

