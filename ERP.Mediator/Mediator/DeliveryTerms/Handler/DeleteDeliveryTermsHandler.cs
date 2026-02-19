using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.DeliveryTerms.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Handler
{
    public class DeleteDeliveryTermsHandler : IRequestHandler<DeleteDeliveryTermsQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteDeliveryTermsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteDeliveryTermsQuery request, CancellationToken cancellationToken)
        {
            var DeliveryTerms = await unitOfWork.Repository<Entities.Models.DeliveryTerms>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            DeliveryTerms.IsDelete = true;
            DeliveryTerms.IsActive = false;
            DeliveryTerms.DeleteDate = DateTime.Now;
            DeliveryTerms.ModifiedDate = DateTime.Now;
            DeliveryTerms.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.DeliveryTerms>().Update(DeliveryTerms);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
