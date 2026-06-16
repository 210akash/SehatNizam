using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.LabOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.LabOrder.Handler
{
    public class ConfirmLabOrderHandler : IRequestHandler<ConfirmLabOrderCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ConfirmLabOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ConfirmLabOrderCommand request, CancellationToken cancellationToken)
        {
            var LabOrder = await unitOfWork.Repository<Entities.Models.LabOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id,null,null, "LabOrderType,LabOrderType.Service");
            LabOrder.StatusId = 5;
            LabOrder.ModifiedDate = DateTime.Now;
            LabOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.LabOrder>().Update(LabOrder);
            var payment = new AppointmentPayment
            {
                AppointmentId = LabOrder.AppointmentId.Value,
                VisitFee = LabOrder.LabOrderType.Service.BasePrice,
                Discount = request.Discount,
                TotalPayable = LabOrder.LabOrderType.Service.BasePrice - request.Discount,
                PaymentModeId = request.PaymentModeId,
                ServiceId = LabOrder.LabOrderType.ServiceId,
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
