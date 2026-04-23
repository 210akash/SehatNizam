using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.SalaryTaxSlab.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryTaxSlab.Handler
{
    public class SaveSalaryTaxSlabHandler : IRequestHandler<SaveSalaryTaxSlabCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveSalaryTaxSlabHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveSalaryTaxSlabCommand, long>.Handle(SaveSalaryTaxSlabCommand request, CancellationToken cancellationToken)
        {
            var salarytaxslab = await unitOfWork.Repository<Entities.Models.SalaryTaxSlab>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.SalaryTaxSlab>().GetAsync(x => x.FromAmount == request.FromAmount && x.ToAmount == request.ToAmount && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (salarytaxslab == null)
                {
                    var _salarytaxslab = mapper.Map<Entities.Models.SalaryTaxSlab>(request);
                    _salarytaxslab.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _salarytaxslab.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.SalaryTaxSlab>().Add(_salarytaxslab);
                    SaveChanges();
                }
                else
                {
                    var _salarytaxslab = mapper.Map<Entities.Models.SalaryTaxSlab>(request);
                    _salarytaxslab.CreatedById = salarytaxslab.CreatedById;
                    _salarytaxslab.CreatedDate = salarytaxslab.CreatedDate;
                    _salarytaxslab.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _salarytaxslab.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.SalaryTaxSlab>().Update(_salarytaxslab);
                    SaveChanges();
                }
                return 200;

            }
            else
            {
                return 409;
            }

        }
    }
}