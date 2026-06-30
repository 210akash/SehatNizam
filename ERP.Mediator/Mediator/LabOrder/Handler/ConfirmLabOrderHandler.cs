using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.LabOrder.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.LabOrder.Handler
{
    public class ConfirmLabOrderHandler : IRequestHandler<ConfirmLabOrderCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public ConfirmLabOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMediator mediator)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
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
             .GetFirstAsync(x => x.Id == PaymentId, null, null, "Appointment");
            if (payment != null)
            {
                var serviceAccounts = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                .GetAsync(x => x.PaymentModeId == payment.PaymentModeId
                && x.ServiceId == payment.ServiceId
                && x.ProjectId == sessionProvider.Session.SelectedWarehouseId, null, null, "PaymentMode", null, null);

                var transactionCommand = GetAppointmentVoucherCommandAsync(
                           payment.Appointment,
                           payment,
                           serviceAccounts.ToList(),
                           payment.Discount);

                await mediator.Send(transactionCommand);

                return 200;
            }
            return 400;
        }

        private SaveServiceTransactionCommand GetAppointmentVoucherCommandAsync(Entities.Models.Appointment appointment, Entities.Models.AppointmentPayment payment, List<Entities.Models.ServiceAccount> serviceAccounts, decimal discount)
        {
            var payable = serviceAccounts.First(x => x.AccountType == ServiceAccountType.Payable);

            var currentuser = unitOfWork.Repository<AspNetUsers>()
            .GetFirst(x => x.Id == sessionProvider.Session.LoggedInUserId);

            var command = new SaveServiceTransactionCommand
            {
                Date = appointment.AppointmentDate,
                ReferenceNumber = appointment.TokenNumber,
                VoucherTypeId = payable.PaymentMode.VoucherTypeId.Value,
                Remarks = $"Appointment Payment Against Token - {appointment.TokenNumber}",
                PaidReceiveBy = $"Receive By -  {currentuser.FirstName + " " + currentuser.LastName}",
                StatusId = 3,
                AppoinmentsPayments = payment.Id,
                TransactionDetails = new List<SaveTransactionDetailCommand>(),
                TransactionDocuments = null
            };

            // Debit (Cash/Bank)
            command.TransactionDetails.Add(new SaveTransactionDetailCommand
            {
                AccountId = payable.DebitAccountId,
                DepartmentId = appointment.DepartmentId,
                ProjectId = sessionProvider.Session.SelectedWarehouseId,
                AppointmentPaymentId = payment.Id,
                DebitAmount = payment.TotalPayable,
                CreditAmount = 0
            });

            // Credit (Income)
            command.TransactionDetails.Add(new SaveTransactionDetailCommand
            {
                AccountId = payable.CreditAccountId,
                DepartmentId = appointment.DepartmentId,
                ProjectId = sessionProvider.Session.SelectedWarehouseId,
                AppointmentPaymentId = payment.Id,
                DebitAmount = 0,
                CreditAmount = payment.TotalPayable
            });
            var discountAccount = serviceAccounts.First(x => x.AccountType == ServiceAccountType.Discount);

            if (discount > 0 && discountAccount != null)
            {
                // Debit Discount
                command.TransactionDetails.Add(new SaveTransactionDetailCommand
                {
                    AccountId = discountAccount.DebitAccountId,
                    DepartmentId = appointment.DepartmentId,
                    ProjectId = sessionProvider.Session.SelectedWarehouseId,
                    AppointmentPaymentId = payment.Id,
                    DebitAmount = discount,
                    CreditAmount = 0
                });

                // Credit Discount Offset
                command.TransactionDetails.Add(new SaveTransactionDetailCommand
                {
                    AccountId = discountAccount.CreditAccountId,
                    DepartmentId = appointment.DepartmentId,
                    ProjectId = sessionProvider.Session.SelectedWarehouseId,
                    AppointmentPaymentId = payment.Id,
                    DebitAmount = 0,
                    CreditAmount = discount
                });
            }

            return command;
        }
    }
}
