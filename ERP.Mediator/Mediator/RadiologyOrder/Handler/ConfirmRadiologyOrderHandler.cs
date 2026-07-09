using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RadiologyOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Mediator.Mediator.Handler;

namespace ERP.Mediator.Mediator.RadiologyOrder.Handler
{
    public class ConfirmRadiologyOrderHandler : IRequestHandler<ConfirmRadiologyOrderCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;
        private readonly HelperClass helperClass;

        public ConfirmRadiologyOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMediator mediator, HelperClass helperClass)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
            this.helperClass = helperClass;
        }

        public async Task<bool> Handle(ConfirmRadiologyOrderCommand request, CancellationToken cancellationToken)
        {
            using var transaction =
            await unitOfWork.BeginTransactionAsync();
            var RadiologyOrder = await unitOfWork.Repository<Entities.Models.RadiologyOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id,null,null, "RadiologyType,RadiologyType.Service");
            RadiologyOrder.StatusId = 5;
            RadiologyOrder.ModifiedDate = DateTime.Now;
            RadiologyOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.RadiologyOrder>().Update(RadiologyOrder);
            var payment = new AppointmentPayment
            {
                AppointmentId = RadiologyOrder.AppointmentId ?? 0,
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
            await SaveVouchersAgainstServices(payment.Id);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        private async Task<long> SaveVouchersAgainstServices(long PaymentId)
        {
            var payment =
             await unitOfWork.Repository<Entities.Models.AppointmentPayment>()
             .GetFirstAsync(x => x.Id == PaymentId, null, null, "Appointment,Service");

            if (payment != null)
            {
                var serviceAccounts = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                .GetAsync(x => x.PaymentModeId == payment.PaymentModeId
                && x.ServiceTypeId == payment.Service.ServiceTypeId
                && x.ProjectId == sessionProvider.Session.SelectedWarehouseId, null, null, "PaymentMode", null, null);

                var transactionCommand = helperClass.GetAppointmentVoucherCommandAsync(
                           payment.Appointment,
                           payment,
                           serviceAccounts.ToList(),
                           payment.Discount);

                await mediator.Send(transactionCommand);

                return 200;
            }
            return 400;
        }
    }
}
