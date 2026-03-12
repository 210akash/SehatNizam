using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.UOM.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UOM.Handler
{
    public class DeleteUOMHandler : IRequestHandler<DeleteUOMQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteUOMHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteUOMQuery request, CancellationToken cancellationToken)
        {
            var UOM = await unitOfWork.Repository<Entities.Models.UOM>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            UOM.IsDelete = true;
            UOM.IsActive = false;
            UOM.DeleteDate = DateTime.Now;
            UOM.ModifiedDate = DateTime.Now;
            UOM.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.UOM>().Update(UOM);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
