using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Account.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Account.Handler
{
    public class SaveAccountHandler : IRequestHandler<SaveAccountCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAccountHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveAccountCommand, long>.Handle(SaveAccountCommand request, CancellationToken cancellationToken)
        {
            var AccountType = await unitOfWork.Repository<Entities.Models.AccountType>().GetFirstAsNoTrackingAsync(x => x.Id == request.AccountTypeId);
            var Account = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Account>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Account == null)
                {
                    string _AccountCode = "";
                    if (await unitOfWork.Repository<Entities.Models.Account>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.AccountTypeId == request.AccountTypeId))
                    {
                        Func<IQueryable<Entities.Models.Account>, IOrderedQueryable<Entities.Models.Account>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var AccountCode = await unitOfWork.Repository<Entities.Models.Account>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.AccountTypeId == request.AccountTypeId, OrderByDesc,null );
                        int No = Convert.ToInt32(new string(AccountCode.Code.TakeLast(4).ToArray())) + 1;
                        _AccountCode = No.ToString().PadLeft(4, '0');
                    }
                    else
                        _AccountCode = "0001";
                    request.Code = AccountType.Code + _AccountCode;

                    var _Account = mapper.Map<Entities.Models.Account>(request);
                    _Account.CompanyId = sessionProvider.Session.CompanyId;
                    _Account.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Account.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Account>().Add(_Account);
                    SaveChanges();
                }
                else
                {
                    var _Account = mapper.Map<Entities.Models.Account>(request);
                    _Account.Code = Account.Code;
                    _Account.CreatedById = Account.CreatedById;
                    _Account.CompanyId = Account.CompanyId;
                    _Account.CreatedDate = Account.CreatedDate;
                    _Account.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Account.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Account>().Update(_Account);
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