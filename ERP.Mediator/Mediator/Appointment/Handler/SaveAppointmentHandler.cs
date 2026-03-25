using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Command;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
        private readonly IMediator mediator;
        private readonly UserManager<AspNetUsers> userManager;
        public SaveAppointmentHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, UserManager<AspNetUsers> userManager)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
            this.userManager = userManager;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveAppointmentCommand request, CancellationToken cancellationToken)
        {
            // 1️⃣ If PatientId is null, register a new patient
            if (request.PatientId == Guid.Empty || request.PatientId == null)
            {
                // If email is empty, create a default one using FirstName
                if (string.IsNullOrWhiteSpace(request.Patient.Email))
                {
                    request.Patient.Email = $"{request.Patient.FirstName.Replace(" ", "").ToLower()}@hms.com";
                }

                var registerCommand = new RegisterCommand
                {
                    FirstName = request.Patient.FirstName,
                    Email = request.Patient.Email,
                    Password = "Hms@123456",
                    PhoneNumber = request.Patient.PhoneNumber,
                    Gender = request.Patient.Gender,
                    DateOfBirth = request.Patient.DateOfBirth,
                    IsEmployee = request.Patient.IsEmployee
                };

                // 4️⃣ Generate MRN (AspNetUsers.Code) like H1-000001
                string prefix = "H1-";
                var lastPatientWithMrn = await unitOfWork.Repository<AspNetUsers>()
                    .GetOneAsync(u => !string.IsNullOrEmpty(u.Code) && u.Code.StartsWith(prefix),
                                 query => query.OrderByDescending(x => x.Code));

                int newNumber = 1;
                if (lastPatientWithMrn != null)
                {
                    string numericPart = lastPatientWithMrn.Code.Substring(prefix.Length); // get the number part
                    if (!int.TryParse(numericPart, out newNumber))
                    {
                        newNumber = 1;
                    }
                    else
                    {
                        newNumber += 1; // increment
                    }
                }

                registerCommand.Code = prefix + newNumber.ToString().PadLeft(6, '0'); // H1-000001

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
        private async Task<IdentityResponse> RegisterNewPatientAsync(RegisterCommand request)
        {
            var result = new IdentityResponse();

            // Check for duplicates
            if (await unitOfWork.Repository<AspNetUsers>().GetExistsAsync(x => x.PhoneNumber == request.PhoneNumber))
            {
                result.Error = "Phone Number Duplicate!";
                return result;
            }

            if (await unitOfWork.Repository<AspNetUsers>().GetExistsAsync(x => x.Email.ToLower() == request.Email.ToLower()))
            {
                result.Error = "Email Duplicate!";
                return result;
            }

            var user = mapper.Map<AspNetUsers>(request);
            user.Id = Guid.NewGuid();
            user.IsActive = true;
            user.IsDelete = false;
            user.CreatedById = sessionProvider.Session.LoggedInUserId;
            user.CreatedDate = DateTime.Now;
            user.PhoneNumberConfirmed = true;
            user.EmailConfirmed = true;
            user.UserName = request.Email.ToLower();
            user.NormalizedUserName = request.Email.ToUpper();
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            // Generate user code
            string prefix = "";
            var company = await unitOfWork.Repository<Entities.Models.Company>().GetFirstAsync(c => c.Id == sessionProvider.Session.CompanyId);
            if (company != null) prefix = company.Code;

            Func<IQueryable<AspNetUsers>, IOrderedQueryable<AspNetUsers>> orderByDesc = q => q.OrderByDescending(x => x.Code);
            var latestUser = await unitOfWork.Repository<AspNetUsers>()
                .GetOneAsync(u => u.IsActive && u.Code.StartsWith(prefix), orderByDesc);

            string code = "00001";
            if (latestUser != null)
            {
                string numericPart = latestUser.Code.Substring(prefix.Length);
                int latestNumber = int.TryParse(numericPart, out int num) ? num : 0;
                code = (latestNumber + 1).ToString().PadLeft(5, '0');
            }

            user.Code = prefix + code;

            var savedUser = await userManager.CreateAsync(user, request.Password);
            result = mapper.Map<IdentityResponse>(savedUser);
            if (result.Succeeded)
            {
                result.Id = user.Id;

                var userRole = new AspNetUserRoles()
                {
                    RoleId = new Guid(""),
                    UserId = user.Id
                };

                await SaveAspNetUserRolesAsync(userRole);
            }

            return result;
        }

        /// <summary>
        /// Saves the ASP net user roles.
        /// </summary>
        /// <param name="model">The Asp Net User Roles model.</param>
        /// <returns>the task</returns>
        private async Task SaveAspNetUserRolesAsync(AspNetUserRoles model)
        {
            await unitOfWork.Repository<AspNetUserRoles>().AddAsync(model);
            await unitOfWork.SaveChangesAsync();
        }
    }
}