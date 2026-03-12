using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Handler
{
    public class DeleteSaleReturnHandler : IRequestHandler<DeleteSaleReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteSaleReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteSaleReturnQuery request, CancellationToken cancellationToken)
        {
            // 1️  Grab the SaleReturn *with* its details and keep it tracked
            var SaleReturn = await unitOfWork.Repository<Entities.Models.SaleReturn>().GetFirstAsync(y => y.Id == request.Id, null, null, "SaleReturnDetails");

            if (SaleReturn is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            SaleReturn.IsDelete = true;
            SaleReturn.IsActive = false;
            SaleReturn.DeleteDate = now;
            SaleReturn.ModifiedDate = now;
            SaleReturn.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in SaleReturn.SaleReturnDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.SaleReturn>().Update(SaleReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
