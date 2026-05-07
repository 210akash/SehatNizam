using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RadiologyType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyType.Handler
{
    public class DeleteRadiologyTypeHandler : IRequestHandler<DeleteRadiologyTypeCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteRadiologyTypeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteRadiologyTypeCommand request, CancellationToken cancellationToken)
        {
            var radiologyType = await unitOfWork.Repository<ERP.Entities.Models.RadiologyType>().GetByIdAsync(request.Id);
            if (radiologyType == null)
            {
                return false;
            }

            radiologyType.IsDelete = true;
            radiologyType.IsActive = false;
            radiologyType.DeletedById = this.sessionProvider.Session.LoggedInUserId;
            radiologyType.DeletedDate = DateTime.Now;

            unitOfWork.Repository<ERP.Entities.Models.RadiologyType>().Update(radiologyType);
            await unitOfWork.CompleteAsync();

            return true;
        }
    }
}
