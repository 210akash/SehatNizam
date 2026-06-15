using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.BloodBank.Donation.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Donation.Handler
{
    public class SaveBloodDonationHandler : IRequestHandler<SaveBloodDonationCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveBloodDonationHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveBloodDonationCommand request, CancellationToken cancellationToken)
        {
            if (request.AppointmentId.HasValue && request.AppointmentId.Value > 0)
            {
                var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
                    .GetFirstAsNoTrackingAsync(x => x.Id == request.AppointmentId.Value && x.IsActive == true);

                if (appointment == null)
                {
                    return 404;
                }

                var duplicateDonation = await unitOfWork.Repository<BloodDonation>()
                    .GetFirstAsNoTrackingAsync(x => x.IsActive == true
                        && x.IsDelete == false
                        && x.AppointmentId == request.AppointmentId.Value
                        && x.BloodDonorId == request.BloodDonorId
                        && x.Id != request.Id);

                if (duplicateDonation != null)
                {
                    return 410;
                }
            }

            var existing = await unitOfWork.Repository<BloodDonation>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);

            BloodDonation entity;

            if (existing == null)
            {
                entity = mapper.Map<BloodDonation>(request);
                if (string.IsNullOrWhiteSpace(entity.DonationCode))
                {
                    entity.DonationCode = await GenerateDonationCodeAsync();
                }
                entity.CreatedById = sessionProvider.Session.LoggedInUserId;
                entity.CreatedDate = DateTime.Now;
                unitOfWork.Repository<BloodDonation>().Add(entity);
                unitOfWork.SaveChanges();
            }
            else
            {
                var linkedUnit = await unitOfWork.Repository<Entities.Models.BloodUnit>()
                    .GetFirstAsNoTrackingAsync(x => x.BloodDonationId == existing.Id && x.IsActive == true);

                if (linkedUnit != null
                    && linkedUnit.BloodFridgeId.HasValue
                    && linkedUnit.BloodRackId.HasValue
                    && request.ScreeningStatus != existing.ScreeningStatus)
                {
                    return 409;
                }

                entity = mapper.Map<BloodDonation>(request);
                entity.DonationCode = existing.DonationCode;
                entity.CreatedById = existing.CreatedById;
                entity.CreatedDate = existing.CreatedDate;
                entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<BloodDonation>().Update(entity);
                unitOfWork.SaveChanges();
            }

            if (request.ScreeningStatus == (int)BloodScreeningStatus.Pass)
            {
                await CreateBloodUnitIfNeededAsync(entity, request);
                await UpdateDonorLastDonationDateAsync(request.BloodDonorId, request.DonationDate);
            }

            unitOfWork.SaveChanges();
            return 200;
        }

        private async Task<string> GenerateDonationCodeAsync()
        {
            var donations = await unitOfWork.Repository<BloodDonation>()
                .GetAsync(x => x.IsActive == true && x.IsDelete == false);

            var maxNumber = donations
                .Select(d => ParseSequentialCodeNumber(d.DonationCode))
                .DefaultIfEmpty(0)
                .Max();

            return (maxNumber + 1).ToString().PadLeft(4, '0');
        }

        private static int ParseSequentialCodeNumber(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return 0;

            if (code.Length == 4 && int.TryParse(code, out var simpleNumber))
            {
                return simpleNumber;
            }

            var lastDash = code.LastIndexOf('-');
            if (lastDash >= 0 && lastDash < code.Length - 1)
            {
                var suffix = code.Substring(lastDash + 1);
                if (int.TryParse(suffix, out var suffixNumber))
                {
                    return suffixNumber;
                }
            }

            var digits = new string(code.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out var number) ? number : 0;
        }

        private async Task CreateBloodUnitIfNeededAsync(BloodDonation entity, SaveBloodDonationCommand request)
        {
            var existingUnit = await unitOfWork.Repository<Entities.Models.BloodUnit>()
                .GetFirstAsNoTrackingAsync(x => x.BloodDonationId == entity.Id && x.IsActive == true);

            if (existingUnit != null) return;

            var componentType = await unitOfWork.Repository<BloodComponentType>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.BloodComponentTypeId && x.IsActive == true);

            if (componentType == null) return;

            long bloodGroupId = request.BloodGroupMasterId ?? 0;
            if (bloodGroupId == 0)
            {
                var donor = await unitOfWork.Repository<BloodDonor>()
                    .GetFirstAsNoTrackingAsync(x => x.Id == request.BloodDonorId);
                bloodGroupId = donor?.BloodGroupMasterId ?? 0;
            }

            var unit = new Entities.Models.BloodUnit
            {
                BloodDonationId = entity.Id,
                BloodComponentTypeId = request.BloodComponentTypeId,
                BloodGroupMasterId = bloodGroupId,
                Volume = request.Volume,
                CollectionDate = request.DonationDate,
                ExpiryDate = request.DonationDate.AddDays(componentType.ShelfLifeDays),
                Status = (int)BloodUnitStatus.Available,
                CreatedById = sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now
            };

            unitOfWork.Repository<Entities.Models.BloodUnit>().Add(unit);
            unitOfWork.SaveChanges();

            unit.UnitNo = await GenerateUnitNoAsync();
            unitOfWork.Repository<Entities.Models.BloodUnit>().Update(unit);
        }

        private async Task<string> GenerateUnitNoAsync()
        {
            var units = await unitOfWork.Repository<Entities.Models.BloodUnit>()
                .GetAsync(x => x.IsActive == true && x.IsDelete == false);

            var maxNumber = units
                .Select(u => ParseSequentialCodeNumber(u.UnitNo))
                .DefaultIfEmpty(0)
                .Max();

            return (maxNumber + 1).ToString().PadLeft(4, '0');
        }

        private async Task UpdateDonorLastDonationDateAsync(long donorId, DateTime donationDate)
        {
            var donor = await unitOfWork.Repository<BloodDonor>()
                .GetFirstAsNoTrackingAsync(x => x.Id == donorId && x.IsActive == true);

            if (donor == null) return;

            donor.LastDonationDate = donationDate;
            donor.ModifiedById = sessionProvider.Session.LoggedInUserId;
            donor.ModifiedDate = DateTime.Now;
            unitOfWork.Repository<BloodDonor>().Update(donor);
        }
    }
}
