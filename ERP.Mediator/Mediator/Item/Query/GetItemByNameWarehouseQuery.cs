using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class GetItemByNameWarehouseQuery : IRequest<GetStock>
    {
        public GetItemByNameWarehouseQuery(string ItemName, long ProjectId)
        {
            this.ItemName = ItemName;
            this.ProjectId = ProjectId;
        }

        public string ItemName { get; set; }
        public long ProjectId { get; set; }
    }
}