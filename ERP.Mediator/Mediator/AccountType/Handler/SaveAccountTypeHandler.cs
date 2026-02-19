using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountType.Handler
{
    public class SaveAccountTypeHandler : IRequestHandler<SaveAccountTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAccountTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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
            catch (Exception ex)
            {

                throw;
            }
        }

        async Task<long> IRequestHandler<SaveAccountTypeCommand, long>.Handle(SaveAccountTypeCommand request, CancellationToken cancellationToken)
        {
            var AccountSubCategory = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetFirstAsNoTrackingAsync(x => x.Id == request.AccountSubCategoryId);
            var AccountType = await unitOfWork.Repository<Entities.Models.AccountType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.AccountType>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (AccountType == null)
                {
                    string _AccountTypeCode = "";
                    if (await unitOfWork.Repository<Entities.Models.AccountType>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.AccountSubCategoryId == request.AccountSubCategoryId))
                    {
                        Func<IQueryable<Entities.Models.AccountType>, IOrderedQueryable<Entities.Models.AccountType>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var AccountTypeCode = await unitOfWork.Repository<Entities.Models.AccountType>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.AccountSubCategoryId == request.AccountSubCategoryId, OrderByDesc,null );
                        int No = Convert.ToInt32(new string(AccountTypeCode.Code.TakeLast(2).ToArray())) + 1;
                        _AccountTypeCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                        _AccountTypeCode = "01";
                    request.Code = AccountSubCategory.Code + _AccountTypeCode;

                    var _AccountType = mapper.Map<Entities.Models.AccountType>(request);
                    _AccountType.CompanyId = sessionProvider.Session.CompanyId;
                    _AccountType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _AccountType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountType>().Add(_AccountType);
                    SaveChanges();
                }
                else
                {
                    var _AccountType = mapper.Map<Entities.Models.AccountType>(request);
                    _AccountType.Code = AccountType.Code;
                    _AccountType.CreatedById = AccountType.CreatedById;
                    _AccountType.CompanyId = AccountType.CompanyId;
                    _AccountType.CreatedDate = AccountType.CreatedDate;
                    _AccountType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _AccountType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountType>().Update(_AccountType);
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