using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dispatch.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class UpdateDispatchPrintStatusHandler : IRequestHandler<UpdateDispatchPrintStatusCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public UpdateDispatchPrintStatusHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
        }

        public long SaveChanges()
        {
            try
            {
                return unitOfWork.SaveChanges();

            }
            catch (Exception re)
            {

                throw;
            }
        }
        async Task<long> IRequestHandler<UpdateDispatchPrintStatusCommand, long>.Handle(UpdateDispatchPrintStatusCommand request, CancellationToken cancellationToken)
        {
            var DispatchOrder = await unitOfWork.Repository<Entities.Models.DispatchOrder>().GetFirstAsNoTrackingAsync(x => x.Id == request.DispatchOrderId);
            if (DispatchOrder != null)
            {
                if (DispatchOrder.PrintById == null)
                {
                    DispatchOrder.PrintById = sessionProvider.Session.LoggedInUserId;
                    DispatchOrder.PrintDate = DateTime.Now;
                    unitOfWork.Repository<DispatchOrder>().Update(DispatchOrder);
                    SaveChanges();
                }
                return 200;
            }
            else
            {
                return 400;
            }
        }
    }
}
