using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Priority.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Priority.Handler
{
    public class SavePriorityHandler : IRequestHandler<SavePriorityCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SavePriorityHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SavePriorityCommand, long>.Handle(SavePriorityCommand request, CancellationToken cancellationToken)
        {
            var Priority = await unitOfWork.Repository<Entities.Models.Priority>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Priority>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Priority == null)
                {
                    var _Priority = mapper.Map<Entities.Models.Priority>(request);
                    _Priority.CompanyId = sessionProvider.Session.CompanyId;
                    _Priority.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Priority.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Priority>().Add(_Priority);
                    SaveChanges();
                }
                else
                {
                    var _Priority = mapper.Map<Entities.Models.Priority>(request);
                    _Priority.CompanyId = Priority.CompanyId;
                    _Priority.CreatedById = Priority.CreatedById;
                    _Priority.CreatedDate = Priority.CreatedDate;
                    _Priority.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Priority.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Priority>().Update(_Priority);
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