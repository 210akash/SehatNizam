using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.VisitType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.VisitType.Handler
{
    public class DeleteVisitTypeHandler : IRequestHandler<DeleteVisitTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteVisitTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteVisitTypeQuery request, CancellationToken cancellationToken)
        {
            var VisitType = await unitOfWork.Repository<Entities.Models.VisitType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            VisitType.IsDelete = true;
            VisitType.IsActive = false;
            VisitType.DeleteDate = DateTime.Now;
            VisitType.ModifiedDate = DateTime.Now;
            VisitType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.VisitType>().Update(VisitType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
