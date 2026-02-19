using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ItemType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Handler
{
    public class SaveItemTypeHandler : IRequestHandler<SaveItemTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveItemTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveItemTypeCommand, long>.Handle(SaveItemTypeCommand request, CancellationToken cancellationToken)
        {
            var SubCategory = await unitOfWork.Repository<Entities.Models.SubCategory>().GetFirstAsNoTrackingAsync(x => x.Id == request.SubCategoryId);
            var ItemType = await unitOfWork.Repository<Entities.Models.ItemType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.ItemType>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (ItemType == null)
                {
                    string _ItemTypeCode = "";
                    if (await unitOfWork.Repository<Entities.Models.ItemType>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.SubCategoryId == request.SubCategoryId))
                    {
                        Func<IQueryable<Entities.Models.ItemType>, IOrderedQueryable<Entities.Models.ItemType>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var ItemTypeCode = await unitOfWork.Repository<Entities.Models.ItemType>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.SubCategoryId == request.SubCategoryId, OrderByDesc,null );
                        int No = Convert.ToInt32(new string(ItemTypeCode.Code.TakeLast(2).ToArray())) + 1;
                        _ItemTypeCode = No.ToString().PadLeft(3, '0');
                    }
                    else
                        _ItemTypeCode = "001";
                    request.Code = SubCategory.Code + _ItemTypeCode;

                    var _ItemType = mapper.Map<Entities.Models.ItemType>(request);
                    _ItemType.CompanyId = sessionProvider.Session.CompanyId;
                    _ItemType.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _ItemType.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.ItemType>().Add(_ItemType);
                    SaveChanges();
                }
                else
                {
                    var _ItemType = mapper.Map<Entities.Models.ItemType>(request);
                    _ItemType.Code = ItemType.Code;
                    _ItemType.CreatedById = ItemType.CreatedById;
                    _ItemType.CompanyId = ItemType.CompanyId;
                    _ItemType.CreatedDate = ItemType.CreatedDate;
                    _ItemType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _ItemType.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.ItemType>().Update(_ItemType);
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