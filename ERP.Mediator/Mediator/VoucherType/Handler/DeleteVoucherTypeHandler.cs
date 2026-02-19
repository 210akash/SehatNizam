using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.VoucherType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.VoucherType.Handler
{
    public class DeleteVoucherTypeHandler : IRequestHandler<DeleteVoucherTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteVoucherTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteVoucherTypeQuery request, CancellationToken cancellationToken)
        {
            var VoucherType = await unitOfWork.Repository<Entities.Models.VoucherType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            VoucherType.IsDelete = true;
            VoucherType.IsActive = false;
            VoucherType.DeleteDate = DateTime.Now;
            VoucherType.ModifiedDate = DateTime.Now;
            VoucherType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.VoucherType>().Update(VoucherType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
