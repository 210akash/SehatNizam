using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.SalaryTaxSlab.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryTaxSlab.Handler
{
    public class DeleteSalaryTaxSlabHandler : IRequestHandler<DeleteSalaryTaxSlabQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteSalaryTaxSlabHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteSalaryTaxSlabQuery request, CancellationToken cancellationToken)
        {
            var salarytaxslab = await unitOfWork.Repository<Entities.Models.SalaryTaxSlab>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            salarytaxslab.IsDelete = true;
            salarytaxslab.IsActive = false;
            salarytaxslab.DeleteDate = DateTime.Now;
            salarytaxslab.ModifiedDate = DateTime.Now;
            salarytaxslab.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.SalaryTaxSlab>().Update(salarytaxslab);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
