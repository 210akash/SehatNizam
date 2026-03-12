using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleMaterial.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterial.Handler
{
    public class ProcessSaleMaterialHandler : IRequestHandler<ProcessSaleMaterialQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessSaleMaterialHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessSaleMaterialQuery request, CancellationToken cancellationToken)
        {
            var SaleMaterial = await unitOfWork.Repository<Entities.Models.SaleMaterial>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            SaleMaterial.StatusId = 2;
            SaleMaterial.ModifiedDate = DateTime.Now;
            SaleMaterial.ModifiedById = sessionProvider.Session.LoggedInUserId;

            SaleMaterial.ProcessedDate = DateTime.Now;
            SaleMaterial.ProcessedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Repository<Entities.Models.SaleMaterial>().Update(SaleMaterial);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
