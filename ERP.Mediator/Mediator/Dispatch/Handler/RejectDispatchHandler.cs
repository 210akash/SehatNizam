using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class RejectDispatchHandler : IRequestHandler<RejectDispatchQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;
        private readonly List<string> codes = new();
        public RejectDispatchHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }

        public async Task<Tuple<long, string>> Handle(RejectDispatchQuery request, CancellationToken cancellationToken)
        {
            var dispatch = await unitOfWork.Repository<Entities.Models.Dispatch>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            dispatch.StatusId = 1;
            dispatch.ModifiedDate = DateTime.Now;
            dispatch.ModifiedById = sessionProvider.Session.LoggedInUserId;
            dispatch.ApprovedDate = DateTime.Now;
            dispatch.ApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Dispatch>().Update(dispatch);
            var check = await unitOfWork.SaveChangesAsync();
            if (check > 0)
            {
                return new Tuple<long, string>(200, "Dispatch Reject Successful!");
            }
            else
            {
                return new Tuple<long, string>(500, "Error Rejecting, Please contact system admin!");
            }
        }
    }
}
