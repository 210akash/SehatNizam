using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Item.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class DeleteItemHandler : IRequestHandler<DeleteItemQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteItemHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteItemQuery request, CancellationToken cancellationToken)
        {
            var Item = await unitOfWork.Repository<Entities.Models.Item>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Item.IsDelete = true;
            Item.IsActive = false;
            Item.DeleteDate = DateTime.Now;
            Item.ModifiedDate = DateTime.Now;
            Item.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Item>().Update(Item);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
