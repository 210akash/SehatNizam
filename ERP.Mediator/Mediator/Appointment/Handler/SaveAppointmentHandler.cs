using AutoMapper;
using AutoMapper.QueryableExtensions;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class SaveAppointmentHandler : IRequestHandler<SaveAppointmentCommand, Tuple<long,long?>>
    {
        private const long FamilyMemberAppointmentTypeId = 2;

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

        public async Task<Tuple<long, long?>> Handle(SaveAppointmentCommand request, CancellationToken cancellationToken)
        {
            using var transaction =
                await unitOfWork.BeginTransactionAsync();

            try
            {
                Tuple<long, long?> result;

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
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return new Tuple<long, long?>(504, null);
                throw;
            }
        }

        private async Task<Tuple<long,long?>> CreateAppointmentAsync(SaveAppointmentCommand request, CancellationToken cancellationToken)
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
                string tokenNumber;
                if (request.DoctorId.HasValue)
                {
                    tokenNumber = await GenerateAppointmentCodeAsync(request.DoctorId.Value, request.AppointmentDate);
                }
                else
                {
                    tokenNumber = await GenerateDepartmentAppointmentCodeAsync(request.DepartmentId);
                }

                var appointment = new Entities.Models.Appointment
                {
                    AppointmentDate = request.AppointmentDate,
                    TokenNumber = tokenNumber,
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
                return new Tuple<long, long?>(200, appointment.Id);

            }
            catch
            {
                return new Tuple<long, long?>(200, null);
                throw;
            }
        }

        private async Task<long> GetOrCreatePatientAsync(SaveAppointmentCommand request)
        {
            var projectId = sessionProvider.Session.SelectedWarehouseId;

            if (request.Patient == null)
                throw new Exception("Patient information is required.");

            var phoneNo = request.Patient.PhoneNo?.Trim();
            var name = request.Patient.Name?.Trim();

            if (string.IsNullOrWhiteSpace(phoneNo) || string.IsNullOrWhiteSpace(name))
                throw new Exception("Patient name and phone number are required.");

            var isFamilyMemberAppointment = request.AppointmentTypeId == FamilyMemberAppointmentTypeId;
            PatientMaster masterForProject = null;

            Entities.Models.Patient selectedPatient = null;
            if (request.PatientId.HasValue && request.PatientId.Value > 0)
            {
                selectedPatient = await unitOfWork.Repository<Entities.Models.Patient>()
                    .GetOneAsync(
                        x => x.Id == request.PatientId.Value && x.IsActive && !x.IsDelete,
                        includeProperties: "PatientMaster");
            }

            // Self: reuse/update same master (any source project), then ensure Patient row for current project
            if (!isFamilyMemberAppointment)
            {
                if (selectedPatient == null)
                    selectedPatient = await ResolveSingleSelfPatientByPhoneAsync(phoneNo, projectId);

                if (selectedPatient == null)
                    selectedPatient = await ResolveSingleSelfPatientByPhoneAsync(phoneNo, projectId: null);

                var selectedMaster = await ResolvePatientMasterAsync(selectedPatient);

                if (selectedMaster != null)
                {
                    if (HasPatientMasterDemographicChanges(request.Patient, selectedMaster))
                    {
                        UpdatePatientMasterFromCommand(request.Patient, selectedMaster);
                        unitOfWork.Repository<PatientMaster>().Update(selectedMaster);
                        await unitOfWork.SaveChangesAsync();
                    }

                    masterForProject = selectedMaster;
                }
            }

            // Family or self with no linked master → find or create master by full profile
            if (masterForProject == null)
            {
                masterForProject = await FindMatchingPatientMasterAsync(request.Patient);

                if (masterForProject == null)
                    masterForProject = await CreatePatientMasterAsync(request.Patient, phoneNo, name);
            }

            // One master can have one Patient row per project — create only if missing for selected project
            return await EnsurePatientForProjectAsync(
                masterForProject.Id,
                projectId,
                request.AppointmentStatusId);
        }

        private async Task<long> EnsurePatientForProjectAsync(
            long patientMasterId,
            long projectId,
            long appointmentStatusId)
        {
            var existingPatient = await unitOfWork.Repository<Entities.Models.Patient>().GetOneAsync(
                x => x.IsActive && !x.IsDelete
                     && x.PatientMasterId == patientMasterId
                     && x.ProjectId == projectId);

            if (existingPatient != null)
                return existingPatient.Id;

            var mrn = appointmentStatusId != 1
                ? await GenerateMrnAsync()
                : string.Empty;

            var patient = new Entities.Models.Patient
            {
                PatientMasterId = patientMasterId,
                ProjectId = projectId,
                MRN = mrn,
                CreatedById = sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDelete = false
            };

            await unitOfWork.Repository<Entities.Models.Patient>().AddAsync(patient);
            await unitOfWork.SaveChangesAsync();
            return patient.Id;
        }

        private async Task<Entities.Models.Patient> ResolveSingleSelfPatientByPhoneAsync(string phoneNo, long? projectId)
        {
            var patients = (await unitOfWork.Repository<Entities.Models.Patient>().GetAsync(
                x => x.IsActive && !x.IsDelete
                     && x.PatientMaster != null
                     && x.PatientMaster.PhoneNo == phoneNo
                     && (!projectId.HasValue || x.ProjectId == projectId.Value),
                includeProperties: "PatientMaster"))?.ToList();

            if (patients == null || patients.Count != 1)
                return null;

            return patients[0];
        }

        private async Task<PatientMaster> ResolvePatientMasterAsync(Entities.Models.Patient patient)
        {
            if (patient == null)
                return null;

            if (patient.PatientMaster != null)
                return patient.PatientMaster;

            if (patient.PatientMasterId <= 0)
                return null;

            return await unitOfWork.Repository<PatientMaster>()
                .GetOneAsync(x => x.Id == patient.PatientMasterId && x.IsActive && !x.IsDelete);
        }

        private async Task<PatientMaster> FindMatchingPatientMasterAsync(PatientCommand patient)
        {
            var phoneNo = patient.PhoneNo?.Trim();
            var normalizedName = patient.Name?.Trim().ToLower();

            var candidates = await unitOfWork.Repository<PatientMaster>().GetAsync(
                x => x.IsActive && !x.IsDelete
                     && x.PhoneNo == phoneNo
                     && x.Name.ToLower() == normalizedName);

            return candidates?.FirstOrDefault(m => PatientMasterMatches(patient, m));
        }

        private async Task<PatientMaster> CreatePatientMasterAsync(PatientCommand patient, string phoneNo, string name)
        {
            var patientMaster = new PatientMaster
            {
                Name = name,
                Email = patient.Email,
                PhoneNo = phoneNo,
                SecondaryPhoneNo = patient.SecondaryPhoneNo,
                Address = patient.Address,
                CNIC = patient.CNIC,
                Gender = patient.Gender,
                Age = patient.Age,
                DateOfBirth = patient.DateOfBirth,
                CityId = patient.CityId,
                CreatedById = sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDelete = false
            };

            await unitOfWork.Repository<PatientMaster>().AddAsync(patientMaster);
            await unitOfWork.SaveChangesAsync();
            return patientMaster;
        }

        private void UpdatePatientMasterFromCommand(PatientCommand patient, PatientMaster master)
        {
            master.Name = patient.Name?.Trim();
            master.Email = patient.Email;
            master.PhoneNo = patient.PhoneNo?.Trim();
            master.SecondaryPhoneNo = patient.SecondaryPhoneNo;
            master.Address = patient.Address;
            master.CNIC = patient.CNIC;
            master.Gender = patient.Gender;
            master.Age = patient.Age;
            master.DateOfBirth = patient.DateOfBirth;
            master.CityId = patient.CityId;
            master.ModifiedById = sessionProvider.Session.LoggedInUserId;
            master.ModifiedDate = DateTime.Now;
        }

        private static bool PatientMasterMatches(PatientCommand request, PatientMaster master)
        {
            return request != null && master != null && !HasPatientMasterDemographicChanges(request, master);
        }

        private static bool HasPatientMasterDemographicChanges(PatientCommand request, PatientMaster master)
        {
            if (request == null || master == null)
                return true;

            return !string.Equals(request.Name?.Trim(), master.Name?.Trim(), StringComparison.OrdinalIgnoreCase)
                || !string.Equals(NormalizeValue(request.Gender), NormalizeValue(master.Gender), StringComparison.OrdinalIgnoreCase)
                || !DatesEqual(request.DateOfBirth, master.DateOfBirth)
                || request.Age != master.Age
                || !string.Equals(NormalizeCnic(request.CNIC), NormalizeCnic(master.CNIC), StringComparison.OrdinalIgnoreCase)
                || !string.Equals(request.SecondaryPhoneNo?.Trim(), master.SecondaryPhoneNo?.Trim(), StringComparison.OrdinalIgnoreCase)
                || request.CityId != (master.CityId ?? 0);
        }

        private static string NormalizeValue(string value) => value?.Trim() ?? string.Empty;

        private static string NormalizeCnic(string cnic) =>
            string.IsNullOrWhiteSpace(cnic)
                ? string.Empty
                : cnic.Replace("-", string.Empty).Trim();

        private static bool DatesEqual(DateTime? left, DateTime? right)
        {
            if (!left.HasValue && !right.HasValue)
                return true;

            if (!left.HasValue || !right.HasValue)
                return false;

            return left.Value.Date == right.Value.Date;
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

        private async Task<string> GenerateAppointmentCodeAsync()
        {
            Func<IQueryable<Entities.Models.Appointment>,
                IOrderedQueryable<Entities.Models.Appointment>> orderBy =
                    q => q.OrderByDescending(x => x.Id);

            var lastAppointment =
                await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetOneAsync(x => x.IsActive && x.ProjectId == sessionProvider.Session.SelectedWarehouseId, orderBy);

            int nextNumber = 1;

            if (lastAppointment != null &&
                !string.IsNullOrWhiteSpace(lastAppointment.TokenNumber))
            {
                int.TryParse(lastAppointment.TokenNumber, out nextNumber);

                nextNumber++;
            }

            return nextNumber.ToString("D7");
        }

        private async Task<Tuple<long, long?>> UpdateAppointmentAsync(SaveAppointmentCommand request, CancellationToken cancellationToken)
        {
            if (request.AppointmentStatusId == 1)
            {
                var patient =
                   await unitOfWork.Repository<Entities.Models.Patient>()
                   .GetFirstAsync(x => x.Id == request.PatientId);

                if (patient == null)
                {
                    return new Tuple<long, long?>(404, null);
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
                return new Tuple<long, long?>(404, null);
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
            return new Tuple<long, long?>(200, appointment.Id);
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

        private async Task<string> GenerateDepartmentAppointmentCodeAsync(long departmentId)
        {
            var projectId = sessionProvider.Session.SelectedWarehouseId;

            Func<IQueryable<Entities.Models.Appointment>,
                IOrderedQueryable<Entities.Models.Appointment>> orderBy =
                    q => q.OrderByDescending(x => x.Id);

            var lastAppointment =
                await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetOneAsync(
                    x => x.IsActive
                         && x.DepartmentId == departmentId
                         && x.ProjectId == projectId
                         && !string.IsNullOrEmpty(x.TokenNumber),
                    orderBy);

            int nextNumber = 1;

            if (lastAppointment != null &&
                int.TryParse(lastAppointment.TokenNumber, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }

            return nextNumber.ToString("D7");
        }
    }
}