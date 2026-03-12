using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Inspection.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Handler
{
    public class DeleteInspectionHandler : IRequestHandler<DeleteInspectionQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteInspectionHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteInspectionQuery request, CancellationToken cancellationToken)
        {
            // 1️  Grab the Inspection *with* its details and keep it tracked
            var Inspection = await unitOfWork.Repository<Entities.Models.Inspection>().GetFirstAsync(y => y.Id == request.Id, null, null, "InspectionDetail");

            if (Inspection is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            Inspection.IsDelete = true;
            Inspection.IsActive = false;
            Inspection.DeleteDate = now;
            Inspection.ModifiedDate = now;
            Inspection.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in Inspection.InspectionDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.Inspection>().Update(Inspection);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
