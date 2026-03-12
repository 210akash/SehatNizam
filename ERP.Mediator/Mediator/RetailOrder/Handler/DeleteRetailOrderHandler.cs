using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RetailOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrder.Handler
{
    public class DeleteRetailOrderHandler : IRequestHandler<DeleteRetailOrderQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRetailOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteRetailOrderQuery request, CancellationToken cancellationToken)
        {
            var RetailOrder = await unitOfWork.Repository<Entities.Models.RetailOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            RetailOrder.IsDelete = true;
            RetailOrder.IsActive = false;
            RetailOrder.ModifiedDate = DateTime.Now;
            RetailOrder.DeleteDate = DateTime.Now;
            RetailOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.RetailOrder>().Update(RetailOrder);
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
    }
}
