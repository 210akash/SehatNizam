using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class SaveAppointmentHandler : IRequestHandler<SaveAppointmentCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public SaveAppointmentHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveAppointmentCommand request, CancellationToken cancellationToken)
        {
            using var transaction =
                await unitOfWork.BeginTransactionAsync();

            try
            {
                long result;

                if (request.Id > 0)
                {
                    result = await UpdateAppointmentAsync(
                        request,
                        cancellationToken);
                }
                else
                {
                    result = await CreateAppointmentAsync(
                        request,
                        cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return 200;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return 500;
                throw;
            }
        }

        private async Task<long> CreateAppointmentAsync(SaveAppointmentCommand request, CancellationToken cancellationToken)
        {

            try
            {
                // =====================================================
                // 1️⃣ CREATE / GET PATIENT
                // =====================================================

                long patientId = await GetOrCreatePatientAsync(request);

                // =====================================================
                // 2️⃣ CREATE APPOINTMENT
                // =====================================================
                var TokenNumber = "";
                if (request.DoctorId == null)
                    TokenNumber = await GenerateAppointmentCodeAsync();

                var appointment = new Entities.Models.Appointment
                {
                    AppointmentDate = request.AppointmentDate,
                    TokenNumber = TokenNumber,
                    ProjectId = sessionProvider.Session.SelectedWarehouseId,
                    DepartmentId = request.DepartmentId,
                    AppointmentTypeId = request.AppointmentTypeId,
                    PriorityLevelId = request.PriorityLevelId,
                    VisitTypeId = request.VisitTypeId,
                    PatientId = patientId,
                    DoctorId = request.DoctorId,
                    Reason = request.Reason,
                    ConfirmationNotes = request.ConfirmationNotes,
                    ConfirmedDate = request.ConfirmedDate,
                    AppointmentStatusId = request.AppointmentStatusId,
                    ReferrerId = request.ReferrerId,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDelete = false
                };

                await unitOfWork.Repository<Entities.Models.Appointment>()
                    .AddAsync(appointment);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                // =====================================================
                // 3️⃣ PAYMENT
                // =====================================================

                if (request.AppointmentPayment != null)
                {
                    foreach (var item in request.AppointmentPayment)
                    {
                        var payment = new AppointmentPayment
                        {
                            AppointmentId = appointment.Id,
                            VisitFee = item.VisitFee,
                            Discount = item.Discount,
                            TotalPayable = item.TotalPayable,
                            PaymentModeId = item.PaymentModeId,
                            ServiceId = item.ServiceId,
                            PaymentDate = DateTime.Now,
                            PaymentStatusId = item.PaymentStatusId,
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            IsActive = true,
                            IsDelete = false
                        };

                        await unitOfWork.Repository<AppointmentPayment>()
                            .AddAsync(payment);
                    }
                }

                // =====================================================
                // 4️⃣ LAB ORDERS
                // =====================================================

                if (request.LabOrders != null && request.LabOrders.Any())
                {
                    foreach (var item in request.LabOrders)
                    {
                        var labOrder = new Entities.Models.LabOrder
                        {
                            AppointmentId = appointment.Id,
                            LabOrderTypeId = item.LabOrderTypeId,
                            ClinicalNotes = item.ClinicalNotes,
                            StatusId = 5,
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            IsActive = true,
                            IsDelete = false
                        };

                        await unitOfWork.Repository<Entities.Models.LabOrder>()
                            .AddAsync(labOrder);
                    }
                }

                // =====================================================
                // 5️⃣ RADIOLOGY ORDERS
                // =====================================================

                if (request.RadiologyOrders != null && request.RadiologyOrders.Any())
                {
                    foreach (var item in request.RadiologyOrders)
                    {
                        var radiologyOrder = new Entities.Models.RadiologyOrder
                        {
                            AppointmentId = appointment.Id,
                            RadiologyTypeId = item.RadiologyTypeId,
                            ClinicalNotes = item.ClinicalNotes,
                            StatusId = 1,
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            IsActive = true,
                            IsDelete = false
                        };

                        await unitOfWork.Repository<Entities.Models.RadiologyOrder>()
                            .AddAsync(radiologyOrder);
                    }
                }

                // =====================================================
                // 6️⃣ SAVE ALL
                // =====================================================

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return 200;
            }
            catch
            {
                return 500;
                throw;
            }
        }

        private async Task<long> GetOrCreatePatientAsync(SaveAppointmentCommand request)
        {
            if (request.PatientId.HasValue &&
                request.PatientId.Value > 0)
            {
                return request.PatientId.Value;
            }

            if (request.Patient == null)
            {
                throw new Exception("Patient information is required.");
            }

            var project = await unitOfWork.Repository<Entities.Models.Project>()
                .GetOneAsync(x => x.Id == sessionProvider.Session.SelectedWarehouseId);
            string mrn = "";
            if (request.AppointmentStatusId != 1)
                mrn = await GenerateMrnAsync();

            var patient = new Entities.Models.Patient
            {
                Name = request.Patient.Name,
                Email = request.Patient.Email,
                PhoneNo = request.Patient.PhoneNo,
                SecondaryPhoneNo = request.Patient.SecondaryPhoneNo,
                Address = request.Patient.Address,
                CNIC = request.Patient.CNIC,
                Gender = request.Patient.Gender,
                Age = request.Patient.Age,
                DateOfBirth = request.Patient.DateOfBirth,
                CityId = request.Patient.CityId,
                ProjectId = sessionProvider.Session.SelectedWarehouseId,
                MRN = mrn,
                CreatedById = sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDelete = false
            };

            await unitOfWork.Repository<Entities.Models.Patient>()
                .AddAsync(patient);

            await unitOfWork.SaveChangesAsync();

            return patient.Id;
        }

        private async Task<string> GenerateMrnAsync()
        {
            var lastPatient = await unitOfWork.Repository<Entities.Models.Patient>()
                .GetOneAsync(
                    x => !string.IsNullOrEmpty(x.MRN),
                    q => q.OrderByDescending(x => x.Id));

            int next = 1;

            if (lastPatient != null &&
                int.TryParse(lastPatient.MRN, out int lastNo))
            {
                next = lastNo + 1;
            }

            return next.ToString("D6");
        }

        private async Task<string> GenerateAppointmentCodeAsync()
        {
            Func<IQueryable<Entities.Models.Appointment>,
                IOrderedQueryable<Entities.Models.Appointment>> orderBy =
                    q => q.OrderByDescending(x => x.Id);

            var lastAppointment =
                await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetOneAsync(x => x.IsActive, orderBy);

            int nextNumber = 1;

            if (lastAppointment != null &&
                !string.IsNullOrWhiteSpace(lastAppointment.TokenNumber))
            {
                int.TryParse(lastAppointment.TokenNumber, out nextNumber);

                nextNumber++;
            }

            return nextNumber.ToString("D7");
        }

        private async Task<long> UpdateAppointmentAsync(SaveAppointmentCommand request, CancellationToken cancellationToken)
        {
            if (request.AppointmentStatusId == 1)
            {
                var patient =
                   await unitOfWork.Repository<Entities.Models.Patient>()
                   .GetFirstAsync(x => x.Id == request.PatientId);

                if (patient == null)
                {
                    return 404;
                }

                // =========================================
                // UPDATE APPOINTMENT
                // =========================================

                patient.MRN = await GenerateMrnAsync();
                patient.ModifiedById = sessionProvider.Session.LoggedInUserId;
                patient.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.Patient>().Update(patient);

            }
            var appointment =
                await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetFirstAsync(x => x.Id == request.Id);

            if (appointment == null)
            {
                return 404;
            }

            // =========================================
            // UPDATE APPOINTMENT
            // =========================================

            appointment.AppointmentDate = request.AppointmentDate;
            appointment.DepartmentId = request.DepartmentId;
            appointment.AppointmentTypeId = request.AppointmentTypeId;
            appointment.PriorityLevelId = request.PriorityLevelId;
            appointment.VisitTypeId = request.VisitTypeId;
            appointment.DoctorId = request.DoctorId;
            appointment.Reason = request.Reason;
            appointment.ConfirmationNotes = request.ConfirmationNotes;
            appointment.ConfirmedDate = request.ConfirmedDate;
            appointment.AppointmentStatusId = request.AppointmentStatusId;
            appointment.ReferrerId = request.ReferrerId;
            appointment.ModifiedById = sessionProvider.Session.LoggedInUserId;
            appointment.ModifiedDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.Appointment>().Update(appointment);

            // =========================================
            // PAYMENTS – REPLACE ALL
            // =========================================

            // Remove existing payments for this appointment
            var existingPayments =
                await unitOfWork.Repository<AppointmentPayment>()
                .FindAllAsync(x => x.AppointmentId == appointment.Id && !x.IsDelete);

            foreach (var oldPayment in existingPayments)
            {
                oldPayment.IsDelete = true;
                oldPayment.IsActive = false;
                oldPayment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                oldPayment.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<AppointmentPayment>().Update(oldPayment);
            }

            // Add new payments from request
            if (request.AppointmentPayment != null && request.AppointmentPayment.Any())
            {
                foreach (var item in request.AppointmentPayment)
                {
                    var payment = new AppointmentPayment
                    {
                        AppointmentId = appointment.Id,
                        VisitFee = item.VisitFee,
                        Discount = item.Discount,
                        TotalPayable = item.TotalPayable,
                        PaymentModeId = item.PaymentModeId,
                        ServiceId = item.ServiceId,          // Make sure frontend sends this
                        PaymentDate = DateTime.Now,
                        PaymentStatusId = item.PaymentStatusId,
                        CreatedById = sessionProvider.Session.LoggedInUserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        IsDelete = false
                    };

                    await unitOfWork.Repository<AppointmentPayment>().AddAsync(payment);
                }
            }

            // =========================================
            // REMOVE OLD LAB ORDERS
            // =========================================

            var oldLabOrders =
                await unitOfWork.Repository<Entities.Models.LabOrder>()
                .FindAllAsync(x => x.AppointmentId == appointment.Id && !x.IsDelete);

            foreach (var old in oldLabOrders)
            {
                old.IsDelete = true;
                old.IsActive = false;
                old.ModifiedById = sessionProvider.Session.LoggedInUserId;
                old.DeleteDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.LabOrder>().Update(old);
            }

            // =========================================
            // ADD NEW LAB ORDERS
            // =========================================

            if (request.LabOrders != null)
            {
                foreach (var item in request.LabOrders)
                {
                    var labOrder = new Entities.Models.LabOrder
                    {
                        AppointmentId = appointment.Id,
                        LabOrderTypeId = item.LabOrderTypeId,
                        ClinicalNotes = item.ClinicalNotes,
                        StatusId = 1,
                        CreatedById = sessionProvider.Session.LoggedInUserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        IsDelete = false
                    };

                    await unitOfWork.Repository<Entities.Models.LabOrder>().AddAsync(labOrder);
                }
            }

            // =========================================
            // REMOVE OLD RADIOLOGY
            // =========================================

            var oldRadiology =
                await unitOfWork.Repository<Entities.Models.RadiologyOrder>()
                .FindAllAsync(x => x.AppointmentId == appointment.Id && !x.IsDelete);

            foreach (var old in oldRadiology)
            {
                old.IsDelete = true;
                old.IsActive = false;
                old.ModifiedById = sessionProvider.Session.LoggedInUserId;
                old.DeleteDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.RadiologyOrder>().Update(old);
            }

            // =========================================
            // ADD NEW RADIOLOGY
            // =========================================

            if (request.RadiologyOrders != null)
            {
                foreach (var item in request.RadiologyOrders)
                {
                    var radiologyOrder = new Entities.Models.RadiologyOrder
                    {
                        AppointmentId = appointment.Id,
                        RadiologyTypeId = item.RadiologyTypeId,
                        ClinicalNotes = item.ClinicalNotes,
                        StatusId = 1,
                        CreatedById = sessionProvider.Session.LoggedInUserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        IsDelete = false
                    };

                    await unitOfWork.Repository<Entities.Models.RadiologyOrder>().AddAsync(radiologyOrder);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return 200;
        }
    }
}