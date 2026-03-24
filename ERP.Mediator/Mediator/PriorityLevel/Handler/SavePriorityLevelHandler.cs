using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PriorityLevel.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PriorityLevel.Handler
{
    public class SavePriorityLevelHandler : IRequestHandler<SavePriorityLevelCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SavePriorityLevelHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SavePriorityLevelCommand, long>.Handle(SavePriorityLevelCommand request, CancellationToken cancellationToken)
        {
            var PriorityLevel = await unitOfWork.Repository<Entities.Models.PriorityLevel>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.PriorityLevel>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (PriorityLevel == null)
                {
                    var _PriorityLevel = mapper.Map<Entities.Models.PriorityLevel>(request);
                    _PriorityLevel.CompanyId = sessionProvider.Session.CompanyId;
                    _PriorityLevel.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _PriorityLevel.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.PriorityLevel>().Add(_PriorityLevel);
                    SaveChanges();
                }
                else
                {
                    var _PriorityLevel = mapper.Map<Entities.Models.PriorityLevel>(request);
                    _PriorityLevel.CompanyId = PriorityLevel.CompanyId;
                    _PriorityLevel.CreatedById = PriorityLevel.CreatedById;
                    _PriorityLevel.CreatedDate = PriorityLevel.CreatedDate;
                    _PriorityLevel.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _PriorityLevel.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.PriorityLevel>().Update(_PriorityLevel);
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