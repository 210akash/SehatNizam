using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.CostSheet.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.CostSheet.Handler
{
    public class DeleteCostSheetHandler : IRequestHandler<DeleteCostSheetQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteCostSheetHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteCostSheetQuery request, CancellationToken cancellationToken)
        {
            var CostSheet = await unitOfWork.Repository<Entities.Models.CostSheet>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            CostSheet.IsDelete = true;
            CostSheet.IsActive = false;
            CostSheet.DeleteDate = DateTime.Now;
            CostSheet.ModifiedDate = DateTime.Now;
            CostSheet.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.CostSheet>().Update(CostSheet);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
