using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GST.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GST.Handler
{
    public class DeleteGSTHandler : IRequestHandler<DeleteGSTQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteGSTHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteGSTQuery request, CancellationToken cancellationToken)
        {
            var GST = await unitOfWork.Repository<Entities.Models.GST>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            GST.IsDelete = true;
            GST.IsActive = false;
            GST.DeleteDate = DateTime.Now;
            GST.ModifiedDate = DateTime.Now;
            GST.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.GST>().Update(GST);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
