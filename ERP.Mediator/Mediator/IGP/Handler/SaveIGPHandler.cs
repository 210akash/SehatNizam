using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.IGP.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class SaveIGPHandler : IRequestHandler<SaveIGPCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public SaveIGPHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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

        async Task<long> IRequestHandler<SaveIGPCommand, long>.Handle(SaveIGPCommand request, CancellationToken cancellationToken)
        {
            var IGP = await unitOfWork.Repository<Entities.Models.IGP>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (IGP == null)
            {
                string _IGPCode = "";
                if (await unitOfWork.Repository<Entities.Models.IGP>().GetExistsAsync())
                {
                    Func<IQueryable<Entities.Models.IGP>, IOrderedQueryable<Entities.Models.IGP>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var IGPCode = await unitOfWork.Repository<Entities.Models.IGP>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                    int No = Convert.ToInt32(IGPCode.Code) + 1;
                    _IGPCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _IGPCode = "0000001";
                request.Code = _IGPCode;

                var _IGP = mapper.Map<Entities.Models.IGP>(request);
                _IGP.CreatedById = sessionProvider.Session.LoggedInUserId;
                _IGP.CreatedDate = DateTime.Now;
                _IGP.StatusId = 1;

                _IGP.IGPDetails.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId;
                });

                unitOfWork.Repository<Entities.Models.IGP>().Add(_IGP);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate = masterupdate.IGPDetails;
                masterupdate.IGPDetails = null;
                var _IGP = mapper.Map<Entities.Models.IGP>(masterupdate);
                _IGP.Code = IGP.Code;
                _IGP.StatusId = IGP.StatusId;
                _IGP.CreatedById = IGP.CreatedById;
                _IGP.CreatedDate = IGP.CreatedDate;
                _IGP.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _IGP.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.IGP>().Update(_IGP);


                var IGPDetailsList = await unitOfWork.Repository<IGPDetails>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.IGPId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousIGPDetailIds = IGPDetailsList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousIGPDetailIds.Except(currentCategoryStoreIds).ToList();

                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    IGPDetails _IGPDetails = IGPDetailsList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_IGPDetails != null)
                    {
                        _IGPDetails.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _IGPDetails.DeleteDate = DateTime.Now;
                        _IGPDetails.IsActive = false;
                        _IGPDetails.IsDelete = true;
                        unitOfWork.Repository<Entities.Models.IGPDetails>().Update(_IGPDetails);
                    }
                }

                foreach (var IGPD in detailupdate)
                {
                    if (IGPD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<IGPDetails>()
                           .GetFirstAsync(x => x.Id == IGPD.Id);
                        updatedetail.PurchaseOrderDetailId = IGPD.PurchaseOrderDetailId;
                        updatedetail.Received = IGPD.Received;
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<IGPDetails>().Update(updatedetail);
                    }
                    else
                    {
                        var _IGPDetails = mapper.Map<IGPDetails>(IGPD);
                        _IGPDetails.IGPId = request.Id;
                        _IGPDetails.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _IGPDetails.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<IGPDetails>().Add(_IGPDetails);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}