using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
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
            // 1️⃣ If PatientId is null, register a new patient
            if (request.PatientId == null)
            {
                var registerCommand = new Patient
                {
                    Name = request.Patient.Name,
                    Email = request.Patient.Email,
                    PhoneNo = request.Patient.PhoneNo,
                    SecondaryPhoneNo = request.Patient.SecondaryPhoneNo,
                    Address = request.Patient.Address,
                    CNIC = request.Patient.CNIC,
                    Age = request.Patient.Age,
                    Gender = request.Patient.Gender,
                    DateOfBirth = request.Patient.DateOfBirth,
                    CityId = request.Patient.CityId,
                    ProjectId = sessionProvider.Session.SelectedWarehouseId,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                };

                var currentProject = await unitOfWork.Repository<Entities.Models.Project>()
                  .GetOneAsync(u => u.IsActive && u.Id == sessionProvider.Session.SelectedWarehouseId);

                // 4️⃣ Generate MRN (AspNetUsers.Code) like H1-000001
                string prefix = currentProject.Code;
                var lastPatientWithMrn = await unitOfWork.Repository<Patient>()
                    .GetOneAsync(u => !string.IsNullOrEmpty(u.MRN) && u.MRN.StartsWith(prefix),
                                 query => query.OrderByDescending(x => x.MRN));

                int newNumber = 1;
                if (lastPatientWithMrn != null)
                {
                    string numericPart = lastPatientWithMrn.MRN.Substring(prefix.Length); // get the number part
                    if (!int.TryParse(numericPart, out newNumber))
                    {
                        newNumber = 1;
                    }
                    else
                    {
                        newNumber += 1; // increment
                    }
                }

                registerCommand.MRN = prefix + newNumber.ToString().PadLeft(6, '0'); // H1-000001

                // Call your Register handler logic
                var identityResponse = await RegisterNewPatientAsync(registerCommand);

                if (!identityResponse.Succeeded)
                    throw new Exception($"Failed to register patient: {identityResponse.Error}");

                request.PatientId = identityResponse.Id; // set newly created patient ID
            }

            // 2️⃣ Check if appointment exists
            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (appointment == null)
            {
                // Create new appointment
                string newCode = await GenerateAppointmentCodeAsync();

                var newAppointment = mapper.Map<Entities.Models.Appointment>(request);
                newAppointment.TokenNumber = newCode;
                newAppointment.CreatedById = sessionProvider.Session.LoggedInUserId;
                newAppointment.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                newAppointment.CreatedDate = DateTime.Now;
                newAppointment.AppointmentStatusId = 1;  // default status
                unitOfWork.Repository<Entities.Models.Appointment>().Add(newAppointment);
            }
            else
            {
                //// Update existing appointment
                //var updatedAppointment = mapper.Map<Entities.Models.Appointment>(request);
                //updatedAppointment.Id = appointment.Id;
                //updatedAppointment.Code = appointment.Code;
                //updatedAppointment.StatusId = appointment.StatusId;
                //updatedAppointment.InvoiceStatusId = appointment.InvoiceStatusId;
                //updatedAppointment.CreatedById = appointment.CreatedById;
                //updatedAppointment.CreatedDate = appointment.CreatedDate;
                //updatedAppointment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                //updatedAppointment.ModifiedDate = DateTime.Now;

                //unitOfWork.Repository<Entities.Models.Appointment>().Update(updatedAppointment);
                //await unitOfWork.SaveChangesAsync();

            }
            await unitOfWork.SaveChangesAsync();
            return 200;
        }

        // Helper: Generate next appointment code
        private async Task<string> GenerateAppointmentCodeAsync()
        {
            if (await unitOfWork.Repository<Entities.Models.Appointment>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.Appointment>, IOrderedQueryable<Entities.Models.Appointment>> orderByDesc = q => q.OrderByDescending(x => x.TokenNumber);
                var lastAppointment = await unitOfWork.Repository<Entities.Models.Appointment>().GetOneAsync(x => x.IsActive, orderByDesc);
                int nextNumber = int.TryParse(lastAppointment.TokenNumber, out int n) ? n + 1 : 1;
                return nextNumber.ToString().PadLeft(7, '0');
            }
            return "0000001";
        }

        // Helper: Register a new patient
        private async Task<IdentityResponse> RegisterNewPatientAsync(Patient request)
        {
            var result = new IdentityResponse();

            // Check for duplicates
            if (await unitOfWork.Repository<AspNetUsers>().GetExistsAsync(x => x.PhoneNumber == request.PhoneNo))
            {
                result.Error = "Phone Number Duplicate!";
                return result;
            }

            unitOfWork.Repository<Patient>().Add(request);
            SaveChanges();
            result.Id = request.Id;
            return result;
        }
    }
}