using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Vendor.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Vendor.Handler
{
    public class DeleteVendorHandler : IRequestHandler<DeleteVendorQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteVendorHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteVendorQuery request, CancellationToken cancellationToken)
        {
            var vendor = await unitOfWork.Repository<Entities.Models.Vendor>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            vendor.IsDelete = true;
            vendor.IsActive = false;
            vendor.DeleteDate = DateTime.Now;
            vendor.ModifiedDate = DateTime.Now;
            vendor.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Vendor>().Update(vendor);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
