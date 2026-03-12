using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.CancelDispatch.Command;
using ERP.Mediator.Mediator.Dispatch.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.CancelDispatch.Handler
{
    public class SaveCancelDispatchHandler : IRequestHandler<SaveCancelDispatchCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;
        private long DNNumber = 0;

        public SaveCancelDispatchHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
        }

        public long SaveChanges()
        {
            try
            {
                return unitOfWork.SaveChanges();

            }
            catch (Exception re)
            {

                throw;
            }
        }

        async Task<long> IRequestHandler<SaveCancelDispatchCommand, long>.Handle(SaveCancelDispatchCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var CancelDispatch = await unitOfWork.Repository<Entities.Models.CancelDispatch>().GetFirstAsNoTrackingAsync(x => x.OrderId == request.OrderId && x.IsActive == true);
                if (CancelDispatch == null)
                {
                    string _DispatchCode = "";
                    var check = await unitOfWork.Repository<Entities.Models.CancelDispatch>().GetExistsAsync(x => x.IsActive);
                    if (check)
                    {
                        Func<IQueryable<Entities.Models.CancelDispatch>, IOrderedQueryable<Entities.Models.CancelDispatch>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                        var DispatchCode = await unitOfWork.Repository<Entities.Models.CancelDispatch>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                        int No = Convert.ToInt32(DispatchCode.Code) + 1;
                        _DispatchCode = No.ToString().PadLeft(7, '0');
                    }
                    else
                        _DispatchCode = "0000001";
                    request.Code = _DispatchCode;

                    Entities.Models.CancelDispatch _cancelDispatch = new Entities.Models.CancelDispatch();
                    _cancelDispatch.Code = _DispatchCode;
                    _cancelDispatch.OrderId = request.OrderId;
                    _cancelDispatch.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _cancelDispatch.CreatedDate = DateTime.Now;
                    _cancelDispatch.StatusId = (long)OrderStatusEnum.CancelDispatchCreated;
                    unitOfWork.Repository<Entities.Models.CancelDispatch>().Add(_cancelDispatch);
                    SaveChanges();

                    OrderProcess process = new OrderProcess();
                    process.CancelDispatchId = _cancelDispatch.Id;
                    process.Comments = request.Remarks;
                    process.ToStatusId = (long)OrderStatusEnum.CancelDispatchCreated;
                    process.CreatedById = sessionProvider.Session.LoggedInUserId;
                    process.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);
                    SaveChanges();

                    foreach (var item in request.GetOrderItems)
                    {
                        Entities.Models.CancelDispatchDetail _cancelDispatchDetail = new Entities.Models.CancelDispatchDetail();
                        _cancelDispatchDetail.CancelDispatchId = _cancelDispatch.Id;
                        _cancelDispatchDetail.OrderItemId = item.Id;
                        _cancelDispatchDetail.Quantity = item.Quantity;
                        _cancelDispatchDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _cancelDispatchDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.CancelDispatchDetail>().Add(_cancelDispatchDetail);
                        SaveChanges();
                    }
                    return 200;
                }
                else
                {
                    return 409;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
