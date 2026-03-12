using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleMaterialReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Handler
{
    public class DeleteSaleMaterialReturnHandler : IRequestHandler<DeleteSaleMaterialReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteSaleMaterialReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteSaleMaterialReturnQuery request, CancellationToken cancellationToken)
        {
            // 1️  Grab the SaleMaterialReturn *with* its details and keep it tracked
            var SaleMaterialReturn = await unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetFirstAsync(y => y.Id == request.Id, null, null, "SaleMaterialReturnDetails");

            if (SaleMaterialReturn is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            SaleMaterialReturn.IsDelete = true;
            SaleMaterialReturn.IsActive = false;
            SaleMaterialReturn.DeleteDate = now;
            SaleMaterialReturn.ModifiedDate = now;
            SaleMaterialReturn.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in SaleMaterialReturn.SaleMaterialReturnDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().Update(SaleMaterialReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
