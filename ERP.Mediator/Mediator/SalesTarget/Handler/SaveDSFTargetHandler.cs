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
    public class SaveDSFTargetHandler : IRequestHandler<SaveDSFTargetCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveDSFTargetHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
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

        async Task<long> IRequestHandler<SaveDSFTargetCommand, long>.Handle(SaveDSFTargetCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.DSFTargetList)
            {
                var salesTarget = await unitOfWork.Repository<Entities.Models.SalesTarget>().GetFirstAsNoTrackingAsync(x => x.UserId == item.DSFId && x.TargetMonth.Month == request.TargetMonth.Month);
                if (salesTarget == null)
                {
                    var _salesTarget = mapper.Map<Entities.Models.SalesTarget>(item);
                    _salesTarget.UserId = item.DSFId;
                    _salesTarget.TargetMonth = request.TargetMonth;
                    _salesTarget.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _salesTarget.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.SalesTarget>().Add(_salesTarget);
                }
                else
                {
                    var _salesTarget = mapper.Map<Entities.Models.SalesTarget>(item);
                    _salesTarget.UserId = item.DSFId;
                    _salesTarget.TargetMonth = salesTarget.TargetMonth;
                    _salesTarget.CreatedById = salesTarget.CreatedById;
                    _salesTarget.CreatedDate = salesTarget.CreatedDate;
                    _salesTarget.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _salesTarget.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.SalesTarget>().Update(_salesTarget);
                }
            }
            SaveChanges();
            return 200;
        }
    }
}