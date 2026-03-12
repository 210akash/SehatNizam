using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Vendor.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Vendor.Handler
{
    public class SaveVendorHandler : IRequestHandler<SaveVendorCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveVendorHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveVendorCommand, long>.Handle(SaveVendorCommand request, CancellationToken cancellationToken)
        {
            var vendor = await unitOfWork.Repository<Entities.Models.Vendor>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Vendor>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (vendor == null)
                {
                    var _vendor = mapper.Map<Entities.Models.Vendor>(request);
                    _vendor.CompanyId = sessionProvider.Session.CompanyId;
                    _vendor.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _vendor.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Vendor>().Add(_vendor);
                    SaveChanges();
                }
                else
                {
                    var _vendor = mapper.Map<Entities.Models.Vendor>(request);
                    _vendor.CompanyId = vendor.CompanyId;
                    _vendor.CreatedById = vendor.CreatedById;
                    _vendor.CreatedDate = vendor.CreatedDate;
                    _vendor.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _vendor.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Vendor>().Update(_vendor);
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