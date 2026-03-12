using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Rack.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Rack.Handler
{
    public class DeleteRackHandler : IRequestHandler<DeleteRackQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRackHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteRackQuery request, CancellationToken cancellationToken)
        {
            if (!await unitOfWork.Repository<Entities.Models.Zone>().GetExistsAsync(y => y.Id == request.Id && y.IsActive))
            {
                var Rack = await unitOfWork.Repository<Entities.Models.Rack>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                Rack.IsDelete = true;
                Rack.IsActive = false;
                Rack.ModifiedDate = DateTime.Now;
                Rack.DeleteDate = DateTime.Now;
                Rack.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Rack>().Update(Rack);
                var check = await unitOfWork.SaveChangesAsync();
                if (check > 0)
                {
                    return (long)ResponseStatus.OK;
                }
                else
                {
                    return (long)ResponseStatus.Error;
                }
            }
            else
                return (long)ResponseStatus.Conflict;
        }
    }
}
