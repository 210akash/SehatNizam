using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PaymentMode.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Handler
{
    public class DeletePaymentModeHandler : IRequestHandler<DeletePaymentModeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeletePaymentModeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeletePaymentModeQuery request, CancellationToken cancellationToken)
        {
            var PaymentMode = await unitOfWork.Repository<Entities.Models.PaymentMode>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            PaymentMode.IsDelete = true;
            PaymentMode.IsActive = false;
            PaymentMode.DeleteDate = DateTime.Now;
            PaymentMode.ModifiedDate = DateTime.Now;
            PaymentMode.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PaymentMode>().Update(PaymentMode);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
