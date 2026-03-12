using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RetailOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Handler
{
    public class DeleteRetailOrderReturnHandler : IRequestHandler<DeleteRetailOrderReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRetailOrderReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteRetailOrderReturnQuery request, CancellationToken cancellationToken)
        {
            // 1️  Grab the RetailOrderReturn *with* its details and keep it tracked
            var RetailOrderReturn = await unitOfWork.Repository<Entities.Models.RetailOrderReturn>().GetFirstAsync(y => y.Id == request.Id, null, null, "RetailOrderReturnDetail");

            if (RetailOrderReturn is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            RetailOrderReturn.IsDelete = true;
            RetailOrderReturn.IsActive = false;
            RetailOrderReturn.DeleteDate = now;
            RetailOrderReturn.ModifiedDate = now;
            RetailOrderReturn.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in RetailOrderReturn.RetailOrderReturnDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.RetailOrderReturn>().Update(RetailOrderReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
