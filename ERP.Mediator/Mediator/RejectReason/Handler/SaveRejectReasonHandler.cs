using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RejectReason.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RejectReason.Handler
{
    public class SaveRejectReasonHandler : IRequestHandler<SaveRejectReasonCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRejectReasonHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRejectReasonCommand, long>.Handle(SaveRejectReasonCommand request, CancellationToken cancellationToken)
        {
            var RejectReason = await unitOfWork.Repository<Entities.Models.RejectReason>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.RejectReason>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (RejectReason == null)
                {
                    var _RejectReason = mapper.Map<Entities.Models.RejectReason>(request);
                    _RejectReason.CompanyId = sessionProvider.Session.CompanyId;
                    _RejectReason.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _RejectReason.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.RejectReason>().Add(_RejectReason);
                    SaveChanges();
                }
                else
                {
                    var _RejectReason = mapper.Map<Entities.Models.RejectReason>(request);
                    _RejectReason.CompanyId = RejectReason.CompanyId;
                    _RejectReason.CreatedById = RejectReason.CreatedById;
                    _RejectReason.CreatedDate = RejectReason.CreatedDate;
                    _RejectReason.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _RejectReason.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.RejectReason>().Update(_RejectReason);
                    SaveChanges();
                }
                return 200;

            }
            else
            {
                return 409;
            }

        }
    }
}