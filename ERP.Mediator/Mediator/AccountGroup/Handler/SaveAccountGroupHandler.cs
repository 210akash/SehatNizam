using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountGroup.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountGroup.Handler
{
    public class SaveAccountGroupHandler : IRequestHandler<SaveAccountGroupCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAccountGroupHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveAccountGroupCommand, long>.Handle(SaveAccountGroupCommand request, CancellationToken cancellationToken)
        {
            var Account = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.AccountId);
            var AccountGroup = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (AccountGroup == null)
                {
                    string _AccountGroupCode = "";
                    if (await unitOfWork.Repository<Entities.Models.AccountGroup>().GetExistsAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.AccountId == request.AccountId))
                    {
                        Func<IQueryable<Entities.Models.AccountGroup>, IOrderedQueryable<Entities.Models.AccountGroup>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var AccountGroupCode = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.AccountId == request.AccountId, OrderByDesc,null );
                        int No = Convert.ToInt32(new string(AccountGroupCode.Code.TakeLast(4).ToArray())) + 1;
                        _AccountGroupCode = No.ToString().PadLeft(4, '0');
                    }
                    else
                        _AccountGroupCode = "0001";
                    request.Code = Account.Code + _AccountGroupCode;

                    var _AccountGroup = mapper.Map<Entities.Models.AccountGroup>(request);
                    _AccountGroup.CompanyId = sessionProvider.Session.CompanyId;
                    _AccountGroup.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _AccountGroup.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountGroup>().Add(_AccountGroup);
                    SaveChanges();
                }
                else
                {
                    string _AccountGroupCode = "";
                    if (await unitOfWork.Repository<Entities.Models.AccountGroup>().GetExistsAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.AccountId == request.AccountId))
                    {
                        Func<IQueryable<Entities.Models.AccountGroup>, IOrderedQueryable<Entities.Models.AccountGroup>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var AccountGroupCode = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.AccountId == request.AccountId, OrderByDesc, null);
                        int No = Convert.ToInt32(new string(AccountGroupCode.Code.TakeLast(4).ToArray())) + 1;
                        _AccountGroupCode = No.ToString().PadLeft(4, '0');
                    }
                    else
                        _AccountGroupCode = "0001";
                    request.Code = Account.Code + _AccountGroupCode;

                    var _AccountGroup = mapper.Map<Entities.Models.AccountGroup>(request);
                    _AccountGroup.Code = AccountGroup.Code;
                    _AccountGroup.CreatedById = AccountGroup.CreatedById;
                    _AccountGroup.CompanyId = AccountGroup.CompanyId;
                    _AccountGroup.CreatedDate = AccountGroup.CreatedDate;
                    _AccountGroup.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _AccountGroup.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountGroup>().Update(_AccountGroup);
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