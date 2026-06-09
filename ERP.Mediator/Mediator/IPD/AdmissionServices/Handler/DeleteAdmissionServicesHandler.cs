using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.AdmissionServices.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionServices.Handler
{
    public class DeleteAdmissionServicesHandler : IRequestHandler<DeleteAdmissionServicesQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteAdmissionServicesHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAdmissionServicesQuery request, CancellationToken cancellationToken)
        {
            var AdmissionServices = await unitOfWork.Repository<Entities.Models.AppointmentPayment>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AdmissionServices.IsDelete = true;
            AdmissionServices.IsActive = false;
            AdmissionServices.ModifiedDate = DateTime.Now;
            AdmissionServices.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AppointmentPayment>().Update(AdmissionServices);
            return true;
        }
    }
}
