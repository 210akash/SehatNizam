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
    public class DeleteSaleMaterialHandler : IRequestHandler<DeleteSaleMaterialQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteSaleMaterialHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteSaleMaterialQuery request, CancellationToken cancellationToken)
        {
            var SaleMaterial = await unitOfWork.Repository<Entities.Models.SaleMaterial>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            SaleMaterial.IsDelete = true;
            SaleMaterial.IsActive = false;
            SaleMaterial.DeleteDate = DateTime.Now;
            SaleMaterial.ModifiedDate = DateTime.Now;
            SaleMaterial.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.SaleMaterial>().Update(SaleMaterial);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
