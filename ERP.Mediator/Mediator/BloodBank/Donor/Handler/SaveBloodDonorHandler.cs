using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.BloodBank.Donor.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Donor.Handler
{
    public class SaveBloodDonorHandler : IRequestHandler<SaveBloodDonorCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveBloodDonorHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveBloodDonorCommand request, CancellationToken cancellationToken)
        {
            var existing = await unitOfWork.Repository<Entities.Models.BloodDonor>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);

            var duplicateCnic = await unitOfWork.Repository<Entities.Models.BloodDonor>()
                .GetAsync(x => x.CNIC.ToLower() == request.CNIC.ToLower().Trim()
                    && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (duplicateCnic.Any()) return 409;

            if (existing == null)
            {
                if (string.IsNullOrWhiteSpace(request.DonorCode))
                {
                    request.DonorCode = await GenerateDonorCodeAsync();
                }

                var entity = mapper.Map<Entities.Models.BloodDonor>(request);
                entity.CreatedById = sessionProvider.Session.LoggedInUserId;
                entity.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodDonor>().Add(entity);
            }
            else
            {
                var entity = mapper.Map<Entities.Models.BloodDonor>(request);
                entity.DonorCode = existing.DonorCode;
                entity.CreatedById = existing.CreatedById;
                entity.CreatedDate = existing.CreatedDate;
                entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodDonor>().Update(entity);
            }

            unitOfWork.SaveChanges();
            return 200;
        }

        private async Task<string> GenerateDonorCodeAsync()
        {
            var donors = await unitOfWork.Repository<Entities.Models.BloodDonor>()
                .GetAsync(x => x.IsActive == true && x.IsDelete == false);

            var maxNumber = donors
                .Select(d => ParseDonorCodeNumber(d.DonorCode))
                .DefaultIfEmpty(0)
                .Max();

            return (maxNumber + 1).ToString().PadLeft(4, '0');
        }

        private static int ParseDonorCodeNumber(string donorCode)
        {
            if (string.IsNullOrWhiteSpace(donorCode)) return 0;

            var digits = new string(donorCode.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out var number) ? number : 0;
        }
    }
}
