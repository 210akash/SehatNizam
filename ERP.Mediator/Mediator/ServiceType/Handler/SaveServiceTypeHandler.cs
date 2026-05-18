using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ServiceType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ServiceType.Handler
{
    public class SaveServiceTypeHandler : IRequestHandler<SaveServiceTypeCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public SaveServiceTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveServiceTypeCommand request, CancellationToken cancellationToken)
        {
            Entities.Models.ServiceType ServiceType;

            if (request.Id > 0)
            {
                // Update existing
                ServiceType = await unitOfWork.Repository<Entities.Models.ServiceType>().FindAsync(y=>y.Id == request.Id);
                if (ServiceType == null)
                {
                    return 404; // Not Found
                }

                mapper.Map(request, ServiceType);
                ServiceType.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
                ServiceType.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.ServiceType>().Update(ServiceType);
            }
            else
            {
                // Create new
                ServiceType = mapper.Map<Entities.Models.ServiceType>(request);
                ServiceType.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                ServiceType.IsActive = true;

                await unitOfWork.Repository<Entities.Models.ServiceType>().AddAsync(ServiceType);
            }

            await unitOfWork.SaveChangesAsync();
            return 200; // Success
        }
    }
}
