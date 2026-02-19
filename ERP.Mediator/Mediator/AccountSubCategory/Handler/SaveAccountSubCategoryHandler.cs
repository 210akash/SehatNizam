using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.AccountSubCategory.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Handler
{
    public class SaveAccountSubCategoryHandler : IRequestHandler<SaveAccountSubCategoryCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAccountSubCategoryHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveAccountSubCategoryCommand, long>.Handle(SaveAccountSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var Category = await unitOfWork.Repository<Entities.Models.AccountCategory>().GetFirstAsNoTrackingAsync(x => x.Id == request.AccountCategoryId);
            var AccountSubCategory = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (AccountSubCategory == null)
                {
                    string _AccountSubCategoryCode = "";
                    if (await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.AccountCategoryId == request.AccountCategoryId))
                    {
                        Func<IQueryable<Entities.Models.AccountSubCategory>, IOrderedQueryable<Entities.Models.AccountSubCategory>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var AccountSubCategoryCode = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.AccountCategoryId == request.AccountCategoryId, OrderByDesc,null );
                        int No = Convert.ToInt32(new string(AccountSubCategoryCode.Code.TakeLast(2).ToArray())) + 1;
                        _AccountSubCategoryCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                        _AccountSubCategoryCode = "01";
                    request.Code = Category.Code + _AccountSubCategoryCode;

                    var _AccountSubCategory = mapper.Map<Entities.Models.AccountSubCategory>(request);
                    _AccountSubCategory.CompanyId = sessionProvider.Session.CompanyId;
                    _AccountSubCategory.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _AccountSubCategory.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountSubCategory>().Add(_AccountSubCategory);
                    SaveChanges();
                }
                else
                {
                    var _AccountSubCategory = mapper.Map<Entities.Models.AccountSubCategory>(request);
                    _AccountSubCategory.Code = AccountSubCategory.Code;
                    _AccountSubCategory.CreatedById = AccountSubCategory.CreatedById;
                    _AccountSubCategory.CompanyId = AccountSubCategory.CompanyId;
                    _AccountSubCategory.CreatedDate = AccountSubCategory.CreatedDate;
                    _AccountSubCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _AccountSubCategory.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountSubCategory>().Update(_AccountSubCategory);
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