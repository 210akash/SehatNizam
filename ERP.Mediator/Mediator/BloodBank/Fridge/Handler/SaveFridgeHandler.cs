using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.BloodBank.Fridge.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Fridge.Handler
{
    public class SaveFridgeHandler : IRequestHandler<SaveFridgeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveFridgeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveFridgeCommand request, CancellationToken cancellationToken)
        {
            var existing = await unitOfWork.Repository<Entities.Models.BloodFridge>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);

            var duplicate = await unitOfWork.Repository<Entities.Models.BloodFridge>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                    && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (duplicate.Any()) return 409;

            if (existing == null)
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                {
                    request.Code = await GenerateCodeAsync();
                }

                var entity = mapper.Map<Entities.Models.BloodFridge>(request);
                entity.CreatedById = sessionProvider.Session.LoggedInUserId;
                entity.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodFridge>().Add(entity);
            }
            else
            {
                var entity = mapper.Map<Entities.Models.BloodFridge>(request);
                entity.Code = existing.Code;
                entity.CreatedById = existing.CreatedById;
                entity.CreatedDate = existing.CreatedDate;
                entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodFridge>().Update(entity);
            }

            unitOfWork.SaveChanges();
            return 200;
        }

        private async Task<string> GenerateCodeAsync()
        {
            if (!await unitOfWork.Repository<Entities.Models.BloodFridge>()
                .GetExistsAsync(x => x.IsActive == true))
            {
                return "0001";
            }

            Func<IQueryable<Entities.Models.BloodFridge>, IOrderedQueryable<Entities.Models.BloodFridge>> orderByDesc =
                query => query.OrderByDescending(x => x.Id);
            var last = await unitOfWork.Repository<Entities.Models.BloodFridge>()
                .GetOneAsync(x => x.IsActive == true, orderByDesc, null);
            return ((last?.Id ?? 0) + 1).ToString().PadLeft(4, '0');
        }
    }
}
