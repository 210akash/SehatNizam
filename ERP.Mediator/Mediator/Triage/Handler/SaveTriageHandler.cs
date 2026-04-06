using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Triage.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Triage.Handler
{
    public class SaveTriageHandler : IRequestHandler<SaveTriageCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public SaveTriageHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveTriageCommand request, CancellationToken cancellationToken)
        {
            var Triage = await unitOfWork.Repository<Entities.Models.Triage>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (Triage == null)
            {
                var _Triage = mapper.Map<Entities.Models.Triage>(request);
                _Triage.CreatedById = sessionProvider.Session.LoggedInUserId;
                _Triage.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Triage>().Add(_Triage);
                SaveChanges();
            }
            else
            {
                var _Triage = mapper.Map<Entities.Models.Triage>(request);
                _Triage.CreatedById = Triage.CreatedById;
                _Triage.CreatedDate = Triage.CreatedDate;
                _Triage.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _Triage.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Triage>().Update(_Triage);
                SaveChanges();
            }

            return 200; // Success code for adding/updating
        }
    }
}