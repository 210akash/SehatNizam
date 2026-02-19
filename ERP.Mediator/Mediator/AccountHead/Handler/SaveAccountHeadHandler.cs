using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountHead.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountHead.Handler
{
    public class SaveAccountHeadHandler : IRequestHandler<SaveAccountHeadCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAccountHeadHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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

        public async Task<long> Handle(SaveAccountHeadCommand request, CancellationToken cancellationToken)
        {
            var AccountHead = await unitOfWork.Repository<Entities.Models.AccountHead>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.AccountHead>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id
                               && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (AccountHead == null)
                {
                    string _AccountHeadCode = "";
                    if (await unitOfWork.Repository<Entities.Models.AccountHead>()
                        .GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.IsActive == true))
                    {
                        Func<IQueryable<Entities.Models.AccountHead>, IOrderedQueryable<Entities.Models.AccountHead>> OrderByDesc =
                            query => query.OrderByDescending(x => x.Code);
                        var AccountHeadCode = await unitOfWork.Repository<Entities.Models.AccountHead>()
                            .GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                        int No = Convert.ToInt32(AccountHeadCode.Code) + 1;
                        _AccountHeadCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                    {
                        _AccountHeadCode = "01";
                    }

                    request.Code = _AccountHeadCode;

                    var _AccountHead = mapper.Map<Entities.Models.AccountHead>(request);
                    _AccountHead.CompanyId = sessionProvider.Session.CompanyId;
                    _AccountHead.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _AccountHead.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountHead>().Add(_AccountHead);
                    SaveChanges();
                }
                else
                {
                    var _AccountHead = mapper.Map<Entities.Models.AccountHead>(request);
                    _AccountHead.Code = AccountHead.Code;
                    _AccountHead.CompanyId = AccountHead.CompanyId;
                    _AccountHead.CreatedById = AccountHead.CreatedById;
                    _AccountHead.CreatedDate = AccountHead.CreatedDate;
                    _AccountHead.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _AccountHead.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountHead>().Update(_AccountHead);
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