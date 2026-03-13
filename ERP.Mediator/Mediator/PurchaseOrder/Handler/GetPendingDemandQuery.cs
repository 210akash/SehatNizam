using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.PurchaseOrder.Query
{
    public class GetPendingDemandQuery : IRequest<List<GetDropDown>>
    {
        //public GetPendingDemandQuery(List<long> PurchaseDemandId)
        //{
        //    this.PurchaseDemandId = PurchaseDemandId;
        //}

        public List<long> PurchaseDemandId { get; set; }
    }
}