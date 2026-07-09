using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Handler;
using ERP.Mediator.Mediator.AppointmentPayments.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Handler
{
    public class ApproveAppointmentPaymentsHandler : IRequestHandler<ApproveAppointmentPaymentsCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly HelperClass helperClass;
        private readonly IMediator mediator;

        public ApproveAppointmentPaymentsHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, HelperClass helperClass, IMediator mediator)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.helperClass = helperClass;
            this.mediator = mediator;
        }

        public async Task<long> Handle(ApproveAppointmentPaymentsCommand request, CancellationToken cancellationToken)
        {
            if (request.Payments == null || !request.Payments.Any())
            {
                return 400;
            }

            var paymentIds = request.Payments.Select(x => x.Id).ToList();
            var payments = await unitOfWork.Repository<Entities.Models.AppointmentPayment>()
                .GetAsync(x => x.AppointmentId == request.AppointmentId
                    && paymentIds.Contains(x.Id)
                    && x.IsActive
                    && !x.IsDelete
                    && (x.PaymentStatusId == 1 || x.PaymentStatusId == 2),null,null,"Service");

            if (payments == null)
            {
                return 404;
            }

            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
              .GetFirstAsNoTrackingAsync(x => x.Id == request.AppointmentId
                  && x.IsActive
                  && !x.IsDelete);

            await using var transaction = await unitOfWork.BeginTransactionAsync();

            var now = DateTime.Now;
            foreach (var item in request.Payments)
            {
                var payment = payments.First(x => x.Id == item.Id);

                if (item.Discount < 0 || item.Discount > payment.VisitFee)
                {
                    return 422;
                }

                var serviceAccounts = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
            .GetAsync(x => x.PaymentModeId == payment.PaymentModeId
            && x.ServiceTypeId == payment.Service.ServiceTypeId
            && x.ProjectId == sessionProvider.Session.SelectedWarehouseId, null, null, "PaymentMode", null, null);

                payment.Discount = item.Discount;
                payment.TotalPayable = payment.VisitFee - item.Discount;
                payment.PaymentStatusId = 3;
                payment.PaymentModeId = request.PaymentModeId;
                payment.PaymentDate = now;
                payment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                payment.ModifiedDate = now;
                unitOfWork.Repository<AppointmentPayment>().Update(payment);

                var transactionCommand = helperClass.GetAppointmentVoucherCommandAsync(appointment, payment, serviceAccounts.ToList(), payment.Discount);
                await mediator.Send(transactionCommand, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            return 200;
        }
    }
}
