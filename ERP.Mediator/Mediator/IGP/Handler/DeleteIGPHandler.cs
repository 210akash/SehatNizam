using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IGP.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class DeleteIGPHandler : IRequestHandler<DeleteIGPQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteIGPHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteIGPQuery request, CancellationToken cancellationToken)
        {
            var IGP = await unitOfWork.Repository<Entities.Models.IGP>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            IGP.IsDelete = true;
            IGP.IsActive = false;
            IGP.DeleteDate = DateTime.Now;
            IGP.ModifiedDate = DateTime.Now;
            IGP.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.IGP>().Update(IGP);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
