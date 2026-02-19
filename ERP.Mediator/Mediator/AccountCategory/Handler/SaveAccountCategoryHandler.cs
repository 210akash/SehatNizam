using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountCategory.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountCategory.Handler
{
    public class SaveAccountCategoryHandler : IRequestHandler<SaveAccountCategoryCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAccountCategoryHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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

        public async Task<long> Handle(SaveAccountCategoryCommand request, CancellationToken cancellationToken)
        {
            var AccountCategory = await unitOfWork.Repository<Entities.Models.AccountCategory>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.AccountCategory>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id
                               && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (AccountCategory == null)
                {
                    string _AccountCategoryCode = "";
                    if (await unitOfWork.Repository<Entities.Models.AccountCategory>()
                        .GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.IsActive == true))
                    {
                        Func<IQueryable<Entities.Models.AccountCategory>, IOrderedQueryable<Entities.Models.AccountCategory>> OrderByDesc =
                            query => query.OrderByDescending(x => x.Code);
                        var AccountCategoryCode = await unitOfWork.Repository<Entities.Models.AccountCategory>()
                            .GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                        int No = Convert.ToInt32(AccountCategoryCode.Code) + 1;
                        _AccountCategoryCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                    {
                        _AccountCategoryCode = "01";
                    }

                    request.Code = _AccountCategoryCode;

                    var _AccountCategory = mapper.Map<Entities.Models.AccountCategory>(request);
                    _AccountCategory.CompanyId = sessionProvider.Session.CompanyId;
                    _AccountCategory.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _AccountCategory.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountCategory>().Add(_AccountCategory);
                    SaveChanges();
                }
                else
                {
                    var _AccountCategory = mapper.Map<Entities.Models.AccountCategory>(request);
                    _AccountCategory.Code = AccountCategory.Code;
                    _AccountCategory.CompanyId = AccountCategory.CompanyId;
                    _AccountCategory.CreatedById = AccountCategory.CreatedById;
                    _AccountCategory.CreatedDate = AccountCategory.CreatedDate;
                    _AccountCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _AccountCategory.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.AccountCategory>().Update(_AccountCategory);
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