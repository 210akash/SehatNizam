using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RadiologyOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyOrder.Handler
{
    public class ConfirmRadiologyOrderHandler : IRequestHandler<ConfirmRadiologyOrderCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ConfirmRadiologyOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ConfirmRadiologyOrderCommand request, CancellationToken cancellationToken)
        {
            var RadiologyOrder = await unitOfWork.Repository<Entities.Models.RadiologyOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id,null,null, "RadiologyOrderType,RadiologyOrderType.Service");
            RadiologyOrder.StatusId = 5;
            RadiologyOrder.ModifiedDate = DateTime.Now;
            RadiologyOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.RadiologyOrder>().Update(RadiologyOrder);
            var payment = new AppointmentPayment
            {
                AppointmentId = RadiologyOrder.Id,
                VisitFee = RadiologyOrder.RadiologyType.Service.BasePrice,
                Discount = request.Discount,
                TotalPayable = RadiologyOrder.RadiologyType.Service.BasePrice - request.Discount,
                PaymentModeId = request.PaymentModeId,
                ServiceId = RadiologyOrder.RadiologyType.ServiceId,
                PaymentDate = DateTime.Now,
                PaymentStatusId = 3,
                CreatedById = sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDelete = false
            };

            await unitOfWork.Repository<AppointmentPayment>()
                .AddAsync(payment);

            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
