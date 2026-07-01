using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class ConfirmAppoinmentHandler : IRequestHandler<ConfirmAppoinmentQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMediator mediator;
        private readonly SessionProvider sessionProvider;
        public ConfirmAppoinmentHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMediator mediator)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
        }

        public async Task<Tuple<long, string>> Handle(ConfirmAppoinmentQuery request, CancellationToken cancellationToken)
        {
            int check = 0;

            // Fetch the appointment with status 1
            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetFirstAsync(y => y.Id == request.Id && y.AppointmentStatusId == 1, null, null, null);

            if (appointment != null)
            {
                // Begin transaction
                await using var transaction = await unitOfWork.BeginTransactionAsync();

                // Update appointment status
                var updateAppointment = unitOfWork.Repository<Entities.Models.Appointment>().GetFirst(y => y.Id == appointment.Id);
                updateAppointment.AppointmentStatusId = 5;
                updateAppointment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                updateAppointment.ModifiedDate = DateTime.Now;
                updateAppointment.TokenNumber = await GenerateAppointmentCodeAsync(updateAppointment.DoctorId.Value, updateAppointment.AppointmentDate);
                unitOfWork.Repository<Entities.Models.Appointment>().Update(updateAppointment);

                // Update related AppointmentPayments
                var patient = unitOfWork.Repository<Entities.Models.Patient>()
                    .Find(x => x.Id == appointment.PatientId);
                if (patient != null && string.IsNullOrEmpty(patient.MRN))
                {
                    var mrn = await GenerateMrnAsync();
                    patient.MRN = mrn; // Set status to 3
                    patient.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    patient.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Patient>().Update(patient);
                }

                // Update related AppointmentPayments
                var payment = await unitOfWork.Repository<AppointmentPayment>()
                    .GetFirstAsNoTrackingAsync(x => x.AppointmentId == appointment.Id, null, null, "Service");

                var serviceAccounts = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                .GetAsync(x => x.PaymentModeId == payment.PaymentModeId
                && x.ServiceTypeId == payment.Service.ServiceTypeId
                && x.ProjectId == sessionProvider.Session.SelectedWarehouseId, null, null, "PaymentMode", null, null);

                if (payment != null)
                {
                    payment.Discount = request.Discount; // Set status to 3
                    payment.TotalPayable = payment.VisitFee - request.Discount; // Set status to 3
                    payment.PaymentStatusId = 3; // Set status to 3
                    payment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    payment.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<AppointmentPayment>().Update(payment);

                    var transactionCommand = GetAppointmentVoucherCommandAsync(
                        updateAppointment,
                        payment,
                        serviceAccounts.ToList(),
                        request.Discount);

                    await mediator.Send(transactionCommand, cancellationToken);
                }

                await unitOfWork.SaveChangesAsync();

                // 6. Commit transaction
                await transaction.CommitAsync();
                return new Tuple<long, string>(200, "Appointment Confirmed Successfully!");
            }

            return new Tuple<long, string>(500, "Error Confirming, Please contact system admin!");
        }

        private async Task<string> GenerateMrnAsync()
        {
            var lastPatient = await unitOfWork.Repository<Entities.Models.Patient>()
                .GetOneAsync(
                    x => !string.IsNullOrEmpty(x.MRN) && x.ProjectId == sessionProvider.Session.SelectedWarehouseId,
                    q => q.OrderByDescending(x => x.Id));

            int next = 1;

            if (lastPatient != null &&
                int.TryParse(lastPatient.MRN, out int lastNo))
            {
                next = lastNo + 1;
            }

            return next.ToString("D6");
        }

        private async Task<string> GenerateAppointmentCodeAsync(Guid doctorId, DateTime appointmentDate)
        {
            var projectId = sessionProvider.Session.SelectedWarehouseId;

            var repository = unitOfWork.Repository<Entities.Models.Appointment>();

            var appointmentsForDay = await repository.FindAllAsync(
                x => x.IsActive
                     && x.DoctorId == doctorId
                     && x.ProjectId == projectId
                     && x.AppointmentDate.Date == appointmentDate.Date
                     && !string.IsNullOrEmpty(x.TokenNumber)
            );

            int nextNumber = 1;

            var maxNumber = appointmentsForDay
                .Select(x =>
                {
                    int.TryParse(x.TokenNumber, out int num);
                    return num;
                })
                .DefaultIfEmpty(0)
                .Max();

            nextNumber = maxNumber + 1;

            return nextNumber.ToString("D7");
        }

        private SaveServiceTransactionCommand GetAppointmentVoucherCommandAsync(Entities.Models.Appointment appointment, AppointmentPayment payment, List<Entities.Models.ServiceAccount> serviceAccounts, decimal discount)
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
