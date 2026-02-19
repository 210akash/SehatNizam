using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Category.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Category.Handler
{
    public class SaveCategoryHandler : IRequestHandler<SaveCategoryCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveCategoryHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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

        public async Task<long> Handle(SaveCategoryCommand request, CancellationToken cancellationToken)
        {
            var Category = await unitOfWork.Repository<Entities.Models.Category>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Category>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id
                               && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Category == null)
                {
                    string _CategoryCode = "";
                    if (await unitOfWork.Repository<Entities.Models.Category>()
                        .GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId))
                    {
                        Func<IQueryable<Entities.Models.Category>, IOrderedQueryable<Entities.Models.Category>> OrderByDesc =
                            query => query.OrderByDescending(x => x.Code);
                        var CategoryCode = await unitOfWork.Repository<Entities.Models.Category>()
                            .GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                        int No = Convert.ToInt32(CategoryCode.Code) + 1;
                        _CategoryCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                    {
                        _CategoryCode = "01";
                    }

                    request.Code = _CategoryCode;

                    var _Category = mapper.Map<Entities.Models.Category>(request);
                    _Category.CompanyId = sessionProvider.Session.CompanyId;
                    _Category.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Category.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Category>().Add(_Category);
                    SaveChanges();

                    foreach (var item in request.StoreIds)
                    {
                        Entities.Models.CategoryStore lObjCategoryStore = new()
                        {
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            CategoryId = _Category.Id,
                            StoreId = item
                        };
                        unitOfWork.Repository<Entities.Models.CategoryStore>().Add(lObjCategoryStore);
                    }
                    SaveChanges();
                }
                else
                {
                    var _Category = mapper.Map<Entities.Models.Category>(request);
                    _Category.Code = Category.Code;
                    _Category.CompanyId = Category.CompanyId;
                    _Category.CreatedById = Category.CreatedById;
                    _Category.CreatedDate = Category.CreatedDate;
                    _Category.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Category.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Category>().Update(_Category);
                    SaveChanges();

                    var CategoryStoreList = await unitOfWork.Repository<CategoryStore>()
                        .GetPagingWhereAsNoTrackingAsync(y => y.CategoryId == request.Id && y.IsActive == true,
                        null, null, null, null, null).Item1.ToListAsync();

                    List<long> previousCategoryStoreIds = CategoryStoreList
                        .Select(y => y.StoreId)
                        .ToList();

                    List<long> currentCategoryStoreIds = request.StoreIds;
                    List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();
                    List<long> addCategoryStoreIds = currentCategoryStoreIds.Except(previousCategoryStoreIds).ToList();

                    // Handle deletions
                    foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                    {
                        CategoryStore categoryStore = CategoryStoreList.Where(y=>y.StoreId == deletedCategoryStoreId).FirstOrDefault();

                        if (categoryStore != null)
                        {
                            categoryStore.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            categoryStore.ModifiedDate = DateTime.Now;
                            categoryStore.IsActive = false; // Soft delete
                            categoryStore.IsDelete = true; // Soft delete
                            unitOfWork.Repository<Entities.Models.CategoryStore>().Update(categoryStore);
                            SaveChanges();
                        }
                    }

                    // Handle additions
                    foreach (var storeId in addCategoryStoreIds)
                    {
                        CategoryStore lObjCategoryStore = new()
                        {
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            CategoryId = request.Id,
                            StoreId = storeId
                        };
                        unitOfWork.Repository<CategoryStore>().Add(lObjCategoryStore);
                        SaveChanges();
                    }
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