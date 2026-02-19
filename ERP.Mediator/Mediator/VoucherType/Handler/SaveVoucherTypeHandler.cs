using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.VoucherType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.VoucherType.Handler
{
    public class SaveVoucherTypeHandler : IRequestHandler<SaveVoucherTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveVoucherTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            try
            {
                return unitOfWork.SaveChanges();

            }
            catch (Exception dex)
            {

                throw;
            }
        }

        public async Task<long> Handle(SaveVoucherTypeCommand request, CancellationToken cancellationToken)
        {
            var VoucherType = await unitOfWork.Repository<Entities.Models.VoucherType>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.VoucherType>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id
                               && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (VoucherType == null)
                {
                    string _VoucherTypeCode = "";
                    if (await unitOfWork.Repository<Entities.Models.VoucherType>()
                        .GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.IsActive == true))
                    {
                        Func<IQueryable<Entities.Models.VoucherType>, IOrderedQueryable<Entities.Models.VoucherType>> OrderByDesc =
                            query => query.OrderByDescending(x => x.Code);
                        var VoucherTypeCode = await unitOfWork.Repository<Entities.Models.VoucherType>()
                            .GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                        int No = Convert.ToInt32(VoucherTypeCode.Code) + 1;
                        _VoucherTypeCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                    {
                        _VoucherTypeCode = "01";
                    }

                    request.Code = _VoucherTypeCode;

                    var _VoucherType = mapper.Map<Entities.Models.VoucherType>(request);
                    _VoucherType.CompanyId = sessionProvider.Session.CompanyId;
                    _VoucherType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _VoucherType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.VoucherType>().Add(_VoucherType);
                    SaveChanges();
                }
                else
                {
                    var _VoucherType = mapper.Map<Entities.Models.VoucherType>(request);
                    _VoucherType.Code = VoucherType.Code;
                    _VoucherType.CompanyId = VoucherType.CompanyId;
                    _VoucherType.CreatedById = VoucherType.CreatedById;
                    _VoucherType.CreatedDate = VoucherType.CreatedDate;
                    _VoucherType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _VoucherType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.VoucherType>().Update(_VoucherType);
                    SaveChanges();
                }

                return 200; // Success code for adding/updating
            }
            else
            {
                return 409; // Conflict code for duplicate
            }
        }
    }
}