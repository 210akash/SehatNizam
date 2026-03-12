using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SalesTarget.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.SalesTarget.Handler
{
    public class SaveSalesTargetHandler : IRequestHandler<SaveSalesTargetCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveSalesTargetHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveSalesTargetCommand, long>.Handle(SaveSalesTargetCommand request, CancellationToken cancellationToken)
        {
            var IsSalesTargetExist = await unitOfWork.Repository<Entities.Models.SalesTarget>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && (x.Id <= 0 || x.Id != request.Id)
            && x.TargetMonth.Month == request.TargetMonth.Month && x.TargetMonth.Year == request.TargetMonth.Year
            //&& x.TerritoryId == request.TerritoryId && x.TargetMonth.Month == request.TargetMonth.Month && x.TargetMonth.Year == request.TargetMonth.Year
            //&& x.DSFId == request.DSFId && x.TargetMonth.Month == request.TargetMonth.Month && x.TargetMonth.Year == request.TargetMonth.Year
            );
            
            if (IsSalesTargetExist != null && IsSalesTargetExist.Id > 0)
            {
                return 409;
            }
            var salesTarget = await unitOfWork.Repository<Entities.Models.SalesTarget>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (salesTarget == null)
            {
                var _salesTarget = mapper.Map<Entities.Models.SalesTarget>(request);
                _salesTarget.CreatedById = sessionProvider.Session.LoggedInUserId;
                _salesTarget.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.SalesTarget>().Add(_salesTarget);
                SaveChanges();
                SaveChanges();
            }
            else
            {

                var _salesTarget = mapper.Map<Entities.Models.SalesTarget>(request);
                _salesTarget.CreatedById = salesTarget.CreatedById;
                _salesTarget.CreatedDate = salesTarget.CreatedDate;
                _salesTarget.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _salesTarget.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.SalesTarget>().Update(_salesTarget);
                SaveChanges();
            }
            return 200;
        }
    }
}