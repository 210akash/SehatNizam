using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ItemType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Handler
{
    public class DeleteItemTypeHandler : IRequestHandler<DeleteItemTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteItemTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteItemTypeQuery request, CancellationToken cancellationToken)
        {
            var ItemType = await unitOfWork.Repository<Entities.Models.ItemType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            ItemType.IsDelete = true;
            ItemType.IsActive = false;
            ItemType.DeleteDate = DateTime.Now;
            ItemType.ModifiedDate = DateTime.Now;
            ItemType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.ItemType>().Update(ItemType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
