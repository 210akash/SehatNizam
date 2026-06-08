using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.IPD.Room.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Room.Handler
{
    public class SaveRoomHandler : IRequestHandler<SaveRoomCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRoomHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRoomCommand, long>.Handle(SaveRoomCommand request, CancellationToken cancellationToken)
        {
            var Category = await unitOfWork.Repository<Entities.Models.AccountCategory>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.WardId);
            var Room = await unitOfWork.Repository<Entities.Models.Room>().GetFirstAsNoTrackingAsync(x => x.IsActive == true &&  x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Room>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId);

            if (checkDuplicate.Count() == 0)
            {
                if (Room == null)
                {
                    string _RoomCode = "";
                    if (await unitOfWork.Repository<Entities.Models.Room>().GetExistsAsync(y => y.IsActive == true && y.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.WardId == request.WardId))
                    {
                        Func<IQueryable<Entities.Models.Room>, IOrderedQueryable<Entities.Models.Room>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var RoomCode = await unitOfWork.Repository<Entities.Models.Room>().GetOneAsync(y => y.IsActive == true && y.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.WardId == request.WardId, OrderByDesc,null );
                        int No = Convert.ToInt32(new string(RoomCode.Code.TakeLast(2).ToArray())) + 1;
                        _RoomCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                        _RoomCode = "01";
                    request.Code = Category.Code + _RoomCode;

                    var _Room = mapper.Map<Entities.Models.Room>(request);
                    _Room.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Room.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Room>().Add(_Room);
                    SaveChanges();
                }
                else
                {
                    var _Room = mapper.Map<Entities.Models.Room>(request);
                    _Room.Code = Room.Code;
                    _Room.CreatedById = Room.CreatedById;
                    _Room.CreatedDate = Room.CreatedDate;
                    _Room.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Room.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Room>().Update(_Room);
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