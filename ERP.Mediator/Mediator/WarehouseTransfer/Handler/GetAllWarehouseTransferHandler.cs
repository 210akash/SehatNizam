using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.WarehouseTransfer.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Handler
{
    public class GetAllWarehouseTransferHandler : IRequestHandler<GetAllWarehouseTransferQuery, Tuple<IEnumerable<GetWarehouseTransfer>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllWarehouseTransferHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetWarehouseTransfer>, long>> Handle(GetAllWarehouseTransferQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.WarehouseTransfer, bool>> predicate;

            Expression<Func<Entities.Models.WarehouseTransfer, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Status,
                x => x.TransferTo,
                x => x.Company,
                x => x.TransferFrom,
                x => x.WarehouseTransferDetail.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            List<string> thenIncludes = new()
            {
                "WarehouseTransferDetail.Item",
                "WarehouseTransferDetail.CostSheet",
                "WarehouseTransferDetail.Item.UOM"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Purchase Manager"))
            {
                predicate = x => x.IsActive == true
                      && x.TransferFromId == sessionProvider.Session.SelectedWarehouseId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if(roles.Contains("Purchaser"))
            {
                predicate = x => x.IsActive == true
                      && x.TransferFromId == sessionProvider.Session.SelectedWarehouseId
                      && x.StatusId == request.StatusId
                      && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.TransferFromId == sessionProvider.Session.SelectedWarehouseId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.WarehouseTransfer, object>> OrderBy = null;
            Expression<Func<Entities.Models.WarehouseTransfer, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var WarehouseTransfer = mapper.Map<IEnumerable<GetWarehouseTransfer>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetWarehouseTransfer>, long>(WarehouseTransfer, entity.Item2);
        }
    }
}
