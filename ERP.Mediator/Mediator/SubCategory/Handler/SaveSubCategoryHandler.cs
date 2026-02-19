using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SubCategory.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Handler
{
    public class SaveSubCategoryHandler : IRequestHandler<SaveSubCategoryCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveSubCategoryHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveSubCategoryCommand, long>.Handle(SaveSubCategoryCommand request, CancellationToken cancellationToken)
        {
            var Category = await unitOfWork.Repository<Entities.Models.Category>().GetFirstAsNoTrackingAsync(x => x.Id == request.CategoryId);
            var SubCategory = await unitOfWork.Repository<Entities.Models.SubCategory>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.SubCategory>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (SubCategory == null)
                {
                    string _SubCategoryCode = "";
                    if (await unitOfWork.Repository<Entities.Models.SubCategory>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.CategoryId == request.CategoryId))
                    {
                        Func<IQueryable<Entities.Models.SubCategory>, IOrderedQueryable<Entities.Models.SubCategory>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var SubCategoryCode = await unitOfWork.Repository<Entities.Models.SubCategory>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.CategoryId == request.CategoryId, OrderByDesc,null );
                        int No = Convert.ToInt32(new string(SubCategoryCode.Code.TakeLast(2).ToArray())) + 1;
                        _SubCategoryCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                        _SubCategoryCode = "01";
                    request.Code = Category.Code + _SubCategoryCode;

                    var _SubCategory = mapper.Map<Entities.Models.SubCategory>(request);
                    _SubCategory.CompanyId = sessionProvider.Session.CompanyId;
                    _SubCategory.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _SubCategory.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.SubCategory>().Add(_SubCategory);
                    SaveChanges();
                }
                else
                {
                    var _SubCategory = mapper.Map<Entities.Models.SubCategory>(request);
                    _SubCategory.Code = SubCategory.Code;
                    _SubCategory.CreatedById = SubCategory.CreatedById;
                    _SubCategory.CompanyId = SubCategory.CompanyId;
                    _SubCategory.CreatedDate = SubCategory.CreatedDate;
                    _SubCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _SubCategory.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.SubCategory>().Update(_SubCategory);
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