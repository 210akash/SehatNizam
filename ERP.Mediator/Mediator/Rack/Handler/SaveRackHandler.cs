using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Rack.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Rack.Handler
{
    public class SaveRackHandler : IRequestHandler<SaveRackCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRackHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRackCommand, long>.Handle(SaveRackCommand request, CancellationToken cancellationToken)
        {
            var rack = await unitOfWork.Repository<Entities.Models.Rack>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (rack == null)
            {
                var newRack = mapper.Map<Entities.Models.Rack>(request);
                newRack.CreatedById = sessionProvider.Session.LoggedInUserId;
                newRack.CompanyId = sessionProvider.Session.CompanyId;
                newRack.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Rack>().Add(newRack);
                SaveChanges();
            }
            else
            {
                var updatedRack = mapper.Map<Entities.Models.Rack>(request);
                updatedRack.CreatedById = rack.CreatedById;
                updatedRack.CreatedDate = rack.CreatedDate;
                updatedRack.CompanyId = rack.CompanyId;
                updatedRack.ModifiedById = sessionProvider.Session.LoggedInUserId;
                updatedRack.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Rack>().Update(updatedRack);
                SaveChanges();
            }
            return 200;
        }
    }
}
