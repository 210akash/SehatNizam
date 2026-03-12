using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class ProcessDispatchHandler : IRequestHandler<ProcessDispatchQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessDispatchHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessDispatchQuery request, CancellationToken cancellationToken)
        {
            int check = 0;
            var dispatch = await unitOfWork.Repository<Entities.Models.Dispatch>()
               .GetFirstAsync(                            //  <--  tracked query
                       d => d.Id == request.Id,
                       includeProperties:
                       "DispatchOrder," +
                       "DispatchOrder.DispatchDetail");   // you don’t need OrderItem here

            if (dispatch is null) return false;

            // update children
            foreach (var order in dispatch.DispatchOrder.Where(y=>y.IsActive))
            {
                order.StatusId = (long)OrderStatusEnum.Processed;
                order.ModifiedById = sessionProvider.Session.LoggedInUserId;
                order.ModifiedDate = DateTime.Now;
            }

            // update header
            dispatch.StatusId = 2;
            dispatch.ProcessedDate = DateTime.Now;
            dispatch.ProcessedById = sessionProvider.Session.LoggedInUserId;

            check =  await unitOfWork.SaveChangesAsync();

            if (check > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
