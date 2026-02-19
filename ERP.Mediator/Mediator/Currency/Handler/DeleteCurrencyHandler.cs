using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Currency.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Currency.Handler
{
    public class DeleteCurrencyHandler : IRequestHandler<DeleteCurrencyQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteCurrencyHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteCurrencyQuery request, CancellationToken cancellationToken)
        {
            var Currency = await unitOfWork.Repository<Entities.Models.Currency>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Currency.IsDelete = true;
            Currency.IsActive = false;
            Currency.DeleteDate = DateTime.Now;
            Currency.ModifiedDate = DateTime.Now;
            Currency.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Currency>().Update(Currency);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
