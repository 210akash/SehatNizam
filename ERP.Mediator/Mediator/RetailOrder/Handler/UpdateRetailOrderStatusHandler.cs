using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RetailOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrder.Handler
{
    public class UpdateRetailOrderStatusHandler : IRequestHandler<UpdateRetailOrderStatusQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public UpdateRetailOrderStatusHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(UpdateRetailOrderStatusQuery request, CancellationToken cancellationToken)
        {
            var RetailOrder = await unitOfWork.Repository<Entities.Models.RetailOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.RetailOrderId);
            RetailOrder.RetailOrderStatusId = request.ToStatusId;
            RetailOrder.ModifiedDate = DateTime.Now;
            RetailOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.RetailOrder>().Update(RetailOrder);

            RetailOrderProcess process = new RetailOrderProcess();
            process.RetailOrderId = request.RetailOrderId;
            process.FromStatusId = request.FromStatusId;
            process.ToStatusId = request.ToStatusId;
            process.Comments = request.Comments;
            process.CreatedById = sessionProvider.Session.LoggedInUserId;
            process.CreatedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.RetailOrderProcess>().Add(process);

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
