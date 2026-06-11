using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.BloodBank.BloodGroup.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.BloodGroup.Handler
{
    public class SaveBloodGroupMasterHandler : IRequestHandler<SaveBloodGroupMasterCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveBloodGroupMasterHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveBloodGroupMasterCommand request, CancellationToken cancellationToken)
        {
            var existing = await unitOfWork.Repository<Entities.Models.BloodGroupMaster>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);

            var duplicate = await unitOfWork.Repository<Entities.Models.BloodGroupMaster>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                    && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (duplicate.Any()) return 409;

            if (existing == null)
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                {
                    request.Code = await GenerateCodeAsync();
                }

                var entity = mapper.Map<Entities.Models.BloodGroupMaster>(request);
                entity.CreatedById = sessionProvider.Session.LoggedInUserId;
                entity.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodGroupMaster>().Add(entity);
            }
            else
            {
                var entity = mapper.Map<Entities.Models.BloodGroupMaster>(request);
                entity.Code = existing.Code;
                entity.CreatedById = existing.CreatedById;
                entity.CreatedDate = existing.CreatedDate;
                entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodGroupMaster>().Update(entity);
            }

            unitOfWork.SaveChanges();
            return 200;
        }

        private async Task<string> GenerateCodeAsync()
        {
            if (!await unitOfWork.Repository<Entities.Models.BloodGroupMaster>()
                .GetExistsAsync(x => x.IsActive == true))
            {
                return "0001";
            }

            Func<IQueryable<Entities.Models.BloodGroupMaster>, IOrderedQueryable<Entities.Models.BloodGroupMaster>> orderByDesc =
                query => query.OrderByDescending(x => x.Id);
            var last = await unitOfWork.Repository<Entities.Models.BloodGroupMaster>()
                .GetOneAsync(x => x.IsActive == true, orderByDesc, null);
            return ((last?.Id ?? 0) + 1).ToString().PadLeft(4, '0');
        }
    }
}
