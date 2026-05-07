using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Service.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Service.Handler
{
    public class SaveServiceHandler : IRequestHandler<SaveServiceCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public SaveServiceHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveServiceCommand request, CancellationToken cancellationToken)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Code))
            {
                return 400; // Bad Request
            }

            if (request.BasePrice < 0)
            {
                return 400; // Bad Request
            }

            Entities.Models.Service service;

            if (request.Id > 0)
            {
                // Update existing
                service = await unitOfWork.Repository<Entities.Models.Service>().GetByIdAsync(request.Id);
                if (service == null)
                {
                    return 404; // Not Found
                }

                mapper.Map(request, service);
                service.UpdatedById = this.sessionProvider.Session.LoggedInUserId;
                service.UpdatedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.Service>().Update(service);
            }
            else
            {
                // Check for duplicate code
                var exists = await unitOfWork.Repository<Entities.Models.Service>()
                    .AnyAsync(x => x.Code.ToLower() == request.Code.ToLower() 
                        && x.IsActive 
                        && !x.IsDelete
                        && x.CompanyId == this.sessionProvider.Session.CompanyId);

                if (exists)
                {
                    return 409; // Conflict - Code already exists
                }

                // Create new
                service = mapper.Map<Entities.Models.Service>(request);
                service.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                service.CompanyId = this.sessionProvider.Session.CompanyId;
                service.IsActive = true;

                await unitOfWork.Repository<Entities.Models.Service>().AddAsync(service);
            }

            await unitOfWork.CompleteAsync();
            return 200; // Success
        }
    }
}
