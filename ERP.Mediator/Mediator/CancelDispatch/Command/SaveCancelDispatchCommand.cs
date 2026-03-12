using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.CancelDispatch.Command
{
    public class SaveCancelDispatchCommand : IRequest<long>
    {
        public string Code { get; set; }
        public long OrderId { get; set; }
        public string Remarks { get; set; }
        public List<GetOrderItems> GetOrderItems { get; set; }
    }
}
