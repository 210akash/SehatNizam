using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class ProcessGRNHandler : IRequestHandler<ProcessGRNQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessGRNHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessGRNQuery request, CancellationToken cancellationToken)
        {
            var GRN = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            GRN.StatusId = 2;
            GRN.ProcessedDate = DateTime.Now;
            GRN.ProcessedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.GRN>().Update(GRN);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
