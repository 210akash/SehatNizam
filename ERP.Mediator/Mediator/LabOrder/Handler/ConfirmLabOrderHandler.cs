using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.LabOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Mediator.Mediator.Handler;


namespace ERP.Mediator.Mediator.LabOrder.Handler
{
    public class ConfirmLabOrderHandler : IRequestHandler<ConfirmLabOrderCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;
        private readonly HelperClass helperClass;

        public ConfirmLabOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMediator mediator, HelperClass helperClass)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
            this.helperClass = helperClass;
        }

        public async Task<bool> Handle(ConfirmLabOrderCommand request, CancellationToken cancellationToken)
        {
            using var transaction =
              await unitOfWork.BeginTransactionAsync();

            var LabOrder = await unitOfWork.Repository<Entities.Models.LabOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id, null, null, "LabOrderType,LabOrderType.Service");
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
