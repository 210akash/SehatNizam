using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Item.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class SaveItemHandler : IRequestHandler<SaveItemCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveItemHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveItemCommand, long>.Handle(SaveItemCommand request, CancellationToken cancellationToken)
        {
            var ItemType = await unitOfWork.Repository<Entities.Models.ItemType>().GetFirstAsNoTrackingAsync(x => x.Id == request.ItemTypeId);
            var Item = await unitOfWork.Repository<Entities.Models.Item>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Item>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Item == null)
                {
                    string _ItemCode = "";
                    if (await unitOfWork.Repository<Entities.Models.Item>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.ItemTypeId == request.ItemTypeId))
                    {
                        Func<IQueryable<Entities.Models.Item>, IOrderedQueryable<Entities.Models.Item>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var ItemCode = await unitOfWork.Repository<Entities.Models.Item>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.ItemTypeId == request.ItemTypeId, OrderByDesc,null );
                        int No = Convert.ToInt32(new string(ItemCode.Code.TakeLast(4).ToArray())) + 1;
                        _ItemCode = No.ToString().PadLeft(4, '0');
                    }
                    else
                        _ItemCode = "0001";
                    request.Code = ItemType.Code + _ItemCode;

                    var _Item = mapper.Map<Entities.Models.Item>(request);
                    _Item.CompanyId = sessionProvider.Session.CompanyId;
                    _Item.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Item.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Item>().Add(_Item);
                    SaveChanges();
                }
                else
                {
                    var _Item = mapper.Map<Entities.Models.Item>(request);
                    _Item.Code = Item.Code;
                    _Item.CreatedById = Item.CreatedById;
                    _Item.CompanyId = Item.CompanyId;
                    _Item.CreatedDate = Item.CreatedDate;
                    _Item.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Item.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Item>().Update(_Item);
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