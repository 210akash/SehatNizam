using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Room.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Room.Handler
{
    public class DeleteRoomHandler : IRequestHandler<DeleteRoomQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRoomHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteRoomQuery request, CancellationToken cancellationToken)
        {
            var Room = await unitOfWork.Repository<Entities.Models.Room>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Room.IsDelete = true;
            Room.IsActive = false;
            Room.DeleteDate = DateTime.Now;
            Room.ModifiedDate = DateTime.Now;
            Room.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Room>().Update(Room);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
