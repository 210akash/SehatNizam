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
    public class RejectSaleMaterialHandler : IRequestHandler<RejectSaleMaterialQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public RejectSaleMaterialHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(RejectSaleMaterialQuery request, CancellationToken cancellationToken)
        {
            var SaleMaterial = await unitOfWork.Repository<Entities.Models.SaleMaterial>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            SaleMaterial.StatusId = 1;
            SaleMaterial.ModifiedDate = DateTime.Now;
            SaleMaterial.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.SaleMaterial>().Update(SaleMaterial);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
