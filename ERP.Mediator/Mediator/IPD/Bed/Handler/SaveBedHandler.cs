using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Bed.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Handler
{
    public class SaveBedHandler : IRequestHandler<SaveBedCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveBedHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveBedCommand, long>.Handle(SaveBedCommand request, CancellationToken cancellationToken)
        {
            var Room = await unitOfWork.Repository<Entities.Models.Room>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.RoomId);
            var Bed = await unitOfWork.Repository<Entities.Models.Bed>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Bed>().GetAsync(x => x.BedNo.ToLower() == request.BedNo.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.Room.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId);

            if (checkDuplicate.Count() == 0)
            {
                if (Bed == null)
                {
                    string _BedCode = "";
                    if (await unitOfWork.Repository<Entities.Models.Bed>().GetExistsAsync(y => y.IsActive == true && y.Room.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.RoomId == request.RoomId))
                    {
                        Func<IQueryable<Entities.Models.Bed>, IOrderedQueryable<Entities.Models.Bed>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var BedCode = await unitOfWork.Repository<Entities.Models.Bed>().GetOneAsync(y => y.IsActive == true && y.Room.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.RoomId == request.RoomId, OrderByDesc,null);
                        int No = Convert.ToInt32(new string(BedCode.Code.TakeLast(2).ToArray())) + 1;
                        _BedCode = No.ToString().PadLeft(2, '0');
                    }
                    else
                        _BedCode = "01";
                    request.Code = Room.Code + _BedCode;

                    var _Bed = mapper.Map<Entities.Models.Bed>(request);
                    _Bed.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Bed.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Bed>().Add(_Bed);
                    SaveChanges();
                }
                else
                {
                    var _Bed = mapper.Map<Entities.Models.Bed>(request);
                    _Bed.Code = Bed.Code;
                    _Bed.CreatedById = Bed.CreatedById;
                    _Bed.CreatedDate = Bed.CreatedDate;
                    _Bed.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Bed.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Bed>().Update(_Bed);
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