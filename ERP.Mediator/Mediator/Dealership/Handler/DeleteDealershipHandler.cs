using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Dealership.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Handler
{
    public class DeleteDealershipHandler : IRequestHandler<DeleteDealershipQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteDealershipHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteDealershipQuery request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.Repository<Entities.Models.Dealership>().GetExistsAsync(y => y.Id == request.Id))
            {
                var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                dealership.IsDelete = true;
                dealership.IsActive = false;
                dealership.ModifiedDate = DateTime.Now;
                dealership.DeleteDate = DateTime.Now;
                dealership.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Dealership>().Update(dealership);
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
