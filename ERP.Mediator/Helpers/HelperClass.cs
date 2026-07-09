using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Handler
{
    public class HelperClass
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public HelperClass(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public SaveServiceTransactionCommand GetAppointmentVoucherCommandAsync(Entities.Models.Appointment appointment, AppointmentPayment payment, List<Entities.Models.ServiceAccount> serviceAccounts, decimal discount)
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
            var discountAccount = serviceAccounts.FirstOrDefault(x => x.AccountType == ServiceAccountType.Discount);

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
