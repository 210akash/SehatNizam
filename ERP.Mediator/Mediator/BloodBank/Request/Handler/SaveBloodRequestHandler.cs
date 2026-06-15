using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.BloodBank.Request.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Request.Handler
{
    public class SaveBloodRequestHandler : IRequestHandler<SaveBloodRequestCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveBloodRequestHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveBloodRequestCommand request, CancellationToken cancellationToken)
        {
            if (request.Quantity < 1) return 400;

            if (request.AppointmentId.HasValue && request.AppointmentId.Value > 0)
            {
                var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
                    .GetFirstAsNoTrackingAsync(x => x.Id == request.AppointmentId.Value && x.IsActive == true);

                if (appointment == null)
                {
                    return 404;
                }
            }

            var existing = await unitOfWork.Repository<Entities.Models.BloodRequest>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);

            if (existing == null)
            {
                var entity = mapper.Map<Entities.Models.BloodRequest>(request);
                if (string.IsNullOrWhiteSpace(entity.Code))
                {
                    entity.Code = await GenerateRequestCodeAsync();
                }
                entity.Status = (int)BloodRequestStatus.Pending;
                entity.CreatedById = sessionProvider.Session.LoggedInUserId;
                entity.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodRequest>().Add(entity);
            }
            else
            {
                if (existing.Status != (int)BloodRequestStatus.Pending)
                {
                    return 409;
                }

                var entity = mapper.Map<Entities.Models.BloodRequest>(request);
                entity.Code = existing.Code;
                entity.Status = existing.Status;
                entity.CreatedById = existing.CreatedById;
                entity.CreatedDate = existing.CreatedDate;
                entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodRequest>().Update(entity);
            }

            unitOfWork.SaveChanges();
            return 200;
        }

        private async Task<string> GenerateRequestCodeAsync()
        {
            var requests = await unitOfWork.Repository<Entities.Models.BloodRequest>()
                .GetAsync(x => x.IsActive == true && x.IsDelete == false);

            var maxNumber = requests
                .Select(r => ParseSequentialCodeNumber(r.Code))
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
    }
}
